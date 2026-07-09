using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuiltInTestPopup : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Button prefab;
    [SerializeField] private TMP_InputField value1;
    [SerializeField] private TMP_InputField value2;

    private enum BuiltInTestPopupType
    {
        BuiltInFunctionTest,
        GameActionEvent,
        BuiltInFunction,
        BuiltInFunctionTestSdk,
    }

    public void Show(int t)
    {
        _button.SetActiveAll(false);
        switch ((BuiltInTestPopupType) t)
        {
            case BuiltInTestPopupType.BuiltInFunctionTest:
                Show((BuiltInTestPopupType) t, Enum.GetNames(typeof(BuiltInFunctionTest)).ToArray(),
                    (int[]) Enum.GetValues(typeof(BuiltInFunctionTest)));
                break;
            case BuiltInTestPopupType.GameActionEvent:
                Show((BuiltInTestPopupType) t, Enum.GetNames(typeof(GameActionEvent)).ToArray(),
                    (int[]) Enum.GetValues(typeof(GameActionEvent)));
                break;
            case BuiltInTestPopupType.BuiltInFunction:
                Show((BuiltInTestPopupType) t, Enum.GetNames(typeof(BuiltInFunction)).ToArray(),
                    (int[]) Enum.GetValues(typeof(BuiltInFunction)));
                break;
            case BuiltInTestPopupType.BuiltInFunctionTestSdk:
                Show((BuiltInTestPopupType) t, Enum.GetNames(typeof(BuiltInFunctionTestSdk)).ToArray(),
                    (int[]) Enum.GetValues(typeof(BuiltInFunctionTestSdk)));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(t), t, null);
        }
    }

    private Button GetAButton(int i)
    {
        if (i < _button.Count)
        {
            _button[i].gameObject.SetActive(true);
            _button[i].onClick.RemoveAllListeners();
            return _button[i];
        }

        var button = Instantiate(prefab, container);
        button.gameObject.SetActive(true);
        _button.Add(button);
        button.onClick.RemoveAllListeners();
        return button;
    }

    private List<Button> _button = new List<Button>();

    [SerializeField] private BuiltInButton builtInButton;
    [SerializeField] private BuiltInButtonTest builtInButtonTest;
    [SerializeField] private BuiltInButtonTestSdk builtInButtonTestSdk;
    private void Show(BuiltInTestPopupType t, IList<string> names, IList<int> values)
    {
        for (var i = 0; i < values.Count; i++)
        {
            var button = GetAButton(i);
            if (button.GetComponentInChildren<TMP_Text>() != null)
                button.GetComponentInChildren<TMP_Text>().text = names[i];
            if (button.GetComponentInChildren<Text>() != null)
                button.GetComponentInChildren<Text>().text = names[i];
            switch (t)
            {
                case BuiltInTestPopupType.BuiltInFunctionTest:
                    var key = (BuiltInFunctionTest) values[i];
                    button.onClick.AddListener(() =>
                    {
                        builtInButtonTest.Handler(key,int.Parse(value1.text), int.Parse(value2.text));
                        gameObject.SetActive(false);
                    });
                    break;
                case BuiltInTestPopupType.GameActionEvent:
                    var key2 = (GameActionEvent) values[i];
                    button.onClick.AddListener(() =>
                    {
                        SubjectController.GameActionEvent.OnNext(key2);
                        gameObject.SetActive(false);
                    });
                  
                    break;
                case BuiltInTestPopupType.BuiltInFunction:
                    var key3 = (BuiltInFunction) values[i];
                    button.onClick.AddListener(() =>
                    {
                        builtInButton.Handler1(key3);
                        gameObject.SetActive(false);
                    });
                    break;
                case BuiltInTestPopupType.BuiltInFunctionTestSdk:
                    var key4 = (BuiltInFunctionTestSdk) values[i];
                    button.onClick.AddListener(() =>
                    {
                        builtInButtonTestSdk.Handler(key4);
                        gameObject.SetActive(false);
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }
    }
}