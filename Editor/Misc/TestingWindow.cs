using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.EditorPrefs;

public class EditorPrefInt
{

    public int Value
    {
        get => GetInt(_name);
        set
        {
            if (value != Value)
            {
                SetInt(_name, value);
            }
        }
    }

    public bool BoolValue
    {
        get => GetInt(_name) != 0;
        set
        {
            if (value == (Value != 0)) return;
            SetInt(_name, value ? 1 : 0);
        }
    }

    private readonly string _name;

    public EditorPrefInt(string name)
    {
        _name = name;
    }
    
    public EditorPrefInt(PlayerPrefEnum name)
    {
        _name = "extra_"+name;
    }

    public bool HasKey() => EditorPrefs.HasKey(_name);
    
    public EditorPrefInt(string name,int defaultValue)
    {
        if(!EditorPrefs.HasKey(_name))
            SetInt(name, defaultValue);
        _name = name;
    }
}

public class TestingWindow : EditorWindow
{
    [MenuItem("Tools/DuongPham/" + nameof(TestingWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(TestingWindow));
    }

    private int Level
    {
        get => GetInt("Level");
        set => SetInt("Level", value);
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


    private string DirtyScenePath
    {
        get => GetString("DirtyScenePath");
        set => SetString("DirtyScenePath", value);
    }

    private string PlayerPrefKey
    {
        get => GetString("PlayerPrefKey");
        set => SetString("PlayerPrefKey", value);
    }

    public NotifyEnum NotifyEnum;
    public GameActionEvent GameActionEvent;

    Vector2 scrollPosition;
    private Vector3Int date = new Vector3Int(1, 1, 2024);

    private ProjectSettings _projectSettings;

    public class EnumIntPlayerPref
    {
        private IList<int> values;
        private IList<GUIContent> names;

        public int Value
        {
            get => GetInt(type + "editor", 0);
            set => SetInt(type + "editor", value);
        }

        public IList<int> EnumValues => values;
        public IList<GUIContent> EnumNames => names;
        private BuiltInEnumType type;

        public EnumIntPlayerPref(BuiltInEnumType builtInEnumType)
        {
            type = builtInEnumType;
            EnumList enumList = new EnumList(builtInEnumType.ToString());

            int _index = -1;
            if (enumList.Found && enumList.Values.Length > 0)
            {
                _index = -1;
                for (var i = 0; i < enumList.Values.Length; i++)
                    if (Value == enumList.Values[i])
                    {
                        _index = i;
                        break;
                    }

                values = enumList.Values;
                names = enumList.Names;
                if (_index == -1)
                {
                    _index = values.Count;
                    values = values.ArrayAdd(Value);
                    names = names.ArrayAdd(new GUIContent($"NotFound {enumList.TypeName}_{Value}"));
                }
            }
            else
            {
                values = new[] {Value};
                names = new[] {new GUIContent($"NotFound {enumList.TypeName}_{Value}")};
            }
        }
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

    private string CustomMethod
    {
        get => GetString("CustomMethod");
        set => SetString("CustomMethod", value);
    }
    
    private readonly EnumIntPlayerPref[] _enumInt = new EnumIntPlayerPref[10];
    private int _test123;

    private EditorPrefInt _flagHide = new EditorPrefInt("flagHide");

    [Flags]
    private enum HideFlag
    {
        Focus = 1,
        Log = 2,
        NameSpace = 4,
        Property = 8,
        ActionNEvent = 16,
        Data = 32
    }

    private bool IsEnable(HideFlag flag)
    {
        return ((int) flag & _flagHide.Value) == 0;
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Delete All Data", new GUIStyle(GUI.skin.button)
        {
//            normal = new GUIStyleState() {textColor = Color.red}
        }))
        {
            //UIDebugLog.Log(ReactivePlayerData.FilePath);
            DeleteAll();
        }

        StartHorizon(() =>
        {
            if (GUILayout.Button("Start Scene"))
            {
                EditorSceneManager.OpenScene(
                    $"Assets/{_projectSettings.projectSku}/Scenes/{_projectSettings.startSceneName}.unity");
            }

            if (GUILayout.Button("Level Editor"))
            {
                EditorSceneManager.OpenScene(
                    $"Assets/{_projectSettings.projectSku}/Scenes/{_projectSettings.levelEditorSceneName}.unity");
            }

            if (GUILayout.Button("Test"))
            {
                EditorSceneManager.OpenScene(
                    $"Assets/{_projectSettings.projectSku}/Scenes/Test.unity");
            }
        });
        
        var style2 = new GUIStyle(GUI.skin.button);
        style2.normal.textColor = Color.green;
        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = new Color(0.2f, 0.2f, 0.2f);
        StartHorizon(() =>
        {
            foreach (var value in (int[]) Enum.GetValues(typeof(HideFlag)))
            {
                if (GUILayout.Button(((HideFlag) value).ToString(), (value & _flagHide.Value) != 0 ? style : style2))
                {
                    if ((value & _flagHide.Value) != 0)
                    {
                        _flagHide.Value &= ~value;
                    }
                    else
                    {
                        _flagHide.Value |= value;
                    }

                    Repaint();
                }
            }
        });

        StartHorizon(() =>
        {
            Level = EditorGUILayout.IntField("Level", Level);
            if (GUILayout.Button("Level"))
            {
                RootView.rootView.gameController.StartLevel(Level);
                RootView.rootView.gameController.CalculateUpdateQuest(-1);
            }

            if (GUILayout.Button("Test"))
            {
                int count = 0;
                for (int i = 0; i < Level; i++)
                {
                    for (int j = i + 1; j < Level; j++)
                    {
                        count++;
                    }
                }

                Debug.Log(count);
            }
        });


        if (_projectSettings == null || _projectSettings.IsNotCurrent)
            _projectSettings = ProjectSettings.LoadProjectSettings();

        if (IsEnable(HideFlag.NameSpace))
            StartHorizon(() =>
            {
                _projectSettings.projectSku = EditorGUILayout.TextField(_projectSettings.projectSku);
                _projectSettings.startSceneName = EditorGUILayout.TextField(_projectSettings.startSceneName);
                _projectSettings.levelEditorSceneName =
                    EditorGUILayout.TextField(_projectSettings.levelEditorSceneName);
                //            if (GUILayout.Button("Save"))
                //            {
                //                _projectSettings.SaveProjectSettings();
                //            }
                //            if (GUILayout.Button("Reload"))
                //            {
                //                _projectSettings = ProjectSettings.LoadProjectSettings();
                //                Repaint();
                //            }
            });
        EditorGUIUtility.labelWidth = 100;


        if (IsEnable(HideFlag.NameSpace))
            StartHorizon(() =>
            {
                _projectSettings.keystorePassword =
                    EditorGUILayout.TextField("Password", _projectSettings.keystorePassword);
                _projectSettings.nameSpace = EditorGUILayout.TextField("NameSpace", _projectSettings.nameSpace);
                if (GUILayout.Button("Save"))
                {
                    _projectSettings.SaveProjectSettings();
                }

                if (GUILayout.Button("Reload"))
                {
                    _projectSettings = ProjectSettings.LoadProjectSettings();
                    Repaint();
                }
            });

        if (IsEnable(HideFlag.Focus))
            StartHorizon(() =>
            {
                _projectSettings.folder1 =
                    EditorGUILayout.TextField("Prefab folder1", _projectSettings.folder1);

                if (GUILayout.Button("Focus"))
                {
                    UnityEngine.Object obj =
                        AssetDatabase.LoadAssetAtPath(_projectSettings.folder1, typeof(UnityEngine.Object));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            });

        if (IsEnable(HideFlag.Focus))
            StartHorizon(() =>
            {
                _projectSettings.folder2 =
                    EditorGUILayout.TextField("Prefab folder2", _projectSettings.folder2);

                if (GUILayout.Button("Focus"))
                {
                    UnityEngine.Object obj =
                        AssetDatabase.LoadAssetAtPath(_projectSettings.folder2, typeof(UnityEngine.Object));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            });

      
        if (IsEnable(HideFlag.Focus))
            StartHorizon(() =>
        {
            DirtyScenePath = EditorGUILayout.TextField("DirtyScenePath", DirtyScenePath);
            if (GUILayout.Button("Dirty Load Scene"))
            {
                EditorSceneManager.OpenScene(DirtyScenePath);
            }
        });

        if (IsEnable(HideFlag.Property))
            StartHorizon(() =>
            {
                PlayerPrefKey = EditorGUILayout.TextField("PlayerPrefKey", PlayerPrefKey);
                if (GUILayout.Button("Log PlayerPrefKey"))
                {
                    Debug.Log(PlayerPrefs.GetString(PlayerPrefKey));
                }
            });


        var oldColor = GUI.backgroundColor;
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (IsEnable(HideFlag.Property))
        {
            //        TestingSettings.TurnOffDrawProduct = EditorGUILayout.Toggle("TurnOffDrawProduct", TestingSettings.TurnOffDrawProduct);
            TestingSettings.UiLogEnable = EditorGUILayout.Toggle("UILogEnable", TestingSettings.UiLogEnable);
            TestingSettings.AlwaysNewDay = EditorGUILayout.Toggle("AlwaysNewDay", TestingSettings.AlwaysNewDay);
        }


        if (IsEnable(HideFlag.Focus))
            StartHorizon(() =>
            {
                if (GUILayout.Button(nameof(Kernel)))
                {
                    UnityEngine.Object obj = (Kernel) FindObjectOfType(typeof(Kernel));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }

                if (GUILayout.Button(nameof(BaseGameController)))
                {
                    UnityEngine.Object obj = (BaseGameController) FindObjectOfType(typeof(BaseGameController));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }

                if (GUILayout.Button(nameof(PoolManager)))
                {
                    UnityEngine.Object obj = (PoolManager) FindObjectOfType(typeof(PoolManager));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            });


        if (IsEnable(HideFlag.Focus))
            StartHorizon(() =>
            {
                if (GUILayout.Button(nameof(ScreenRoot)))
                {
                    UnityEngine.Object obj = (ScreenRoot) FindObjectOfType(typeof(ScreenRoot));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }

                if (GUILayout.Button(nameof(PlayScreen)))
                {
                    UnityEngine.Object obj = (PlayScreen) FindObjectOfType(typeof(PlayScreen));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }

                if (GUILayout.Button(nameof(PopupManager)))
                {
                    UnityEngine.Object obj = (PopupManager) FindObjectOfType(typeof(PopupManager));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            });

        if (IsEnable(HideFlag.Focus))
            StartHorizon(() =>
            {
                Kernel gc = (Kernel) FindObjectOfType(typeof(Kernel));
                var names = new[] {nameof(ShopDatabase), nameof(GameSetting)};
                for (var i = 0; i < names.Length; i++)
                {
                    if (GUILayout.Button(names[i]))
                        foreach (var database in gc.DatabasesForEditorWindow)
                            if (database.GetType().Name == names[i])
                            {
                                UnityEngine.Object obj = database;
                                Selection.activeObject = database;
                                EditorGUIUtility.PingObject(database);
                                Debug.Log(obj, obj);
                            }
                }
            });


        EditorGUIUtility.labelWidth = 50;


        if (IsEnable(HideFlag.Property))
            StartHorizon(() =>
            {
                ProperyName = EditorGUILayout.TextField("Property", ProperyName);
                TestParameter1 = EditorGUILayout.IntField("Value", TestParameter1);
                ReactiveDataEnum = (ReactiveDataEnum) EditorGUILayout.EnumPopup(ReactiveDataEnum);
                if (GUILayout.Button("Set"))
                {
                    BaseFrameworkView.GetPropertyByName(ProperyName, ReactiveDataEnum).Value = TestParameter1;
                }
            });
        if (IsEnable(HideFlag.Property))
            StartHorizon(() =>
            {
                ProperyName2 = EditorGUILayout.TextField("Property", ProperyName2);
                TestParameter2 = EditorGUILayout.IntField("Value", TestParameter2);
                ReactiveDataEnum2 = (ReactiveDataEnum) EditorGUILayout.EnumPopup(ReactiveDataEnum2);
                if (GUILayout.Button("Set"))
                {
                    BaseFrameworkView.GetPropertyByName(ProperyName2, ReactiveDataEnum2).Value = TestParameter2;
                }
            });

        if (IsEnable(HideFlag.Property))
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

        if (IsEnable(HideFlag.Property))
            StartHorizon(() =>
            {
                CustomPlayerPrefKey2 = EditorGUILayout.TextField("Key", CustomPlayerPrefKey2);
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

        if (IsEnable(HideFlag.Log))
            StartHorizon(() =>
            {
                date = EditorGUILayout.Vector3IntField("Date", date);

                if (GUILayout.Button("Log"))
                {
                    Debug.LogError(new DateTime(date.z, date.y, date.x));
                    Debug.LogError(SdkExtenstions.GetEpochDate(date));
                }
            });

        if (IsEnable(HideFlag.ActionNEvent))
            StartHorizon(() =>
            {
                CustomMethod = EditorGUILayout.TextField(CustomMethod);
                if (GUILayout.Button("Invoke"))
                {
                    var type = RootView.rootView.gameController.GetType();
                    var method =
                        type.GetMethod(
                            CustomMethod,
                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                            Type.DefaultBinder,
                            new Type[] { },
                            null);
                    
                    method.Invoke(RootView.rootView.gameController, null);
                }
            });

        
  
        if (IsEnable(HideFlag.ActionNEvent))
            StartHorizon(() =>
            {
                GameActionEvent =
                    (GameActionEvent) EditorGUILayout.EnumPopup(this.GameActionEvent);
                if (GUILayout.Button("GameActionEvent"))
                {
                    SubjectController.GameActionEvent.OnNext(GameActionEvent);
                }
            });

        if (IsEnable(HideFlag.ActionNEvent))
            StartHorizon(() =>
            {
                int i = 0;
                var type = BuiltInEnumType.GlobalCallBackEnum;
                if (_enumInt[i] == null)
                    _enumInt[i] = new EnumIntPlayerPref(type);
                _enumInt[i].Value = EditorGUILayout.IntPopup(_enumInt[i].Value, _enumInt[i].EnumNames.ToArray(),
                    _enumInt[i].EnumValues.ToArray());
                if (GUILayout.Button(type.ToString()))
                {
                    SubjectController.OnPointParameterCallBack.OnNext(
                        new PointParameter(BuiltInEnumType.GlobalCallBackEnum, _enumInt[i].Value));
                }
            });
        if (IsEnable(HideFlag.ActionNEvent))
            StartHorizon(() =>
            {
                int i = 1;
                var type = BuiltInEnumType.ProductGameAction;
                if (_enumInt[i] == null)
                    _enumInt[i] = new EnumIntPlayerPref(type);
                _enumInt[i].Value = EditorGUILayout.IntPopup(_enumInt[i].Value, _enumInt[i].EnumNames.ToArray(),
                    _enumInt[i].EnumValues.ToArray());
                if (GUILayout.Button(type.ToString()))
                {
                    SubjectController.ProductGameActionEvent.OnNext(new ProductAction()
                    {
                        index = _enumInt[i].Value,
                        amount = 1
                    });
                }
            });

        if (IsEnable(HideFlag.Property))
            StartHorizon(() =>
            {
                int i = 2;
                var type = BuiltInEnumType.RemoteConfigKey;
                if (_enumInt[i] == null)
                    _enumInt[i] = new EnumIntPlayerPref(type);
                _enumInt[i].Value = EditorGUILayout.IntPopup(_enumInt[i].Value, _enumInt[i].EnumNames.ToArray(),
                    _enumInt[i].EnumValues.ToArray());
                var key = ((RemoteConfigKey) _enumInt[i].Value);

                if (GUILayout.Button(type.ToString()))
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

        if (IsEnable(HideFlag.ActionNEvent))
            StartHorizon(() =>
            {
                NotifyEnum = (NotifyEnum) EditorGUILayout.EnumPopup(this.NotifyEnum);
                if (GUILayout.Button("NotifyEnum"))
                {
                    SubjectController.NotifyEvent.OnNext(NotifyEnum);
                }
            });


        GUI.backgroundColor = oldColor;
        // ___________________________________
        GUILayout.Space(2);

        if (IsEnable(HideFlag.Log))
            StartHorizon(() =>
            {
                _test123 = EditorGUILayout.IntField("test123", _test123);
                if (GUILayout.Button("Math"))
                {
                    // -1 => 3
                    // -2 => 2
                    // voi 4
                    // Mathf
                    Debug.Log((4 - Mathf.Abs(_test123 % 4)) % 4);
                    //    Debug.Log(-1%3);
                    //     Debug.Log(-2%3);
                    for (int i = -10; i < 20; i++)
                    {
                        Debug.Log(i + "/" + i.Repeat(4) + "/" + i.PingPong(4));
                    }
                }
            });

//
//

        if (IsEnable(HideFlag.Log))
            if (GUILayout.Button("Test Enum in Namespace"))
            {
//            MergeBlock.EffectEnum.BombEffect
                Debug.Log(DuongExtensions.GetType("MergeBlock", "EffectEnum"));
//            Type type = Type.GetType("MergeBlock.EffectEnum");
//            Debug.Log(type.Name);
            }

        if (IsEnable(HideFlag.Data))
            if (GUILayout.Button("Reset Unlimited life"))
            {
                var now = DateTime.Now;
                int key = (int) BaseCustomTimeTick.CountDownUnlimitedLives;
                BaseFrameworkView.PlayerData.customTimeTick[key] = now.Ticks;
                BaseFrameworkView.PlayerData.CustomDateTime[key] =
                    DateTime.FromBinary(BaseFrameworkView.PlayerData.customTimeTick[key]);
            }

        if (IsEnable(HideFlag.ActionNEvent))
            StartHorizon(() =>
            {
                if (GUILayout.Button("gameController.SaveTemp()"))
                {
                    RootView.rootView.gameController.SaveTemp();
//                _gameSave = RootView.rootView.gameController.CurrentGameSave.Clone();
//                PlayerPrefs.SetString("game_save_temp", JsonUtility.ToJson(_gameSave));
                }

                if (GUILayout.Button("gameController.LoadTemp()"))
                {
                    RootView.rootView.gameController.LoadTemp();

//                string methodName = nameof(JsonUtility.FromJson);
//                var gameSaveType = DuongExtensions.GetType(_projectSettings.nameSpace, BaseGameSave.GameClassName);
//                var method = typeof(JsonUtility).GetMethod(
//                    methodName, new[] {typeof(string)});
//                if (method != null)
//                {
//                    MethodInfo genericMethod = method.MakeGenericMethod(gameSaveType);
//                    _gameSave =
//                        genericMethod.Invoke(null, new object[] {PlayerPrefs.GetString("game_save_temp")}) as IGameSave;
//                    RootView.rootView.gameController.OverrideGameSave(_gameSave.Clone());
//                    RootView.rootView.gameController.StartCurrentGameSave();
//                }
                }
            });


        EditorGUILayout.EndScrollView();
    }

    public static void DeleteAll()
    {
        DuongExtensions.DeleteData();
    }

    void StartHorizon(Action action)
    {
        //*
        EditorGUILayout.BeginHorizontal();
        action.Invoke();
        EditorGUILayout.EndHorizontal();
        //*
    }

    private static string FilePath => string.Format("{0}/{1}", Application.persistentDataPath, "PlayerData.json");
}