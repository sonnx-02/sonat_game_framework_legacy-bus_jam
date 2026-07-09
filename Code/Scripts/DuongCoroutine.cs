using System;
using System.Collections;
using UnityEngine;

public static class DuongCoroutine
{
    public static IEnumerator IeLoopYoYo(float from, float to, float time, Action<float> onUpdate, float delay = 0,
        float delay2 = 0f)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        float t = 0f;
        onUpdate(from);
        while (t < time)
        {
            yield return null;
            t += Time.deltaTime;
            onUpdate(Mathf.Lerp(from, to, t / time));
        }
     
        onUpdate(to);
        if (delay2 > 0)
            yield return new WaitForSeconds(delay2);
  
        
        t = 0;
        while (t < time)
        {
            yield return null;
            t += Time.deltaTime;
            onUpdate(Mathf.Lerp(to, from, t / time));
        }
        
        onUpdate(from);
    }
}