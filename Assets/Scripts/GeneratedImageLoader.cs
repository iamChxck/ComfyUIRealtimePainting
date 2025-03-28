using System.Collections;
using UnityEngine;
using System.IO;
using System.Linq;

public class GeneratedImageLoader : MonoBehaviour
{
    [SerializeField] string imageFolderPath = "C:\\Users\\Lapto\\Documents\\GitHub\\ComfyUI\\temp";
    
    ComfyPromptCtr promptCtr;
    bool hasStarted = false;

    private void Start()
    {
        promptCtr = GetComponent<ComfyPromptCtr>();
    }

    public void LoadLatestImage()
    {
        if (!string.IsNullOrEmpty(GetLatestImagePath()))
        {
            StartCoroutine(LoadImageFromFile(GetLatestImagePath()));
        }
        else
        {
            Debug.LogError("No image found in the specified folder.");
        }
    }

    private string GetLatestImagePath()
    {
        if (!Directory.Exists(imageFolderPath))
        {
            Debug.LogError("Directory does not exist: " + imageFolderPath);
            return null;
        }

        DirectoryInfo dir = new DirectoryInfo(imageFolderPath);
        FileInfo latestFile = dir.GetFiles("*.png")
                                 .OrderByDescending(f => f.LastWriteTime)
                                 .FirstOrDefault();

        return latestFile != null ? latestFile.FullName : null;
    }

    IEnumerator LoadImageFromFile(string filePath)
    {
        byte[] imageData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            UIManager.Instance.SetGeneratedImage(sprite);
            promptCtr.QueuePrompt();
        }
        else
        {
            Debug.LogError("Failed to load image from file.");
        }
        yield return null;
    }

    public void StartCapture()
    {
        hasStarted = true;
    }

    public void StopCapture()
    {
        hasStarted = false;
    }
}
