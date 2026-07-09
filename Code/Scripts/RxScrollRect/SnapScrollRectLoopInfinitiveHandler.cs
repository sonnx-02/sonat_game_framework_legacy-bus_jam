using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class SnapScrollRectLoopInfinitiveHandler : BaseFrameworkView
{
    [SerializeField] private SnapScrollRectLoopInfinitive scrollRect;
    [SerializeField] private BaseFrameworkView[] items;
    [SerializeField] private bool populateOnEnable;

  
    
    public void Init()
    {
        DuongExtensions.ExecuteInNextFrame(Wait);
    }

    protected override void Register()
    {
        base.Register();
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        scrollRect.onChanged.Subscribe(Handler);
   

        SubjectController.InAppPurchasedEvent
            .TakeUntilDestroy(this)
            .Subscribe(data =>
            {
                _actives = ActiveList().ToArray();
                scrollRect.Populate(_actives.Length);
                DuongExtensions.ExecuteInNextFrame(Wait);
            })
            .DisposeWith(this);
    }

    private void OnEnable()
    {
        if (populateOnEnable)
        {
            CheckKernelLoaded();
             _actives = ActiveList().ToArray();
            scrollRect.Populate(_actives.Length);
            DuongExtensions.ExecuteInNextFrame(Wait);
        }
    }

    private BaseFrameworkView[] _actives;

    private IEnumerable<BaseFrameworkView> ActiveList()
    {
        for (var index = 0; index < items.Length; index++)
        {
            var monoBehaviour = items[index];
            monoBehaviour.CheckKernelLoaded();
            if (monoBehaviour.GetComponent<IDisplayable>() != null &&
                monoBehaviour.GetComponent<IDisplayable>().IsDisplayable())
            {
                yield return monoBehaviour;
            }
        }
    }

    private void Wait()
    {
//        var actives = items.Select(x => x.activeSelf);
    }

    private IList<int> _centered;
    private void Handler(bool remapChanged)
    {
        if(_actives.Length == 0)
            return;
        _centered = scrollRect.GetCentered();
        if (remapChanged)
        {
            var itemIndex = new int[Mathf.Min(_actives.Length,3)];
            for (var i = 0; i < _actives.Length; i++)
            {
                if (i < itemIndex.Length)
                {
                    itemIndex[i] = _centered[i].Repeat(_actives.Length - 1);
                    _actives[itemIndex[i]].transform.SetParentAndExpand(scrollRect.CurrentItems[_centered[i]].transform, true, true);
                }
                else
                {
                    _actives[_centered[i].Repeat(_actives.Length - 1)].gameObject.SetActive(false);
                }
            }
        }
        scrollRect.ChangePagination(_centered[0].Repeat(_actives.Length - 1));
    }
}
