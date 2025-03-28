using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class ResponseData
{
    public string prompt_id;
}

public class ComfyPromptCtr : MonoBehaviour
{
    string promptJson;

    private void Start()
    {
        //LoadPromptsJson();
        //QueuePrompt();
    }

    private void LoadPromptsJson()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "RealtimePainting.json");

        // For standalone platforms (Windows, macOS, etc.)
        if (File.Exists(filePath))
        {
            promptJson = File.ReadAllText(filePath);
            Debug.Log(promptJson);
        }
        else
        {
            Debug.LogError("prompt.json file not found at: " + filePath);
        }
    }

    public void QueuePrompt()
    {
        StartCoroutine(QueuePromptCoroutine());
    }

    private IEnumerator QueuePromptCoroutine()
    {
        string url = "http://127.0.0.1:8188/prompt";

        if (string.IsNullOrEmpty(promptJson))
        {
            Debug.LogError("Prompt JSON is empty or not loaded!");
            yield break;
        }

        string promptText = GeneratePromptJson();
        promptText = promptText.Replace("Pprompt", CheckIfInputFieldIsEmpty());

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(promptText);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Prompt queued successfully." + request.downloadHandler.text);

            ResponseData data = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
            GetComponent<ComfyWebsocket>().promptID = data.prompt_id;
        }
    }

    string CheckIfInputFieldIsEmpty()
    {
        if (UIManager.Instance.GetInputField() == string.Empty)
        {
            return "Anime";
        }
        return UIManager.Instance.GetInputField();
    }

    private string GeneratePromptJson()
    {
        string guid = Guid.NewGuid().ToString();

        string promptJsonWithGuid = $@"
            {{
                ""id"": ""{guid}"",
                ""prompt"": {promptJson}
            }}";

        return promptJsonWithGuid;
    }
}
