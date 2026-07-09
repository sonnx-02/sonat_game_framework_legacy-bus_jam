using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class DialogAddCurrency2 : DialogAddCurrencyBase
{
    [SerializeField] private BaseBaseActionScript[] mainActions;
    [SerializeField] private BaseBaseActionScript[] other;
    [SerializeField] private float delayToClose;
    

    public override void Show(Product product)
    {
        base.Show(product);
        if (delayToClose > 0)
        {
            delayToClose.Timer(Close);
            mainActions.StartAction(product);
            other.StartAction();
        }
        else
        {
            mainActions.StartAction(product);
            other.StartAction();
            Observable.EveryUpdate().Where(x => mainActions.All(y => !y.IsBusy))
                .Take(1).TakeUntilDisable(this).Subscribe(onNext: data => Close());
        }
    }
}
