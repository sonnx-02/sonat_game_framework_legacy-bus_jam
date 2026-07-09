using TMPro;
using UnityEngine;

public class IndexBindingTextAmount : IndexBindingScript
{
    [SerializeField] private TMP_Text amount;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        amount.text = computedValue.ToString();
        if (computedValue <= 0)
        {
            amount.gameObject.SetActive(false);
        } else amount.gameObject.SetActive(true);
    }
}
