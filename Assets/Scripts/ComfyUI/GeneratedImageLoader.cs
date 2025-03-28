using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GeneratedImageLoader : MonoBehaviour
{
    [SerializeField] string historyUrl = "http://127.0.0.1:8188/history/";
    ComfyPromptCtr promptCtr;
    string fileName;

    private void Start()
    {
        promptCtr = GetComponent<ComfyPromptCtr>();
    }

    public void LoadLatestImage(string promptID)
    {
        StopAllCoroutines();
        StartCoroutine(RequestLatestImageRoutine(promptID));
    }

    IEnumerator RequestLatestImageRoutine(string promptID)
    {
        string url = historyUrl + promptID;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching history: " + webRequest.error);
                yield break;
            }

            string latestFilename = ExtractLatestFilename(webRequest.downloadHandler.text);
            if (string.IsNullOrEmpty(latestFilename))
            {
                Debug.LogError("No filename found in response.");
                yield break;
            }

            fileName = latestFilename;
            string imageURL = "http://127.0.0.1:8188/view?filename=" + latestFilename + "&type=temp&subfolder=";

            yield return StartCoroutine(DownloadImage(imageURL));
        }
    }

    string ExtractLatestFilename(string jsonString)
    {
        int lastIndex = jsonString.LastIndexOf("\"filename\":");
        if (lastIndex == -1) return null;

        int firstQuote = jsonString.IndexOf("\"", lastIndex + 11);
        int secondQuote = jsonString.IndexOf("\"", firstQuote + 1);
        return firstQuote != -1 && secondQuote != -1 ? jsonString.Substring(firstQuote + 1, secondQuote - firstQuote - 1) : null;
    }

    IEnumerator DownloadImage(string imageURL)
    {
        yield return new WaitForSeconds(0.5f);
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                UIManager.Instance.SetGeneratedImage(texture);
                promptCtr.QueuePrompt();
            }
            else
            {
                Debug.LogError("Image download failed: " + webRequest.error);
            }
        }
    }

    public string GetFileName()
    {
        return fileName;
    }
}
