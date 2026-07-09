using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RxSequenceAnimator2_InjectAndPlay : BaseFrameworkView
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera cam;
    [SerializeField] private RxSequenceAnimator2 anim;
    [SerializeField] private Transform[] tfs;

    public RxSequenceAnimator2 SequenceAnimator => anim;

    public void StartInjectAnimator<T>(T[] transforms, int i) where T : MonoBehaviour
    {
        gameObject.SetActive(true);
        var pos = transforms.Select(x =>
                (Vector3) cam.WorldToAnchorPos(x.transform.position, canvas.GetComponent<RectTransform>().sizeDelta))
            .ToArray();
        anim.Inject(pos);
        anim.Animator_In2(i);
    }


    public void Stop()
    {
        gameObject.SetActive(false);
        anim.StopAnim();
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        var pos = tfs.Select(x =>
                (Vector3) cam.WorldToAnchorPos(x.transform.position, canvas.GetComponent<RectTransform>().sizeDelta))
            .ToArray();
        anim.Inject(pos);
        anim.GetComponent<RectTransform>().anchoredPosition = pos[value];
    }


    [SerializeField] private RectTransform rt;
    [SerializeField] private RectTransform target;

    public override void LogCustom()
    {
        base.LogCustom();
        var vec = rt.anchoredPosition;
        var targetPos = rt.parent.InverseTransformPoint(target.position);
        rt.anchoredPosition = targetPos;
    }
}