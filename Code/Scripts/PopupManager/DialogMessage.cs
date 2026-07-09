using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogMessage : BaseDialog {

    public TextMeshProUGUI txtMessage;
    public ButtonWithText btnMessageOk;
    public Button btnMessageYes;
    public Button btnMessageNo;
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private int sound = 1;
    private void BeforeShow(string message)
    {
        base.Show();
        txtMessage.text = message;
        btnMessageNo.gameObject.SetActive(false);
        btnMessageOk.gameObject.SetActive(false);
        btnMessageYes.gameObject.SetActive(false);
    }

    public void Show(string message, Action onClickOk,string buttonText = "OK")
    {
        BeforeShow(message);
        btnMessageOk.gameObject.SetActive(true);
        btnMessageOk.button.onClick.RemoveAllListeners();
        btnMessageOk.button.onClick.AddListener(() =>
        {
            parent.CloseAll();
            onClickOk();
            sound.PlaySound();
        });
        if( btnMessageOk.text != null)
            btnMessageOk.text.text = buttonText;
    }

    public void Show(string message, Action onClickYes, Action onClickNo)
    {
        BeforeShow(message);
        btnMessageYes.gameObject.SetActive(true);
        btnMessageNo.gameObject.SetActive(true);
        btnMessageYes.onClick.RemoveAllListeners();
        btnMessageNo.onClick.RemoveAllListeners();
        btnMessageYes.onClick.AddListener(() =>
        {
            parent.CloseAll();
            onClickYes();
            sound.PlaySound();
        });
        btnMessageNo.onClick.AddListener(() =>
        {
            parent.CloseAll();
            onClickNo();
            sound.PlaySound();
        });
    }
}
