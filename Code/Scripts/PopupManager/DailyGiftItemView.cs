using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyGiftItemView : BaseFrameworkView
{
    [SerializeField] private RewardView rewardView;
    [SerializeField] private IndexBindingScriptBase title;
    [SerializeField] private ToggleScript[] currents;
    [SerializeField] private ToggleScript[] takens;
    [SerializeField] private ToggleScript[] borders;
    [SerializeField] public Button button;

    public bool IsCurrent => _isCurrent;
    private int _index;

    public void Setup(Product product, int index, int max)
    {
        _index = index;
        title.OnChanged(index);
        rewardView.SetData(product);
        UpdateStatus(max);
    }

    private bool _isCurrent;
    public bool UpdateStatus(int max)
    {
        if (_index < max)
            takens.OnChanged(PlayerData.dailyGiftProgress > _index ||
                            PlayerData.dailyGiftProgress == _index && DailyData.dailyGiftChecked);
        else
            takens.OnChanged(DailyData.dailyGiftChecked && PlayerData.dailyGiftProgress >= _index);
        _isCurrent = _index == PlayerData.dailyGiftProgress;
        if (PlayerData.dailyGiftProgress >= max)
            _isCurrent = _index == max - 1;
        currents.OnChanged(_isCurrent);
        return _index == PlayerData.dailyGiftProgress;
    }
    

    public void SetBorder(bool hasBorder)
    {
        borders.OnChanged(hasBorder);
    }
}