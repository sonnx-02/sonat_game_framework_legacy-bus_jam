using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardView : PoolItem
{
    [SerializeField] private bool debug;
    [SerializeField] protected Image image;
    [SerializeField] protected TextMeshProUGUI txtAmount;
    [SerializeField] protected ToggleScript[] claimed;
    [SerializeField] protected TextMeshProUGUI otherText;
    [SerializeField] protected Button previewButton;
    [SerializeField] protected IndexBindingScriptBase[] amountBindings;
    [SerializeField] protected IndexBindingScriptBase[] quantityBindings;
    [SerializeField] protected BaseActionProductScript[] productBindings;

    [SerializeField] private string addText;
    private Product _product;
    public virtual Product Product => _product;
    public RewardView[] others;

    protected override void Register()
    {
        base.Register();
        if (previewButton != null)
            previewButton.onClick.AddListener(() =>
            {
                if (!ScreenRoot.dialogController.AnyDialogShow)
                    ScreenRoot.dialogController.Resolve<DialogQuantityPreview>()
                        .Show(_product.quantity, _product.index);
            });
    }


    [SerializeField] private bool imageIncludeParameter;

    public virtual void UpdateQuantity(int value)
    {
        _product.amount = value;
        if (txtAmount != null)
            txtAmount.text = addText + _product.amount;
        foreach (var baseIndexBindingAction in amountBindings)
            baseIndexBindingAction.OnChanged(_product.amount);
        foreach (var baseIndexBindingAction in quantityBindings)
            baseIndexBindingAction.OnChanged((int)_product.quantity);
    }

    public virtual void SetData(Product product, bool isClaimed = false)
    {
#if UNITY_EDITOR
        if (debug)
            Debug.LogError(product.GetString(),gameObject);
#endif
        foreach (var rewardView in others)
            rewardView.SetData(product, isClaimed);
        foreach (var productBinding in productBindings)
            productBinding.StartAction(product);

        _product = product;
        SetClaimed(isClaimed);

        if (image != null)
        {
            var rewardAssetDatabase = Kernel.GetDatabase<GraphicAssetDatabase>();
            var getSprite = rewardAssetDatabase.GetRewardImage(product);
            if (getSprite != null)
                image.sprite = getSprite;
            else
            {
                var find = Kernel.GetDatabase<ProductSpriteDatabase>()
                    .GetData(product, out int i, imageIncludeParameter);
                image.sprite = find?.sprite;
            }
        }
        if (txtAmount != null)
            txtAmount.text = addText + product.amount;
        foreach (var baseIndexBindingAction in amountBindings)
            baseIndexBindingAction.OnChanged(product.amount);
    }

    public void SetImage(Quantity quantity, int index)
    {
        if (image != null)
        {
            var rewardAssetDatabase = Kernel.GetDatabase<GraphicAssetDatabase>();
            var getSprite = rewardAssetDatabase.GetRewardImage(quantity, index);
            if (getSprite != null)
                image.sprite = getSprite;
            else
                image.sprite = Kernel.GetDatabase<ProductSpriteDatabase>()
                    .GetData(new Product(quantity, 0, index), out int i, false)
                    .sprite;
        }
    }

    private void Reset()
    {
        txtAmount = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponentInChildren<Image>();
    }

    public void SetClaimed(bool value)
    {
        claimed.OnChanged(value, gameObject);
    }
}