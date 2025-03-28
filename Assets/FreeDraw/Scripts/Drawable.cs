using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FreeDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Drawable : MonoBehaviour
    {
        // PEN SETTINGS
        public static Color Pen_Colour = Color.red;
        public static int Pen_Width = 3;

        public delegate void Brush_Function(Vector2 world_position);
        public Brush_Function current_brush;

        public LayerMask Drawing_Layers;
        public bool Reset_Canvas_On_Play = true;
        public Color Reset_Colour = new Color(0, 0, 0, 0);  // Transparent by default

        public bool Reset_To_This_Texture_On_Play = false;
        public Texture2D reset_texture;

        public static Drawable drawable;
        Sprite drawable_sprite;
        Texture2D drawable_texture;

        Vector2 previous_drag_position;
        Color[] clean_colours_array;
        Color transparent;
        Color32[] cur_colors;
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;

        // Flag to mark the beginning of a stroke.
        bool strokeStarted = false;

        // Stack to store previous texture states for undo functionality
        private Stack<Color[]> undoStack = new Stack<Color[]>();


        ////////////////////////////////
        // BRUSH TYPES
        ////////////////////////////////

        // Brush template example.
        public void BrushTemplate(Vector2 world_position)
        {
            Vector2 pixel_pos = WorldToPixelCoordinates(world_position);
            cur_colors = drawable_texture.GetPixels32();

            // If this is the first click of the stroke, ensure state is saved.
            if (!strokeStarted)
            {
                SaveStateForUndo();
                strokeStarted = true;
            }
            // First click: mark pixel; subsequent: draw between points.
            if (previous_drag_position == Vector2.zero)
            {
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ApplyMarkedPixelChanges();
            previous_drag_position = pixel_pos;
        }

        // Default pen brush.
        public void PenBrush(Vector2 world_point)
        {
            Vector2 pixel_pos = WorldToPixelCoordinates(world_point);
            cur_colors = drawable_texture.GetPixels32();

            // Save state only once per stroke.
            if (!strokeStarted)
            {
                SaveStateForUndo();
                strokeStarted = true;
            }

            if (previous_drag_position == Vector2.zero)
            {
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ApplyMarkedPixelChanges();
            previous_drag_position = pixel_pos;
        }

        public void SetPenBrush()
        {
            current_brush = PenBrush;
        }

        // Fill brush implementation.
        public void FillBrush(Vector2 world_position)
        {
            Vector2 pixel_pos = WorldToPixelCoordinates(world_position);
            int x = (int)pixel_pos.x;
            int y = (int)pixel_pos.y;

            // Here you can decide if you want fill to be a separate undo step.
            // For this example, we save an undo state for fill as well.
            SaveStateForUndo();

            Color target_color = drawable_texture.GetPixel(x, y);
            if (target_color == Pen_Colour) return;

            Queue<Vector2> pixels = new Queue<Vector2>();
            pixels.Enqueue(new Vector2(x, y));

            while (pixels.Count > 0)
            {
                Vector2 current_pixel = pixels.Dequeue();
                int cx = (int)current_pixel.x;
                int cy = (int)current_pixel.y;

                if (cx < 0 || cx >= drawable_texture.width || cy < 0 || cy >= drawable_texture.height)
                    continue;
                if (drawable_texture.GetPixel(cx, cy) != target_color)
                    continue;

                drawable_texture.SetPixel(cx, cy, Pen_Colour);
                pixels.Enqueue(new Vector2(cx + 1, cy));
                pixels.Enqueue(new Vector2(cx - 1, cy));
                pixels.Enqueue(new Vector2(cx, cy + 1));
                pixels.Enqueue(new Vector2(cx, cy - 1));
            }

            drawable_texture.Apply();
        }

        public void SetFillBrush()
        {
            current_brush = FillBrush;
        }


        ////////////////////////////////
        // UPDATE METHOD & UNDO TRIGGER
        ////////////////////////////////
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                Undo();
            }

            bool mouse_held_down = Input.GetMouseButton(0);
            if (mouse_held_down && !no_drawing_on_current_drag)
            {
                Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                if (hit != null && hit.transform != null)
                {
                    // When the stroke is starting (mouse just pressed), we ensure a state is saved.
                    if (!strokeStarted)
                    {
                        SaveStateForUndo();
                        strokeStarted = true;
                    }
                    current_brush(mouse_world_position);
                }
                else
                {
                    previous_drag_position = Vector2.zero;
                    if (!mouse_was_previously_held_down)
                    {
                        no_drawing_on_current_drag = true;
                    }
                }
            }
            else if (!mouse_held_down)
            {
                // Reset stroke state when mouse is released.
                previous_drag_position = Vector2.zero;
                no_drawing_on_current_drag = false;
                strokeStarted = false;
            }
            mouse_was_previously_held_down = mouse_held_down;
        }


        ////////////////////////////////
        // UNDO FUNCTIONALITY
        ////////////////////////////////
        public void SaveStateForUndo()
        {
            Color[] pixels = drawable_texture.GetPixels();
            undoStack.Push((Color[])pixels.Clone());
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                Color[] previousPixels = undoStack.Pop();
                drawable_texture.SetPixels(previousPixels);
                drawable_texture.Apply();
            }
            else
            {
                Debug.Log("No more undo steps available.");
            }
        }


        ////////////////////////////////
        // HELPER METHODS FOR DRAWING
        ////////////////////////////////
        public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
        {
            float distance = Vector2.Distance(start_point, end_point);
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                Vector2 cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, width, color);
            }
        }

        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                if (x >= (int)drawable_sprite.rect.width || x < 0)
                    continue;
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    MarkPixelToChange(x, y, color_of_pen);
                }
            }
        }

        public void MarkPixelToChange(int x, int y, Color color)
        {
            int array_pos = y * (int)drawable_sprite.rect.width + x;
            if (array_pos >= cur_colors.Length || array_pos < 0)
                return;
            cur_colors[array_pos] = color;
        }

        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }

        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }
            drawable_texture.Apply();
        }

        public Vector2 WorldToPixelCoordinates(Vector2 world_position)
        {
            Vector3 local_pos = transform.InverseTransformPoint(world_position);
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;
            return new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));
        }

        public Vector3 PixelToWorldCoordinates(Vector2 pixel_pos)
        {
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;
            float uncentered_x = pixel_pos.x / unitsToPixels - pixelWidth / 2;
            float uncentered_y = pixel_pos.y / unitsToPixels - pixelHeight / 2;
            return transform.TransformPoint(new Vector3(uncentered_x, uncentered_y, 0f));
        }

        public void ResetCanvas()
        {
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();
        }

        ////////////////////////////////
        // INITIALIZATION
        ////////////////////////////////
        void Awake()
        {
            drawable = this;
            current_brush = PenBrush;
            drawable_sprite = GetComponent<SpriteRenderer>().sprite;
            drawable_texture = drawable_sprite.texture;

            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            for (int x = 0; x < clean_colours_array.Length; x++)
                clean_colours_array[x] = Reset_Colour;

            if (Reset_Canvas_On_Play)
                ResetCanvas();
            else if (Reset_To_This_Texture_On_Play)
            {
                Graphics.CopyTexture(reset_texture, drawable_texture);
                Debug.Log("Reset texture");
            }
        }
    }
}
