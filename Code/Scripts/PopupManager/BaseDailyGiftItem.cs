using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseDailyGiftItemView : BaseFrameworkView
{
    [SerializeField] private RewardView rewardView;
    [SerializeField] private IndexBindingScriptBase title;
    [SerializeField] private ToggleScript current;
    [SerializeField] private ToggleScript taken;

    [SerializeField] public ToggleScript border;
    [SerializeField] public Button button;


    public bool IsCurrent => current.CurrentValue;
    private int _index;

    public void Setup(Product product, int index, int max)
    {
        _index = index;
        title.OnChanged(index);
        rewardView.SetData(product);
        UpdateStatus(max);
    }

    public bool UpdateStatus(int max)
    {
        if (_index < max)
            taken.OnChanged(PlayerData.dailyGiftProgress > _index ||
                            PlayerData.dailyGiftProgress == _index && DailyData.dailyGiftChecked);
        else
            taken.OnChanged(DailyData.dailyGiftChecked && PlayerData.dailyGiftProgress >= _index);
        bool isCurrent = _index == PlayerData.dailyGiftProgress;
        if (PlayerData.dailyGiftProgress >= 5)
            isCurrent = _index == 4;
        current.OnChanged(isCurrent);
        return _index == PlayerData.dailyGiftProgress;
    }
}