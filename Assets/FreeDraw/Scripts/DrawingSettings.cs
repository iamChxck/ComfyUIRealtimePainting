using System.Collections;
using System.Collections.Generic;
using TS.ColorPicker;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FreeDraw
{
    // Helper methods used to set drawing settings
    public class DrawingSettings : MonoBehaviour
    {
        [SerializeField] Slider slider;

        public static bool isCursorOverUI = false;
        public float Transparency = 1f;

        Color currPenColor;

        private void Start()
        {
            slider.value = Drawable.Pen_Width;
            currPenColor = Drawable.Pen_Colour;
        }

        // Changing pen settings is easy as changing the static properties Drawable.Pen_Colour and Drawable.Pen_Width
        public void SetMarkerColour(Color new_color)
        {
            Drawable.Pen_Colour = new_color;
        }
        // new_width is radius in pixels
        public void SetMarkerWidth(int new_width)
        {
            Drawable.Pen_Width = new_width;
        }
        public void SetMarkerWidth(float new_width)
        {
            SetMarkerWidth((int)new_width);
        }

        public void SetTransparency(float amount)
        {
            Transparency = amount;
            Color c = Drawable.Pen_Colour;
            c.a = amount;
            Drawable.Pen_Colour = c;
        }

        public void SetPen(Color color)
        {
            SetMarkerColour(color);
            Drawable.drawable.SetPenBrush();
        }

        // Call these these to change the pen settings
        public void SetPen()
        {
            Color c = currPenColor;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }

        public void SetPenRed()
        {
            Color c = Color.red;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetPenGreen()
        {
            Color c = Color.green;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetPenBlue()
        {
            Color c = Color.blue;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetEraser()
        {
            currPenColor = Drawable.Pen_Colour;
            SetMarkerColour(new Color(255f, 255f, 255f, 0f));
        }

        public void PartialSetEraser()
        {
            SetMarkerColour(new Color(255f, 255f, 255f, 0.5f));
        }

        public void SetFillBrush()
        {
            Drawable.drawable.SetFillBrush();
        }
    }
}