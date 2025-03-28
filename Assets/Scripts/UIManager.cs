using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Image generatedImage;
    [SerializeField] TMP_InputField inputField;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    public void SetGeneratedImage(Sprite sprite)
    {
        generatedImage.sprite = sprite;
    }

    public string GetInputField()
    {
        return inputField.text;
    }
}
