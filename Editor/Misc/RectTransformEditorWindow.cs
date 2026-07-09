using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class RectTransformEditorWindow : EditorWindow
{
//    void CopyValues<T>(T from) {
//        _json = JsonUtility.ToJson(from);
//    }
//    
//    void PasteValue<T>(T to) {
//        JsonUtility.FromJsonOverwrite(_json, to);
//    }


    [MenuItem("Tools/DuongPham/" + nameof(RectTransformEditorWindow))]
    private static void ShowEditor()
    {
        GetWindow(typeof(RectTransformEditorWindow), false, nameof(RectTransformEditorWindow));
    }


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

    void OnGUI()
    {
        GUI.color = Color.cyan;
        ;

        RectTransform[] rt;

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
            if (GUILayout.Button("Rename"))
            {
                var x = Selection.gameObjects.OrderBy(a => a.transform.GetSiblingIndex()).ToArray();
                for (var i = 0; i < x.Length; i++)
                    x[i].name = _renamePrefix + " " + i;
            }
        });

        StartHorizon(() =>
        {
            if (GUILayout.Button("Copy"))
            {
                var targets = GetRt().ToArray();
                CopyValues(targets[0]);
            }

            if (GUILayout.Button("Paste"))
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

        if (GUILayout.Button("Rename"))
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

        if (GUILayout.Button("Refactor"))
        {
            var targets = GetRt().ToArray();
            foreach (var o in Selection.objects)
            {
                if (o is GameObject)
                {
//                    var toggles =(o as GameObject).GetComponents<ToggleScript>();
//                    if(toggles != null)
//                        foreach (var toggleScript in toggles)
//                            toggleScript.CopySound();
//                    var toggles2 =(o as GameObject).GetComponents<IndexBindingScript>();
//                    if(toggles2 != null)
//                        foreach (var toggleScript in toggles2)
//                            toggleScript.CopySound();
                    EditorUtility.SetDirty(o);
                }
            }
        }

        GUILayout.Space(100);

        if (GUILayout.Button("Copy"))
        {
            foreach (var o in Selection.gameObjects)
            {
                var x = o.GetComponents<IndexBindingScriptBase>();
                Debug.Log(x.Length);
                foreach (var indexBindingScript in x)
                    if (indexBindingScript is IndexBindingScript script)
                        script.CopySetting();
            }
        }

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

    void StartHorizon(Action action)
    {
        //*
        EditorGUILayout.BeginHorizontal();
        action.Invoke();
        EditorGUILayout.EndHorizontal();
        //*
    }
}