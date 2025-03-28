using FreeDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPickerButton : MonoBehaviour
{
    [SerializeField] Drawable drawableGO;


    public void DeactivateDrawable()
    {
        drawableGO.enabled = false;
    }

    public void ActivateDrawable()
    {
        drawableGO.enabled = true;
    }    
}
