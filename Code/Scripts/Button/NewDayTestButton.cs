using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NewDayTestButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ReactivePlayerDailyData.ForceNewDay = true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
