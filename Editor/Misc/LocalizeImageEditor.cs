using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.UI;

[CustomEditor(typeof(LocalizeImage))]
public class LocalizeImageEditor : UnityEditor.Editor
{ 
    public override void OnInspectorGUI()
    {
        LocalizeImage myScript = (LocalizeImage)target;
        var color = GUI.color;
        
        StartHorizon(() =>
        {
            if (GUILayout.Button("Copy Default"))
            {
                myScript.defaultItem.rectCopy = RectCopy.Copy(myScript.target.rectTransform);
                myScript.defaultItem.sprite = myScript.target.sprite;
            }

            if (GUILayout.Button("Apply Default"))
            {
                myScript.Apply(myScript.defaultItem);
            }
        });
        
        StartHorizon(() =>
        {
          
                
            if (GUILayout.Button("Copy Korea"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.Korean);
                if (korea == null)
                {
                    korea = new LocalizeItem()
                    {
                        language =  SystemLanguage.Korean
                    };
                    myScript.localizeItems.Add(korea);
                }
                
                korea.rectCopy = RectCopy.Copy(myScript.target.rectTransform);
                korea.sprite = myScript.target.sprite; 
            }

            if (GUILayout.Button("Apply Korea"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.Korean);
              
                myScript.Apply(korea);
            }
        });
        
         
        StartHorizon(() =>
        {
        
                
            if (GUILayout.Button("Copy Japan"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.Japanese);
                if (korea == null)
                {
                    korea = new LocalizeItem()
                    {
                        language =  SystemLanguage.Japanese
                    };
                    myScript.localizeItems.Add(korea);
                }
                
                korea.rectCopy = RectCopy.Copy(myScript.target.rectTransform);
                korea.sprite = myScript.target.sprite; 
            }

            if (GUILayout.Button("Apply Japan"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.Japanese);
               
                myScript.Apply(korea);
            }
        });
        
        StartHorizon(() =>
        {
        
                
            if (GUILayout.Button("Copy germany"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.German);
                if (korea == null)
                {
                    korea = new LocalizeItem()
                    {
                        language =  SystemLanguage.German
                    };
                    myScript.localizeItems.Add(korea);
                }
                
                korea.rectCopy = RectCopy.Copy(myScript.target.rectTransform);
                korea.sprite = myScript.target.sprite; 
            }

            if (GUILayout.Button("Apply germany"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.German);
               
                myScript.Apply(korea);
            }
        });
        
        StartHorizon(() =>
        {
        
                
            if (GUILayout.Button("Copy taiwan"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.ChineseTraditional);
                if (korea == null)
                {
                    korea = new LocalizeItem()
                    {
                        language =  SystemLanguage.ChineseTraditional
                    };
                    myScript.localizeItems.Add(korea);
                }
                
                korea.rectCopy = RectCopy.Copy(myScript.target.rectTransform);
                korea.sprite = myScript.target.sprite; 
            }

            if (GUILayout.Button("Apply taiwan"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.ChineseTraditional);
               
                myScript.Apply(korea);
            }
        });


        GUI.color = color;
        GUILayout.Space(10f);
        DrawDefaultInspector();
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



[CustomEditor(typeof(LocalizeTextMesh))]
public class LocalizeTextMeshEditor : UnityEditor.Editor
{ 
    public override void OnInspectorGUI()
    {
        LocalizeTextMesh myScript = (LocalizeTextMesh)target;
        var color = GUI.color;
        
        StartHorizon(() =>
        {
            if (GUILayout.Button("Copy Default"))
            {
                myScript.defaultItem.rectCopy = RectCopy.Copy(myScript.target.rectTransform);
                myScript.defaultItem.font = myScript.target.font;
                myScript.defaultItem.message = myScript.target.text;
                myScript.defaultItem.fontSize = myScript.target.fontSize;
            }

            if (GUILayout.Button("Apply Default"))
            {
                myScript.Apply(myScript.defaultItem);
            }
        });
        
        StartHorizon(() =>
        {
          
                
            if (GUILayout.Button("Copy Korea"))
            {
                var korea = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.Korean);
                if (korea == null)
                {
                    korea = new LocalizeTextItem()
                    {
                        language =  SystemLanguage.Korean
                    };
                    myScript.localizeItems.Add(korea);
                }
                
                korea.rectCopy = RectCopy.Copy(myScript.target.rectTransform);
                korea.font = myScript.target.font;
                korea.message = myScript.target.text;
                korea.fontSize = myScript.target.fontSize;
            }

            if (GUILayout.Button("Apply Korea"))
            {
                myScript.Apply(SystemLanguage.Korean);
            }
        });
        
         
        StartHorizon(() =>
        {
              
            if (GUILayout.Button("Copy Japan"))
            {
                var japan = myScript.localizeItems.ToList().Find(x => x.language == SystemLanguage.Japanese);
                if (japan == null)
                {
                    japan = new LocalizeTextItem()
                    {
                        language =  SystemLanguage.Japanese
                    };
                    myScript.localizeItems.Add(japan);
                }
                
                japan.rectCopy = RectCopy.Copy(myScript.target.rectTransform);
                japan.font = myScript.target.font;
                japan.message = myScript.target.text;
                japan.fontSize = myScript.target.fontSize;
            }

            if (GUILayout.Button("Apply Japan"))
            {
                myScript.Apply(SystemLanguage.Japanese);
            }
        });

        GUI.color = color;
        GUILayout.Space(10f);
        DrawDefaultInspector();
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
