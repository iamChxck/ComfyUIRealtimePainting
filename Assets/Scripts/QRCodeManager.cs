using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class QRCodeManager : MonoBehaviour
{
    public static QRCodeManager instance;
    public static QRCodeManager Instance=>instance;

    public RawImage rawImage; 

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }


    public void LoadImageAsTexture(string path)
    {
        Debug.Log(path);
        byte[] fileData = File.ReadAllBytes(path);
        
        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(fileData))
        {
            if (rawImage != null)
            {
                UIManager.Instance.SetQRCodeImage(texture);
                UIManager.Instance.ToggleQRCodeImage(true);
            }
        }

        else
        {
            Debug.LogError("Failed to load image.");
        }
    }
}
