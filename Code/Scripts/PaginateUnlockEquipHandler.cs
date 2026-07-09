using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PaginateUnlockEquipHandler : BaseFrameworkView
{
    [SerializeField] private BaseArrayIntPropertySourceRetriever retriever;
    [SerializeField] private UnlockEquipManagerDependency[] equips;
    [SerializeField] private UnlockEquipItem currentEquip;
    private ArrayIntReactiveProperty _arrayInt;
    private IntReactiveProperty _pageIndexProperty;

    protected override void Register()
    {
        base.Register();
    }

    public void Setup(IntReactiveProperty property)
    {
        _arrayInt = retriever.GetProperty();
        _pageIndexProperty = property;
        _pageIndexProperty.Subscribe(data => OnUpdated(data));
        for (var index = 0; index < equips.Length; index++)
        {
            var i = index;
            if (equips[index] != null)
                equips[index].currentSelectIndex.Subscribe(data => OnUpdated(i, false));
        }

        _arrayInt.Subscribe(data =>
        {
            if(data != null)
                UIDebugLog.Log0(data.toString());
        });
        SubjectController.OnEquipDataUpdate.Subscribe(data => { UpdateCurrentView(); });
    }

    private void OnUpdated(int index, bool force = true)
    {
        if (force || index == _pageIndexProperty.Value)
        {
            if (index >= 0 && equips[index] != null)
                _arrayInt.Value = new ArrayInt(new[]
                {
                    _pageIndexProperty.Value,
                    equips[Mathf.Clamp(_pageIndexProperty.Value, 0, equips.Length)].CurrentSelectIndex.Value,
                });
            else
                _arrayInt.Value = null;
            UpdateCurrentView();
        }
    }

    private void UpdateCurrentView()
    {
        if (currentEquip == null)
            return;
        if(_arrayInt.Value == null)
            return;
        var ce = equips[Mathf.Clamp(_arrayInt.Value.values[0], 0, equips.Length)];
        if (ce != null)
            ce.GetCurrentEquip().SyncTo(currentEquip);
    }
}