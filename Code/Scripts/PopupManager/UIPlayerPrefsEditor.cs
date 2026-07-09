using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerPrefsEditor : MonoBehaviour
{

    [SerializeField] private TMP_InputField key;
    [SerializeField] private TMP_InputField value;
    [SerializeField] private Button load;
    [SerializeField] private Button save;

    private void Start()
    {
        key.text = "EpochDayAdd";
        load.onClick.AddListener(() => { value.text = PlayerPrefs.GetInt(key.text).ToString(); });
        save.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt(key.text,int.Parse(value.text));
        });
    }
}
