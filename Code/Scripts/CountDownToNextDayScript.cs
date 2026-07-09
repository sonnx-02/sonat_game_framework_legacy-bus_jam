using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CountDownToNextDayScript : MonoBehaviour
{
    [SerializeField] private string format = "{0}";
    TextMeshProUGUI _txtResetIn;
    private Coroutine _coroutine;
    void OnEnable()
    {
        if(_txtResetIn == null)
            _txtResetIn = GetComponent<TextMeshProUGUI>();

        SetLabelTime();
       this.InvokeRepeating(SetLabelTime, 1f);
    }
  

    public void SetLabelTime()
    {
        if (gameObject.activeInHierarchy)
        {
            DateTime ts = DateTime.Today.AddDays(1);
            DateTime now = DateTime.Now;
            TimeSpan difference = ts.Subtract(now);
            var timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", difference.Hours, difference.Minutes, difference.Seconds);
            _txtResetIn.text = string.Format(format, timeStr);
        }
        else
        {
            StopCoroutine(_coroutine);
        }
    }
}


public static class MonoBehaviourExtensions
{
    public static Coroutine Invoke(this MonoBehaviour me, Action theDelegate, float time)
    {
        return me.StartCoroutine(ExecuteAfterTime(theDelegate, time));
    }

    private static IEnumerator ExecuteAfterTime(Action theDelegate, float delay)
    {
        yield return new WaitForSeconds(delay);
        theDelegate();
    }

    public static Coroutine InvokeRepeating(this MonoBehaviour me, Action theDelegate, float time)
    {
        return me.StartCoroutine(ExecuteRepeatingTime(theDelegate, time));
    }

    private static IEnumerator ExecuteRepeatingTime(Action theDelegate, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            theDelegate();
        }
    }
}