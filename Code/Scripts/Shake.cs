using System.Collections;
using UniRx;
using UnityEngine;

public class Shake : BaseView
{
    public AnimationCurve curve;
    public float duration = 1f;
    public float delay;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        SubjectController.OnPointParameterCallBack
            .Where(x => x.enumType == BuiltInEnumType.GlobalCallBackEnum
                        && x.enumValue == (int) BaseGlobalGameEffect.ShakeCamera
                        && gameObject.activeInHierarchy)
            .Subscribe(data => { StartShaking(); }).DisposeWith(this);
    }

    [MyButton(nameof(StartShaking))] public int abc;

    public void StartShaking()
    {
        StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }
}