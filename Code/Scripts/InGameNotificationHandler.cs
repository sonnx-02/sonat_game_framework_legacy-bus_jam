using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class InGameNotificationHandler : BaseFrameworkView
{
    [SerializeField] private BindingToStringTextChange textChange;
    [SerializeField] private BaseRxAnimator[] anims;

    public override void LogCustom()
    {
        base.LogCustom();
        SubjectController.InGameNotification.OnNext(new InGameNotification()
        {
            message = "hello test",
            name = "test ntofifi"
        });
    }

    private void OnEnable()
    {
        SubjectController.InGameNotification.TakeUntilDisable(this)
            .Subscribe(data =>
        {
            Debug.Log(data.message);
            if(gameObject.activeInHierarchy)
                AddQueue(data);
        });
    }

    private Queue<InGameNotification> notifications = new Queue<InGameNotification>();

    private void AddQueue(InGameNotification notification)
    {
        notifications.Enqueue(notification);
        if (!_busy)
        {
            _busy = true;
            Observable.FromCoroutine(IeNotify).Subscribe();
        }
    }

    private bool _busy;

    private IEnumerator IeNotify()
    {
        while (notifications.Count > 0)
        {
            var notification = notifications.Dequeue();
            textChange.OnChanged(notification.message);
            foreach (var baseRxAnimator in anims)
                baseRxAnimator.Animator_InThenOut();
            while (anims.Any(x => x.IsAnimating()))
                yield return null;
        }

        _busy = false;
    }
}