using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineUtilities
{
    public static IEnumerator WaitForRealTime(float delay)
    {
        //yield return new WaitForSeconds(delay);
        while (true)
        {
            float pauseEndTime = Time.realtimeSinceStartup + delay;
            while (Time.realtimeSinceStartup < pauseEndTime)
            {
                yield return 0;
            }
            break;
        }
    }
}