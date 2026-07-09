using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ToggleLink : BaseView
{
    [SerializeField] private ToggleScript[] others;
    [SerializeField] private bool setActiveThis = true;
    [SerializeField] private bool reverse;
    [SerializeField] [IndexAsEnum(BuiltInEnumType.ToggleLink)] private int toggleLink;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        SubjectController.ToggleLink.Where(x => x.key == toggleLink).Subscribe(data =>
        {
            var rev = reverse ? !data.value : data.value;
                
                if(setActiveThis)
                    gameObject.SetActive(rev);
                others.OnChanged(rev);
            });
    }
}
