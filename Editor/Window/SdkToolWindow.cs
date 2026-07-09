using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorPrefs;
// ReSharper disable Unity.InefficientPropertyAccess

public class SdkToolWindow : BaseManagerWindow
{
    [MenuItem("SonatFramework/" + nameof(SdkToolWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(SdkToolWindow));
    }

    private EditorPrefInt _flagHide = new EditorPrefInt(nameof(SdkToolWindow) + "Selection");

    protected override void Prepare()
    {
        base.Prepare();
        if (EnumValues == null)
            EnumValues = (int[]) Enum.GetValues(typeof(HideFlag));
    }

    protected override void OnGUIHidable()
    {
        
        StartHorizon(() =>
        {
            if(EnumValues != null)
                for (var index = 0; index < EnumValues.Length; index++)
                {
                    var value = EnumValues[index];
                    GUI.enabled = (value != _flagHide.Value);
                    if (GUILayout.Button(((HideFlag) value).ToString(), value == _flagHide.Value ? Style2 : Style))
                    {
                        _flagHide.Value = value;
                        Repaint();
                    }

                    GUI.enabled = true;
                }
        });

        EditorGUILayout.Space(5);
    }

    public override void OnGUI()
    {
        base.OnGUI();

   

        switch (GetCurrent())
        {
            case HideFlag.SonatSdk:
                OnSonatSdk();
                break;
            case HideFlag.ReactivePlayerData:
                OnPlayerData();
                break;
            case HideFlag.ReactiveDataMonitor:
                OnDataMonitor();
                break;
            case HideFlag.TrackingMonitor:
                OnTrackingMonitor();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnTrackingMonitor()
    {
        StartHorizon(() =>
        {
            if (GUILayout.Button("Clear"))
            {
                Sonat.BaseSonatAnalyticLog.ClearEditorLog();
                Repaint();
            }
        });

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUI.enabled = false;
        EditorGUILayout.TextArea(
            string.Join("\n", Sonat.BaseSonatAnalyticLog.eventsForEdiorWindow.ToArray()),
            GUILayout.ExpandHeight(true));
        GUI.enabled = true;
        EditorGUILayout.EndScrollView();
    }
    

    private readonly int[] _session = new int[20];
    private readonly int[] _shared = new int[20];
    Vector2 scrollPosition;

    public void OnDataMonitor()
    {
        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.green;
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.LabelField("SessionData",style);
        EnumList enumList = new EnumList(BuiltInEnumType.CustomSessionDataProperty.ToString());
        if (enumList.Found && enumList.Values.Length > 0)
        {
            IList<int> values = enumList.Values.ToList();
            IList<GUIContent> names = enumList.Names.ToList();
            for (var i = 0; i < names.Count; i++)
            {
             
                if (BaseFrameworkView.SessionData != null)
                {
                    var i1 = i;
                    StartHorizon(() =>
                    {
                        EditorGUILayout.IntField(names[i1], BaseFrameworkView.SessionData.customPropertyList[values[i1]].Value);
                        _session[i1] = EditorGUILayout.IntField(_session[i1]);
                        if (GUILayout.Button("Set"))
                        {
                            BaseFrameworkView.SessionData.customPropertyList[values[i1]].Value = _session[i1];
                            Repaint();
                        }
                    });
                }
            }
        }
        
        EditorGUILayout.LabelField("SharedData",style);
         enumList = new EnumList(BuiltInEnumType.FrameworkSharedData.ToString());
        if (enumList.Found && enumList.Values.Length > 0)
        {
            IList<int> values = enumList.Values.ToList();
            IList<GUIContent> names = enumList.Names.ToList();
            for (var i = 0; i < names.Count; i++)
            {
                if (BaseFrameworkView.SharedData != null)
                {
                    var i1 = i;
                    StartHorizon(() =>
                    {
                        EditorGUILayout.IntField(names[i1], BaseFrameworkView.SharedData[values[i1]]);
                        if (i1 == 1)
                        {
                            _shared[i1] = (int)((Quantity)EditorGUILayout.EnumPopup((Quantity)_shared[i1]));
                        }
                        else
                        {
                            _shared[i1] = EditorGUILayout.IntField(_shared[i1]);
                        }
                        if (GUILayout.Button("Set"))
                        {
                            BaseFrameworkView.SharedData[values[i1]] = _shared[i1];
                            Repaint();
                        }
                    });
                }
            }
        }
        
        EditorGUILayout.EndScrollView();
    }



    private void OnSonatSdk()
    {
        TestingSettings.UiLogEnable =
            EditorGUILayout.Toggle(nameof(TestingSettings.UiLogEnable), TestingSettings.UiLogEnable);
        TestingSettings.TurnOffBanner =
            EditorGUILayout.Toggle(nameof(TestingSettings.TurnOffBanner), TestingSettings.TurnOffBanner);
        TestingSettings.TurnOffOnFocusAds = EditorGUILayout.Toggle(nameof(TestingSettings.TurnOffOnFocusAds),
            TestingSettings.TurnOffOnFocusAds);
        TestingSettings.SkipRewarded =
            EditorGUILayout.Toggle(nameof(TestingSettings.SkipRewarded), TestingSettings.SkipRewarded);
        TestingSettings.CopyPaste =
            EditorGUILayout.Toggle(nameof(TestingSettings.CopyPaste), TestingSettings.CopyPaste);
        TestingSettings.InternetConnected = EditorGUILayout.Toggle(nameof(TestingSettings.InternetConnected),
            TestingSettings.InternetConnected);
        TestingSettings.EpochDayAdd =
            EditorGUILayout.IntField(nameof(TestingSettings.EpochDayAdd), TestingSettings.EpochDayAdd);
        TestingSettings.DebugLevel =
            EditorGUILayout.IntField(nameof(TestingSettings.DebugLevel), TestingSettings.DebugLevel);
        TestingSettings.DrawPointMap =
            EditorGUILayout.Toggle(nameof(TestingSettings.DrawPointMap), TestingSettings.DrawPointMap);
        TestingSettings.DebugBreak =
            EditorGUILayout.Toggle(nameof(TestingSettings.DebugBreak), TestingSettings.DebugBreak);
        TestingSettings.DebugBreak1 =
            EditorGUILayout.Toggle(nameof(TestingSettings.DebugBreak1), TestingSettings.DebugBreak1);
    }

    #region property

    private string ProperyName
    {
        get => GetString("ProperyName");
        set => SetString("ProperyName", value);
    }

    private string ProperyName2
    {
        get => GetString("ProperyName2");
        set => SetString("ProperyName2", value);
    }
    private int TestParameter1
    {
        get => GetInt("TestParameter1");
        set => SetInt("TestParameter1", value);
    }


    private int TestParameter2
    {
        get => GetInt("TestParameter2");
        set => SetInt("TestParameter2", value);
    }

    private ReactiveDataEnum ReactiveDataEnum
    {
        get => (ReactiveDataEnum) GetInt("ReactiveDataEnum");
        set => SetInt("ReactiveDataEnum", (int) value);
    }

    private ReactiveDataEnum ReactiveDataEnum2
    {
        get => (ReactiveDataEnum) GetInt("ReactiveDataEnum2");
        set => SetInt("ReactiveDataEnum2", (int) value);
    }
    private int CustomPlayerData
    {
        get => GetInt("CustomPlayerData");
        set => SetInt("CustomPlayerData", value);
    }

    private int CustomPlayerDataValue
    {
        get => GetInt("CustomPlayerDataValue");
        set => SetInt("CustomPlayerDataValue", value);
    }
    
    private string CustomPlayerPrefKey;
//    {
//        get => GetString(CustomPlayerPrefKey2);
//        set => SetString(CustomPlayerPrefKey2, value);
//    }

    private string CustomPlayerPrefKey2
    {
        get => GetString("PlayerPrefKey");
        set => SetString("PlayerPrefKey", value);
    }

    #endregion

    private void OnPlayerData()
    {
        EditorGUIUtility.labelWidth = 80;

        StartHorizon(() =>
        {
            ProperyName = EditorGUILayout.TextField("Property", ProperyName);
            TestParameter1 = EditorGUILayout.IntField("Value", TestParameter1);
            ReactiveDataEnum = (ReactiveDataEnum) EditorGUILayout.EnumPopup(ReactiveDataEnum);
            if (GUILayout.Button("Load"))
            {
                TestParameter1 = BaseFrameworkView.GetPropertyByName(ProperyName, ReactiveDataEnum).Value ;
            }
            if (GUILayout.Button("Set"))
            {
                BaseFrameworkView.GetPropertyByName(ProperyName, ReactiveDataEnum).Value = TestParameter1;
            }
        });
        StartHorizon(() =>
        {
            ProperyName2 = EditorGUILayout.TextField("Property", ProperyName2);
            TestParameter2 = EditorGUILayout.IntField("Value", TestParameter2);
            ReactiveDataEnum2 = (ReactiveDataEnum) EditorGUILayout.EnumPopup(ReactiveDataEnum2);
            if (GUILayout.Button("Load"))
            {
                TestParameter2 = BaseFrameworkView.GetPropertyByName(ProperyName2, ReactiveDataEnum2).Value;
            }
            if (GUILayout.Button("Set"))
            {
                BaseFrameworkView.GetPropertyByName(ProperyName2, ReactiveDataEnum2).Value = TestParameter2;
            }
        });
        EditorGUIUtility.labelWidth = 100;

        StartHorizon(() =>
        {
            
            EnumList enumList = new EnumList(BuiltInEnumType.CustomPlayerDataProperty.ToString());
            if (enumList.Found && enumList.Values.Length > 0)
            {
                IList<int> values = enumList.Values.ToList();
                IList<GUIContent> names = enumList.Names.ToList();
                CustomPlayerData = EditorGUILayout.IntPopup(CustomPlayerData, names.ToArray(),
                    values.ToArray());
            }

            CustomPlayerDataValue = EditorGUILayout.IntField("Value", CustomPlayerDataValue);
            if (GUILayout.Button("Load"))
            {
                CustomPlayerDataValue = BaseFrameworkView.PlayerData.customPropertyList[CustomPlayerData].Value;
            }

            if (GUILayout.Button("Set"))
            {
                BaseFrameworkView.PlayerData.customPropertyList[CustomPlayerData].Value = CustomPlayerDataValue;
            }
        });
        StartHorizon(() => { 
            
            CustomPlayerPrefKey2 = EditorGUILayout.TextField("PlayerPrefKey", CustomPlayerPrefKey2);
            CustomPlayerPrefKey = EditorGUILayout.TextField("Value", CustomPlayerPrefKey);
            if (GUILayout.Button("Load"))
            {
                CustomPlayerPrefKey = PlayerPrefs.GetString(CustomPlayerPrefKey2);
            }

            if (GUILayout.Button("Set"))
            {
                PlayerPrefs.SetString(CustomPlayerPrefKey2, CustomPlayerPrefKey);
            }
        });
  
        
        StartHorizon(() =>
        {
            int i = 2;
            var type = BuiltInEnumType.RemoteConfigKey;
            if (_enumInt[i] == null)
                _enumInt[i] = new EnumIntPlayerPref(type);
            _enumInt[i].Value = EditorGUILayout.IntPopup(_enumInt[i].Value, _enumInt[i].EnumNames.ToArray(),
                _enumInt[i].EnumValues.ToArray());
            var key = ((RemoteConfigKey) _enumInt[i].Value);

            if (GUILayout.Button("Log"))
            {
                try
                {
                    Debug.Log(key + "-bool:" + key.GetValueBoolean());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    Debug.Log(key + "-string:" + key.GetValueString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    Debug.Log(key + "-int:" + key.GetValueInt());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        });
        
        
        if (GUILayout.Button("Reset Unlimited life"))
        {
            var now = DateTime.Now;
            int key = (int) BaseCustomTimeTick.CountDownUnlimitedLives;
            BaseFrameworkView.PlayerData.customTimeTick[key] = now.Ticks;
            BaseFrameworkView.PlayerData.CustomDateTime[key] =
                DateTime.FromBinary(BaseFrameworkView.PlayerData.customTimeTick[key]);
        }
        EditorGUIUtility.labelWidth = 100;
        TestingSettings.AlwaysNewDay = EditorGUILayout.Toggle("AlwaysNewDay", TestingSettings.AlwaysNewDay);
        
        EditorGUILayout.Space(10);
    }

    private readonly EnumIntPlayerPref[] _enumInt = new EnumIntPlayerPref[10];


//    private BuilderRoot _builderRoot;

    private enum HideFlag
    {
        //        BuilderRoot ,
        SonatSdk,
        ReactivePlayerData,
        ReactiveDataMonitor,
        TrackingMonitor,
    }

    private bool IsEnable(HideFlag flag)
    {
        return ((int) flag & _flagHide.Value) == 0;
    }

    private HideFlag GetCurrent()
    {
        foreach (var enumValue in EnumValues)
            if (_flagHide.Value == enumValue)
                return (HideFlag) enumValue;

        return (HideFlag) 0;
    }
}