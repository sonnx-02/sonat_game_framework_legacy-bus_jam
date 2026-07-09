using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

#if USE_SPINE
using Spine.Unity;
public class SpineButton : BaseButton
{
    public SkeletonGraphic skeletonGraphic;
    [SpineAnimation]
    public string lightAnim;
    [SpineAnimation]
    public string tap;
  
    private void OnEnable()
    {
        if ( skeletonGraphic != null )
            skeletonGraphic.AnimationState.SetAnimation(0, lightAnim, true);
    }

    protected override void OnClick()
    {
        base.OnClick();
        skeletonGraphic.AnimationState.SetAnimation(0, tap, false);
    }

//    public void Start()
//    {
//    }
//    public override void OnClick()
//    {
//        if (m_ButtonName == Buttons.BUTTON_ADVENTURES)
//        {
//            AudioPlay.audioPlay.AudioClickButton();
//            skeletonGraphic.AnimationState.SetAnimation(0, tap, false);
//            LeanTween.delayedCall(0.3f, () =>
//            {
//                WorldMap.instance.Open(3);
//            });
//            // Kernel.Resolve<FireBaseController>().LogEvent(FireBaseController.home_Scene, "action_name", "click_Play");
//        }
//        else if (m_ButtonName == Buttons.BUTTON_SHOPIAP)
//        {
//            AudioPlay.audioPlay.AudioClickButton();
//            skeletonGraphic.AnimationState.SetAnimation(0, tap, false);
//            LeanTween.delayedCall(0.3f, () =>
//            {
//                ShopIAP.m_Tab = TabShopIAP.OFFER;
//                UIManager.instance.ShowPopup(Popups.POPUP_SHOPIAP);
//            });
//            // Kernel.Resolve<FireBaseController>().LogEvent(FireBaseController.home_Scene, "action_name", "click_Shop");
//        }
//        else if (m_ButtonName == Buttons.BUTTON_SHOP_TABPLANE)
//        {
//            AudioPlay.audioPlay.AudioClickButton();
//            skeletonGraphic.AnimationState.SetAnimation(0, tap, false);
//            LeanTween.delayedCall(0.3f, () =>
//            {
//                Shop.m_Tab = TabShop.TABPLANES;
//                UIManager.instance.ShowPopup(Popups.POPUP_SHOP);
//            });
//            // Kernel.Resolve<FireBaseController>().LogEvent(FireBaseController.home_Scene, "action_name", "click_Upgrade");
//        }
//        else
//        {
//            base.OnClick();
//        }
//    }
}
#endif