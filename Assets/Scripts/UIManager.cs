using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] RawImage generatedImage;
    [SerializeField] GameObject qrCodeImage; 

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    public void SetGeneratedImage(Texture2D texture)
    {
        generatedImage.texture = texture;
    }

    public void SetQRCodeImage(Texture2D texture)
    {
        qrCodeImage.GetComponent<RawImage>().texture = texture;
    }

    public void ToggleQRCodeImage(bool state)
    {
        qrCodeImage.SetActive(state);
    }
}
