using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ClickStreamShowButton : MonoBehaviour
{
    [SerializeField] private GameObject[] shows;
    [SerializeField] private float throttleTime = 2000;
    [SerializeField] private int count = 5;
    [SerializeField] private bool showPopupTest;
    [SerializeField] private string setKeyTrue = "logEnabled";

    // Start is called before the first frame update
    void Start()
    {
        foreach (var show in shows)
            show.gameObject.SetActive(false);
        
        if (GetComponent<Image>() != null)
            GetComponent<Image>().color = new Color();
//        
        Debug.Log("clickStream");
        var clickStream = GetComponent<Button>().OnPointerClickAsObservable();
//            clickStream.Subscribe(data => Debug.Log("click"));
        clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(throttleTime)))
            .Where(xs => xs.Count >= count)
            .Subscribe(xs =>
            {
                foreach (var show in shows)
                    show.gameObject.SetActive(true);
                if (showPopupTest)
                    RootView.rootView.screenRoot.popupManager.ShowPopup<PopupTest>();
                
#if !UNITY_EDITOR
       if(!string.IsNullOrEmpty(setKeyTrue))
                    PlayerPrefs.SetInt(setKeyTrue,1);
#endif
                Debug.Log("DoubleClick Detected! Count:" + xs.Count);
            });
    }
}