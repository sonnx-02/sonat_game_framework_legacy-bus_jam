using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndexBindingScriptGetRewardsBase : IndexBindingScript
{
    [SerializeField] protected RewardView[] rewardViews;

    protected ListProduct Rewards { get; set; }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        rewardViews.SetActiveAll(false);
    }

    public virtual void Claim()
    {
        Debug.LogError("claim");
        PlayerData.Claim(Rewards.products);
    }

    [MyButtonInt(nameof(Multiple))] [SerializeField]
    private int test = 2;

    [SerializeField] private BoolFloat tweenTime = new BoolFloat(false,0.5f);
    public void Multiple(int multi)
    {
        StartCoroutine(Tween(multi));
    }

    private IEnumerator Tween(int multi)
    {
        var from = Rewards.products.Select(x => x.amount).ToArray();
        var to = Rewards.products.Select(x => x.amount * multi).ToArray();
        float t = 0;
        yield return null;
        if (tweenTime.value)
            while (t < tweenTime.value2)
            {

                t += Time.deltaTime;
                for (var i = 0; i < Rewards.products.Length; i++)
                {
                    Rewards.products[i].amount = (int)Mathf.Lerp(from[i], to[i], t / tweenTime.value2);
                    rewardViews[i].UpdateQuantity(Rewards.products[i].amount);
                }
                yield return null;
            }
        else
        {
            for (var i = 0; i < Rewards.products.Length; i++)
            {
                Rewards.products[i].amount = to[i];
                rewardViews[i].UpdateQuantity(Rewards.products[i].amount);
            }
        }
    }
}