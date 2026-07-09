using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PopupUpdateProfile : BasePopup
{
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private Button btnUpdate;
    [SerializeField] private IndexPropertyChangeScriptBase[] avatarSelects;
    [SerializeField] private int minLengthName = 2;
    [SerializeField] private int maxLengthName = 20;

    private Action<string> _updateActionName;
    private Action<int> _updateActionAvatar;
    
    private readonly IntReactiveProperty _avt = new IntReactiveProperty();

    public PopupUpdateProfile ShowPopup(string message,int avatar, Action<string> nameUpdate,Action<int> avatarUpdate)
    {
        Show();
        CheckRegister();
        _avt.Value = avatar;
        _updateActionName = nameUpdate;
        _updateActionAvatar = avatarUpdate;
        inputName.text = message;
        AdsManager.AppLeaving = 20;

        SubjectController.GlobalError.Where(x => (BaseGlobalError) x == BaseGlobalError.PlayFabNameTaken)
            .TakeUntilDisable(this)
            .Subscribe(data =>
            {
                if(!ScreenRoot.dialogController.AnyDialogShow && ScreenRoot.popupManager.GetCurrentPopup() == this)
                    ScreenRoot.dialogController.ShowMessage($"Name not available");
            });
        return this;
    }

    public static string RemoveSpecialCharacters(string str)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            if ((str[i] >= '0' && str[i] <= '9')
                || (str[i] >= 'A' && str[i] <= 'z'
                    || (str[i] == '.' || str[i] == '_')))
            {
                sb.Append(str[i]);
            }
        }

        return sb.ToString();
    }
    protected override void Register()
    {
        base.Register();
        for (var i = 0; i < avatarSelects.Length; i++)
        {
            var @select = avatarSelects[i];
            select.SetProperty(_avt);
            select.SetValue(i);
        }

        inputName.onValueChanged.AddListener(data => { inputName.text = RemoveSpecialCharacters(data); });

        btnUpdate.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(inputName.text) && inputName.text.Length >= minLengthName &&
                inputName.text.Length <= maxLengthName)
            {
                _updateActionName.Invoke(inputName.text);
                if (inputName.text != string.Empty)
                    _updateActionAvatar.Invoke(_avt.Value);
                ScreenRoot.popupManager.ShowPopup<PopupProfile>();
            }
            else
            {
                if(string.IsNullOrEmpty(inputName.text) )
                    ScreenRoot.dialogController.ShowMessage("Cannot input empty name");
                else  if(inputName.text.Length < minLengthName)
                    ScreenRoot.dialogController.ShowMessage($"Name must be more than {minLengthName} character");
                else if(inputName.text.Length > maxLengthName)
                    ScreenRoot.dialogController.ShowMessage($"Name must be less than {maxLengthName} character");
            }
          
        });
    }
}
