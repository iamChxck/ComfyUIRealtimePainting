#region Includes
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#endregion

namespace TS.ColorPicker
{
    public class InputHex : MonoBehaviour
    {
        #region Variables

        [Header("Inner")]
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_InputField _input;

        public delegate void OnValueChanged(InputHex sender, Color color);
        public OnValueChanged ValueChanged;

        public string Label
        {
            get { return _text.text; }
            set { _text.text = value; }
        }
        public Color Value
        {
            get
            {
                ColorUtility.TryParseHtmlString(string.Format("#{0}", _input.text), out Color color);
                return color;
            }
            set
            {
                _input.text = ColorUtility.ToHtmlStringRGB(value);
            }
        }

        #endregion

        private void Start()
        {
            _input.onEndEdit.AddListener(Input_EndEdit);
        }

        private void Input_EndEdit(string arg0)
        {
            if (string.IsNullOrEmpty(arg0)) { return; }
            ValueChanged?.Invoke(this, Value);
        }
    }
}