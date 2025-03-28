using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptButton : MonoBehaviour
{
    [SerializeField] string prompt;
    bool isSelected;
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(SelectThisButton);
    }

    public void SelectThisButton()
    {
        PromptButton[] otherButtons = GameObject.FindObjectsOfType<PromptButton>();

        foreach (PromptButton button in otherButtons)
        {
            button.SetIsSelected(false);
        }
        isSelected = true;
    }

    public void SetIsSelected(bool state)
    {
        isSelected = state;
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    public string GetPrompt()
    {
        return prompt;
    }
}
