using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TS.ColorPicker
{
    public class InputChannel : MonoBehaviour
    {
        #region Variables

        [Header("Inner")]
        [SerializeField] private TMP_Text _text;
        private TMP_InputField _input;

        public delegate void OnValueChanged(InputChannel sender, float value, int value32);
        public OnValueChanged ValueChanged;

        public string Label
        {
            get { return _text.text; }
            set { _text.text = value; }
        }
        public float Value
        {
            get { return Value32 / 255f; }
            set { Value32 = Mathf.RoundToInt(value * 255f); }
        }
        public int Value32
        {
            get
            {
                if (_input == null || string.IsNullOrEmpty(_input.text)) return 0; // Prevents NullReferenceException
                return Mathf.Clamp(int.Parse(_input.text), 0, 255);
            }
            set
            {
                if (_input != null)
                {
                    _input.text = Mathf.Clamp(value, 0, 255).ToString();
                }
                else
                {
                    Debug.LogWarning("Attempted to set Value32, but _input is null.");
                }
            }
        }

        #endregion

        private void Awake()
        {
            _input = GetComponent<TMP_InputField>(); // Initialize before other scripts access it
        }

        private void Start()
        {
            if (_input == null)
            {
                Debug.LogError("TMP_InputField component is missing on the GameObject!", this);
                return;
            }

            _input.onEndEdit.AddListener(Input_EndEdit);
        }

        private void Input_EndEdit(string arg0)
        {
            if (string.IsNullOrEmpty(arg0)) { return; }
            ValueChanged?.Invoke(this, Value, Value32);
        }
    }
}
