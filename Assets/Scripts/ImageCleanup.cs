using System.IO;
using System.Linq;
using UnityEngine;

public class ImageCleanup : MonoBehaviour
{
    [SerializeField] private string imageFolderPath = "C:\\Users\\user\\Desktop\\CHUCK\\ComfyUI\\ComfyUI\\temp";
    [SerializeField] private int maxImages = 30;
    [SerializeField] private float cleanupInterval = 60f; // Run every 60 seconds

    private void Start()
    {
        InvokeRepeating("CheckAndDeleteOldImages", cleanupInterval, cleanupInterval);
    }

    private void CheckAndDeleteOldImages()
    {
        if (!Directory.Exists(imageFolderPath)) return;

        FileInfo[] imageFiles = new DirectoryInfo(imageFolderPath)
            .GetFiles("*.png")
            .OrderBy(f => f.CreationTime)
            .ToArray();

        if (imageFiles.Length >= maxImages)
        {
            FileInfo oldestFile = imageFiles.FirstOrDefault();
            if (oldestFile != null)
            {
                File.Delete(oldestFile.FullName);
                Debug.Log("Deleted oldest image: " + oldestFile.Name);
            }
        }
    }
}
