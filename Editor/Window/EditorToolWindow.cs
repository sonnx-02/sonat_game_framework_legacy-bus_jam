using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable Unity.InefficientPropertyAccess

public class EditorToolWindow : BaseManagerWindow
{
    [MenuItem("SonatFramework/" + nameof(EditorToolWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(EditorToolWindow));
    }

    private EditorPrefInt _flagHide = new EditorPrefInt(nameof(EditorToolWindow) + "Selection");


    protected override void OnGUIHidable()
    {
        StartHorizon(() =>
        {
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
//            StartHorizon(() =>
//            {
//                if (GUILayout.Button("Focus"))
//                {
//                }
//            });
//            EditorGUILayout.Space(10);

      

        switch (GetCurrent())
        {
            case HideFlag.ProjectSetting:
                ProjectSetting();
                break;
            case HideFlag.Math:
                OnMath();
                break;
            case HideFlag.Focus:
                OnFocus1();
                break;
            case HideFlag.CanvasTool:
                OnCanvasTool();
                break;
            case HideFlag.EditorTool:
                OnEditorTool();
                break;
            case HideFlag.InspectorTool:
                OnInspectorTool();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public  BaseBaseActionScript[] actions = new BaseBaseActionScript[0];
    public  BaseRxAnimator[] anims = new BaseRxAnimator[0];
    public  BaseFrameworkView[] views = new BaseFrameworkView[0];
    private void OnInspectorTool()
    {
        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.green;
        if (GUILayout.Button("Load",style))
        {
            if (Selection.gameObjects.Length > 0)
            {
                actions = Selection.gameObjects[0].GetComponents<BaseBaseActionScript>();
                anims = Selection.gameObjects[0].GetComponents<BaseRxAnimator>();
                views = Selection.gameObjects[0].GetComponents<BaseFrameworkView>();
            }
            Repaint();
        }
        
        SerializedObject serialisedObject = new SerializedObject(this);
        SerializedProperty myIngredientProp = serialisedObject.FindProperty("actions");
        EditorGUILayout.PropertyField(myIngredientProp, true);
        SerializedProperty myIngredientProp2 = serialisedObject.FindProperty("anims");
        EditorGUILayout.PropertyField(myIngredientProp2, true);
        SerializedProperty myIngredientProp3 = serialisedObject.FindProperty("views");
        EditorGUILayout.PropertyField(myIngredientProp3, true);
    }
    
    private static string Path
    {
        get => EditorPrefs.GetString("TempPath");
        set => EditorPrefs.SetString("TempPath", value);
    }

    private void OnEditorTool()
    {
        StartHorizon(() =>
        {
            if (GUILayout.Button("Find All Button With No Sound"))
            {
                var btns = FindObjectsOfType<Button>(true)
                    .Where(button =>
                        button.GetComponent<BaseButton>() == null &&
                        button.GetComponent<ButtonClickSoundScript>() == null &&
                        button.GetComponent<BaseAdsRewardButton>() == null).Select(x => x.gameObject)
                    .OrderBy(x => x.transform.ParentCount()).ToArray();
                foreach (var button in btns)
                {
                    Debug.LogError("no sound " + button, button);
                }

                Selection.objects = btns;
            }

            if (GUILayout.Button("Find All Button Dupplicate Sound"))
            {
                var btns = FindObjectsOfType<Button>(true)
                    .Where(x => (x.GetComponent<BaseButton>() != null &&
                                 x.GetComponent<ButtonClickSoundScript>() != null))
                    .Select(x => x.gameObject)
                    .OrderBy(x => x.transform.ParentCount()).ToArray();


                foreach (var button in btns)
                {
                    Debug.LogError("duplicate sound" + button, button);
                }

                Selection.objects = btns;
            }
        });

        
        StartHorizon(() =>
        {
            find = EditorGUILayout.IntField("x", find);
            if (GUILayout.Button("Find Rx use Sound= x"))
            {
                var btns = FindObjectsOfType<BaseRxAnimator>(true)
                    .Where(x => x.GetSetting().animInSound == find || x.GetSetting().animOutSound == find)
                    .Select(x => x.gameObject)
//                .OrderBy(x => x.transform.ParentCount())
                    .ToArray();


                foreach (var button in btns)
                {
                    Debug.LogError("duplicate sound" + button, button);
                }

                Selection.objects = btns;
            }
        });

        StartHorizon(() =>
        {
            _renamePrefix = EditorGUILayout.TextField("name", _renamePrefix);
            if (GUILayout.Button("Rename list by prefix"))
            {
                var x = Selection.gameObjects.OrderBy(a => a.transform.GetSiblingIndex()).ToArray();
                for (var i = 0; i < x.Length; i++)
                    x[i].name = _renamePrefix + " " + i;
            }
        });
        
        StartHorizon(() =>
        {
            if (GUILayout.Button("Set TempPath"))
            {
                foreach (var o in Selection.objects)
                {
                    Debug.Log(AssetDatabase.GetAssetPath(o));
                    Path = AssetDatabase.GetAssetPath(o);
                    break;
                }
            }
            if (GUILayout.Button("Focus TempPath"))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(Path, typeof(UnityEngine.Object));
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }
            if (GUILayout.Button("MoveTo TempPath"))
            {
                foreach (var o in Selection.objects)
                {
                    var des = $"{Path}/{System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(o))}";
                    if (!File.Exists(des))
                    {
                        if(!AssetDatabase.GetAssetPath(o).Contains("ignore_"))
                            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(o), des);
                    }
                }

                AssetDatabase.Refresh();
            } 
            if (GUILayout.Button("CopyTo TempPath"))
            {
                foreach (var o in Selection.objects)
                {
                    var des = $"{Path}/{System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(o))}";
                    if (!File.Exists(des))
                    {
                        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(o), des);
                    }
                }

                AssetDatabase.Refresh();
            }
        });
        
           if (GUILayout.Button("Rename by component name"))
        {
            foreach (var o in Selection.gameObjects)
            {
                var go = o.gameObject;
                if (go.GetComponent<Button>() != null)
                {
                    if (go.GetComponent<BaseButton>() != null)
                    {
                        go.GetComponent<BaseButton>().Rename();
                    }
                    else if (go.GetComponentInChildren<Text>() != null)
                    {
                        var t = go.GetComponentInChildren<Text>().text;
                        go.name = "button_" + t;
                    }
                }
                else if (go.GetComponent<Image>() != null)
                {
                    go.name = "img_" + go.GetComponent<Image>().mainTexture.name;
                }
                else if (go.GetComponent<SpriteRenderer>() != null)
                {
                    go.name = "spr_" + go.GetComponent<SpriteRenderer>().sprite.name;
                }
                else if (go.GetComponent<TMP_Text>() != null)
                {
                    var t = go.GetComponent<TMP_Text>().text;
                    go.name = "text_" + t.Substring(0, Mathf.Min(10, t.Length));
                }


                EditorUtility.SetDirty(o);
            }
        }
    }
    
    

    private void OnCanvasTool()
    {
       
        StartHorizon(() =>
        {
            if (GUILayout.Button("Copy RT[0]"))
            {
                var targets = GetRt().ToArray();
                CopyValues(targets[0]);
            }

            if (GUILayout.Button("Paste to All RT"))
            {
                var targets = GetRt().ToArray();
                foreach (var o in targets)
                {
                    PasteValue(o);
                    EditorUtility.SetDirty(this);
                }
            }
        });
        
        StartHorizon(() =>
        {
            if (GUILayout.Button("CopyImage"))
            {
                var targets = GetRt().ToArray();

                CopyValuesImg(targets[0].gameObject.GetComponent<Image>());
            }

            if (GUILayout.Button("PasteImage"))
            {
                var targets = GetRt().ToArray();
                foreach (var o in targets)
                {
                    PasteValueImg(o.gameObject.GetComponent<Image>());
                    EditorUtility.SetDirty(this);
                }
            }
        });

        
        StartHorizon(() =>
        {
            if (GUILayout.Button("CopyText"))
            {
                var targets = GetRt().ToArray();
                toCopyText = targets[0].gameObject.GetComponent<TMP_Text>();
            }

            if (GUILayout.Button("PasteText"))
            {
                var targets = GetRt().ToArray();
                foreach (var o in targets)
                {
                    var text = o.gameObject.GetComponent<TMP_Text>();
                    text.text = toCopyText.text;
                    text.font = toCopyText.font;
                    text.color = toCopyText.color;
                    text.fontSize = toCopyText.fontSize;
                    text.enableAutoSizing = toCopyText.enableAutoSizing;
                    text.fontSizeMin = toCopyText.fontSizeMin;
                    text.fontSizeMax = toCopyText.fontSizeMax;
                    text.lineSpacing = toCopyText.lineSpacing;
                    text.wordSpacing = toCopyText.wordSpacing;
                    EditorUtility.SetDirty(text);
                }
            }
        });
        
        EditorGUILayout.Space(10);
          if (GUILayout.Button("Set Zero"))
        {
            foreach (var o in Selection.gameObjects)
            {
                var go = o.gameObject;
                if (go.GetComponent<Transform>() != null)
                    if (go.GetComponent<Transform>() != null)
                    {
                        go.GetComponent<Transform>().localPosition = Vector3.zero;
                        go.GetComponent<Transform>().localScale = Vector3.one;
                        go.GetComponent<Transform>().localEulerAngles = Vector3.zero;
                    }

                EditorUtility.SetDirty(o);
            }
        }

        if (GUILayout.Button("Expand"))
        {
            foreach (var o in Selection.gameObjects)
            {
                var go = o.gameObject;
                if (go.GetComponent<RectTransform>() != null)
                    if (go.GetComponent<RectTransform>() != null)
                        go.GetComponent<RectTransform>().SetExpand();
                EditorUtility.SetDirty(o);
            }
        }

        StartHorizon(() =>
        {
            if (GUILayout.Button("Top Left"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(0, 1);
                    myScript.anchorMax = new Vector2(0, 1);
                    myScript.sizeDelta = size;
                }
            }

            if (GUILayout.Button("Top"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(0.5f, 1);
                    myScript.anchorMax = new Vector2(0.5f, 1);
                    myScript.sizeDelta = size;
                }
            }

            if (GUILayout.Button("Top Right"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(1, 1);
                    myScript.anchorMax = new Vector2(1, 1);
                    myScript.sizeDelta = size;
                }
            }
        });

        StartHorizon(() =>
        {
            if (GUILayout.Button("Left"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(0, .5f);
                    myScript.anchorMax = new Vector2(0, .5f);
                    myScript.sizeDelta = size;
                }
            }

            if (GUILayout.Button("Center"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(0.5f, .5f);
                    myScript.anchorMax = new Vector2(0.5f, .5f);
                    myScript.sizeDelta = size;
                }
            }

            if (GUILayout.Button("Right"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(1, .5f);
                    myScript.anchorMax = new Vector2(1, .5f);
                    myScript.sizeDelta = size;
                }
            }
        });
        StartHorizon(() =>
        {
            if (GUILayout.Button("Bottom Left"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(0, 0);
                    myScript.anchorMax = new Vector2(0, 0);
                    myScript.sizeDelta = size;
                }
            }

            if (GUILayout.Button("Bottom"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(0.5f, 0);
                    myScript.anchorMax = new Vector2(0.5f, 0);
                    myScript.sizeDelta = size;
                }
            }

            if (GUILayout.Button("Bottom Right"))
            {
                var targets = GetRt().ToArray();
                foreach (var temp in targets)
                {
                    RectTransform myScript = (RectTransform) temp;
                    var size = myScript.rect.size;
                    myScript.anchorMin = new Vector2(1, 0);
                    myScript.anchorMax = new Vector2(1, 0);
                    myScript.sizeDelta = size;
                }
            }
        });
    }

    #region  property

    
    private string _json;
    private static Sprite _jsonImg;

    void CopyValuesImg(Image from)
    {
        _jsonImg = from.sprite;
    }

    void PasteValueImg(Image to)
    {
        to.sprite = _jsonImg;
    }

    private static Vector2[] _rtData;

    private static TMP_Text toCopyText;


    void CopyValues(RectTransform from)
    {
        _rtData = new[]
        {
            from.anchoredPosition,
            from.anchorMin,
            from.anchorMax,
            from.sizeDelta,
            from.pivot,
        };
    }

    void PasteValue(RectTransform to)
    {
        to.anchoredPosition = _rtData[0];
        to.anchorMin = _rtData[1];
        to.anchorMax = _rtData[2];
        to.sizeDelta = _rtData[3];
        to.pivot = _rtData[4];
    }

    IEnumerable<RectTransform> GetRt()
    {
        foreach (var o in Selection.objects)
        {
            if (o is GameObject)
            {
                var go = (GameObject) o;
                if (go.GetComponent<RectTransform>() != null)
                    yield return go.GetComponent<RectTransform>();
            }
        }
    }

    private int find = 1;

    private string _renamePrefix;

    #endregion

    private string DirtyScenePath
    {
        get => EditorPrefs.GetString("DirtyScenePath");
        set => EditorPrefs.SetString("DirtyScenePath", value);
    }

    private void OnFocus1()
    {
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
        StartHorizon(() =>
        {
            DirtyScenePath = EditorGUILayout.TextField("DirtyScenePath", DirtyScenePath);
            if (GUILayout.Button("Dirty Load Scene"))
            {
                EditorSceneManager.OpenScene(DirtyScenePath);
            }
        });
    }

    private Vector3Int date = new Vector3Int(1, 1, 2024);
    private int a;
    private int b;
    private void OnMath()
    {
        StartHorizon(() =>
        {
            date = EditorGUILayout.Vector3IntField("",date);

            if (GUILayout.Button("Log EpochDate"))
            {
                Debug.LogError(new DateTime(date.z, date.y, date.x));
                Debug.LogError(SdkExtenstions.GetEpochDate(date));
            }
        });
        StartHorizon(() =>
        {
            EditorGUILayout.LabelField("a, b");
            a =EditorGUILayout.IntField(a);
            b =EditorGUILayout.IntField(b);
        
        });
        
        EditorGUIUtility.labelWidth = 50;

        StartHorizon(() =>
        {
            GUI.enabled = false;
            EditorGUILayout.LabelField("a&b="+(a&b)); // intersect
            EditorGUILayout.LabelField("a|b="+(a|b)); // union
            EditorGUILayout.LabelField("(toggle)a^b="+(a^b));
            EditorGUILayout.LabelField("(remove)a&~b="+(a&~b));
            GUI.enabled = true;
        });
        
        StartHorizon(() =>
        {
            GUI.enabled = false;
            EditorGUILayout.LabelField("a repeat b="+(a.Repeat(b))); // intersect
            EditorGUILayout.LabelField("a ping pong b="+(a.PingPong(b))); // intersect
            GUI.enabled = true;
        });
    }

    protected override void Prepare()
    {
        base.Prepare();
        if (EnumValues == null)
            EnumValues = (int[]) Enum.GetValues(typeof(HideFlag));
    
    }


    private void ProjectSetting()
    {
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
    }


//    private BuilderRoot _builderRoot;

    private enum HideFlag
    {
        //        BuilderRoot ,
        ProjectSetting,
        Math,
        Focus,
        CanvasTool,
        EditorTool,
        InspectorTool,
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