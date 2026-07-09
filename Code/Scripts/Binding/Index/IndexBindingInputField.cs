using TMPro;
using UnityEngine;

namespace BlockJam
{
    public class IndexBindingInputField : IndexBindingScript
    {
        [SerializeField] private TMP_InputField amountInput;

        protected override void Awake()
        {
            base.Awake();
            amountInput.onEndEdit.AddListener(number =>
            {
                GetComponent<ButtonPoint>().Point.value4 = int.Parse(number);
                GetComponent<ButtonPoint>().TriggerOnClick();
            });
        }

        protected override void OnValueChanged(int computedValue)
        {
            base.OnValueChanged(computedValue);
            amountInput.gameObject.SetActive(computedValue == 64);
        }
    }
}
