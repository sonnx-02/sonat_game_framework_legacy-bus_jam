using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogSelectIndex : BaseDialog
{
    [SerializeField] private GenericSelectIndexItem[] selectItems;
    private readonly IntReactiveProperty _current = new IntReactiveProperty(); 
    
//    public Subject<int> onSelect = new Subject<int>();

    [SerializeField] private Button selectButton;

    protected override void Register()
    {
        base.Register();
        selectButton.onClick.AddListener(OnSelect);
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        _current.Subscribe(data =>
        {
            foreach (var item in selectItems)
                item.ChangeSelect(data);
        });
    }

    protected virtual void OnSelect()
    {
        _updateAction.Invoke(_current.Value);
        Close();
    }

    public override void Show()
    {
        base.Show();
        for (var i = 0; i < selectItems.Length; i++)
            selectItems[i].gameObject.SetActive(false);
        
        for (var i = 0; i < Kernel.Resolve<PlayFabManager2>().TotalAvatar; i++)
        {
            selectItems[i].gameObject.SetActive(true);
            selectItems[i].SetIndex(_current,i);
        }
    }

    private Action<int> _updateAction;
    public void ShowIndex(int current,Action<int> afterIndex)
    {
        _current.Value = current;
        _updateAction = afterIndex;
        Show();
    }
}
