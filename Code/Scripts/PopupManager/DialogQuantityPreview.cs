using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogQuantityPreview : BaseDialog
{
    public RewardView rewardView;
    public TextMeshProUGUI txtDescription;

    public void Show(Product product)
    {
        Show(product.quantity, product.index);
    }

    public void Show(Quantity quantity,int index)
    {
        base.Show();
        if(rewardView != null)
            rewardView.SetImage(quantity,index);
        txtDescription.text = Kernel.GetDatabase<GraphicAssetDatabase>().GetPreviewDescription(quantity, index);
    }
}
