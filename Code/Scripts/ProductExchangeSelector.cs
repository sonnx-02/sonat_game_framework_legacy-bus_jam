//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UniRx;
//using UnityEngine;
//using UnityEngine.UI;
//
//public class ProductExchangeSelector : MonoBehaviour
//{
//    [SerializeField] private ProductExchangeSelectItem[] items;
//    [SerializeField] private IntReactiveProperty current;
//    [SerializeField] private Button button;
//    [SerializeField] private SelectorAction action;
//
//
//    private enum SelectorAction
//    {
//        CloseAndReopenDialog,
//        ClosePopup
//    }
//    
//    private void Start()
//    {
//        button.onClick.AddListener(() =>
//        {
//            var trade = items[current.Value].GetTrade();
//            if (trade.IsAffordable)
//            {
//                trade.Spend();
//                trade.Claim();
//                switch (action)
//                {
//                    case SelectorAction.CloseAndReopenDialog:
//                        RootView.rootView.screenRoot.dialogController.CloseAll();
//                        break;
//                    case SelectorAction.ClosePopup:
//                        RootView.rootView.screenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe();
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//            }
//            else
//            {
//                switch (action)
//                {
//                    case SelectorAction.CloseAndReopenDialog:
//                        RootView.rootView.screenRoot.popupManager.ShowPopup<PopupShop>(0,WhenPopupClose.OpenLastDialog);
//                        break;
//                    case SelectorAction.ClosePopup:
//                        RootView.rootView.screenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe();
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//            }
//        });
//        
//        for (var i = 0; i < items.Length; i++)
//        {
//            var i1 = i;
//            items[i].Btn.onClick.AddListener(() =>
//            {
//                current.Value = i1;
//            });
//        }
//
//        current.Subscribe(data =>
//        {
//            for (var i = 0; i < items.Length; i++)
//                items[i].OnSelect(i == data);
//        });
//    }
//
//    void OnEnable()
//    {
//        
//    }
//}
