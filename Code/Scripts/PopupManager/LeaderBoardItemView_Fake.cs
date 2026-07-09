using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderBoardItemView_Fake : PoolItem
{
    [SerializeField] public BindingToStringTextChange profileName;
    [SerializeField] private IndexBindingScriptBase[] rank;
    [SerializeField] public IndexBindingScriptBase score;
    [SerializeField] private IndexBindingScript avatarBindings;
    [SerializeField] private ToggleScript[] isCurrent;

    public void SetData(LeaderBoardData data)
    {
        CheckRegister();
        isCurrent.OnChanged(data.isPlayer);
        rank.OnChanged(data.rank);
        if (profileName != null)
            profileName.OnChanged(data.name);
        score.OnChanged(data.score);
        Debug.LogError("avatarBindings.OnChanged(data.avatarIndex)");
        if (avatarBindings.Size > 0)
            avatarBindings.OnChanged(data.avatarIndex);
    }
}