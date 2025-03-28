using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class QRCodeData
{
    public string qr_code;
    public string url;
}
public class UploadImage : MonoBehaviour
{
    public RawImage rawImage;
    private string uploadURL = "http://192.168.1.246:5000/upload";
    [SerializeField] GeneratedImageLoader m_generatedImage;
    [SerializeField] string m_extractedPath;
    public void StartUpload()
    {
        StartCoroutine(UploadFile());
    }

    private IEnumerator UploadFile()
    {
        Texture2D texture2D = ConvertToTexture2D(rawImage.texture);
        if (texture2D == null)
        {
            Debug.LogError("Failed to convert texture to Texture2D.");
            yield break;
        }

        byte[] textureBytes = texture2D.EncodeToPNG();

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", textureBytes, $"{m_generatedImage.GetFileName()}", "image/png");

        UnityWebRequest www = UnityWebRequest.Post(uploadURL, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError(www.error);
        else
        {
            string jsonResponse = www.downloadHandler.text;
            m_extractedPath = ExtractImagePath(jsonResponse);
            Debug.Log("File uploaded successfully: " + jsonResponse);
            QRCodeManager.Instance.LoadImageAsTexture(m_extractedPath);

        }
    }

    public Texture2D ConvertToTexture2D(Texture texture)
    {
        if (texture is Texture2D)
            return (Texture2D)texture;

        RenderTexture renderTexture = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(texture, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D newTexture = new Texture2D(texture.width, texture.height);
        newTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        newTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return newTexture;
    }

    public string ExtractImagePath(string jsonString)
    {
        QRCodeData qrCodeData = JsonUtility.FromJson<QRCodeData>(jsonString);
        return qrCodeData.qr_code;
    }
}