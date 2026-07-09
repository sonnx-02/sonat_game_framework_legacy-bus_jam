using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

internal class BindingItemOptionMenu
{
    
    [MenuItem( "GameObject/Duong/SpriteChangeImage" ,false,0) ]
    private static void CreateSpriteChange3(MenuCommand menuCommand)
    {
        EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
        var obj = Selection.activeGameObject.GetComponent<Image>();
        obj.name = "Img";

        var script = obj.gameObject.AddComponent<IndexBindingSpriteChange>();
        Selection.activeGameObject = script.gameObject;
        
        var rt = Selection.activeGameObject.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.localPosition = Vector3.zero;
        rt.anchoredPosition = Vector3.zero;
        rt.sizeDelta = Vector2.zero;
        rt.localScale = Vector3.one;
    }
    
    [MenuItem( "GameObject/Duong/Empty" ,false,0) ]
    private static void CreateEmpty(MenuCommand menuCommand)
    {
        EditorApplication.ExecuteMenuItem("GameObject/Create Empty");
        var rt = Selection.activeGameObject.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.localPosition = Vector3.zero;
        rt.anchoredPosition = Vector3.zero;
        rt.sizeDelta = Vector2.zero;
        rt.localScale = Vector3.one;
    }
} 