using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class EditorSetting
{
    public const int MapSizeX = 5;
    public const int MapSizeY = 5;
    public const float PointSize = 20;
    public const float PointGap = 3;

    public static float Distance => (PointSize + PointGap);
}


public static class SerializedPropertyExtensions
{
    public static SerializedProperty FindParentProperty(this SerializedProperty serializedProperty)
    {
        var propertyPaths = serializedProperty.propertyPath.Split('.');
        if (propertyPaths.Length <= 1)
        {
            return default;
        }

        var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths.First());
        for (var index = 1; index < propertyPaths.Length - 1; index++)
        {
            if (propertyPaths[index] == "Array" && propertyPaths.Length > index + 1 &&
                Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
            {
                var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                var arrayIndex = int.Parse(match.Groups[1].Value);
                parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                index++;
            }
            else
            {
                parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
            }
        }

        return parentSerializedProperty;
    }
}


public abstract class DuongClassDrawer : ReloadEnumDrawer
{
    protected Rect WholeRect;
    protected float TotalWidthPixel;
    protected float PartialSum;
    private SerializedProperty _prop;
    public abstract IList<DivideData> DivideData(SerializedProperty property);
    
    protected virtual bool HideLabel => false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) +2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUIUtility.labelWidth = 80 + EditorGUI.indentLevel * 20;
        label = EditorGUI.BeginProperty(position, label, property);
        Init(position, property, label);

        var lastIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SetUp(DivideData(property),TotalWidthPixel);

        EditorGUI.indentLevel = lastIndentLevel;
        EditorGUI.EndProperty();
    }
    
    protected virtual void Init(Rect position, SerializedProperty property, GUIContent label)
    {
        this.PartialSum = 0;
        this._prop = property;
        WholeRect = HideLabel ? position : EditorGUI.PrefixLabel(position, label);
        TotalWidthPixel = WholeRect.width;
    }
    

    protected void SetUp(IList<DivideData> data,float totalWeight)
    {
        int total = data.Where(x => x.Weight > 0).Sum(x => x.Weight);
        var sumPixel = Mathf.Abs(data.Where(x => x.Weight < 0).Sum(x => x.Weight));
        var remain = totalWeight - sumPixel;
        foreach (var divideData in data)
            DividedFieldPixel(divideData,divideData.Weight > 0 ? remain * divideData.Weight / total : Mathf.Abs(divideData.Weight));
    }
    
    protected void DividedFieldPixel(DivideData divideData,float width)
    {
        var rect = new Rect(this.WholeRect.x + this.PartialSum, this.WholeRect.y, width, this.WholeRect.height);
        PartialSum += width;

        EditorGUIUtility.labelWidth = Mathf.Clamp(divideData.LabelWidth, 0, rect.width - 20);
        var item = _prop.FindPropertyRelative(divideData.PropertyName);
        if (item != null && !divideData.Disable)
        {

            if (divideData.EnumList != null && divideData.EnumList.Found && divideData.EnumList.Values.Length > 0)
            {
              //  Debug.Log("spawn enum");
                IndexAsEnumDrawerHelper.DrawPropertyAsEnum(divideData.EnumList,item,rect, new GUIContent(divideData.Label));
             //   EditorGUI.PropertyField(rect, item, new GUIContent(divideData.label));
            }
            else
                EditorGUI.PropertyField(rect, item, new GUIContent(divideData.Label));
        }
        else
        {
            Debug.LogWarningFormat("Failed to find property: '{0}' in '{1}'", divideData.PropertyName, this.GetType());
        }
    }

//    protected void DividedFieldRate(float widthRate, string propertyName, string label = "", float labelWidth = 0)
//    {
//        Debug.Assert(0 < widthRate);
//        Debug.Assert(widthRate <= 1);
//        Debug.Assert(!string.IsNullOrEmpty(propertyName));
//        Debug.Assert(label != null);
//        Debug.Assert(labelWidth >= 0);
//
//        var width = this.WholeRect.width * widthRate;
//        
//        var rect = new Rect(this.WholeRect.x + this.PartialSum, this.WholeRect.y, width, this.WholeRect.height);
//        this.PartialSum += width;
//
//        EditorGUIUtility.labelWidth = Mathf.Clamp(labelWidth, 0, rect.width - 20);
//        var item = this._prop.FindPropertyRelative(propertyName);
//        if (item != null)
//        {
//            EditorGUI.PropertyField(rect, item, new GUIContent(label));
//        }
//        else
//        {
//            Debug.LogWarningFormat("Failed to find property: '{0}' in '{1}'", propertyName, this.GetType());
//        }
//    }
//    
//    protected void DividedButton<T>( SerializedProperty property,float width, string methodName, string label = "")
//    {
//        var rect = new Rect(WholeRect.x + PartialSum, WholeRect.y, width-1, WholeRect.height);
//        PartialSum += width;
//        
//        var target = property.serializedObject.targetObject;
//        var type = target.GetType();
//      
//        System.Reflection.MethodInfo method = type.GetMethod(methodName);
//        if (method == null)
//        {
//              
//            GUI.Label(rect, "404");
//            return;
//        }
//        
//        if (GUI.Button(rect, methodName))
//        {
//            method.Invoke(target, new object[] {property.GetSerializedValue<T>()});
//        }
//    }
//    
    protected Texture2D BgTexture2D
    {
        get
        {
            return StaticTexture.SquareTexture;
//            switch (GameSetting.PointType)
//            {
//                case PointType.Square:
//                    return StaticTexture.SquareTexture;
//                case PointType.HexaHorizontal:
//                    return StaticTexture.HexaHorizontalTexture;
//                case PointType.HexaVertical:
//                    return StaticTexture.HexaVerticalTexture;
//                case PointType.Triangle:
//                    return StaticTexture.SquareTexture;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
        }
    }

    protected Rect GetRect(Point point, Vector2 root)
    {
        var hexaPos = (Vector2) point.GetPos(EditorSetting.Distance, EditorSetting.Distance);
        hexaPos.y *= -1;
        var pos = root + hexaPos;
        return new Rect(pos, new Vector2(EditorSetting.PointSize, EditorSetting.PointSize));
    }


    protected void DrawBgTile(Rect rect, Color bgColor)
    {
        GUI.color = bgColor;
        GUI.DrawTexture(rect, BgTexture2D, ScaleMode.StretchToFill);
    }

    GUIStyle blank = new GUIStyle();

    protected void DrawBgTileButton(Rect rect, Color bgColor, Action action)
    {
        GUI.color = bgColor;
        GUI.DrawTexture(rect, BgTexture2D, ScaleMode.StretchToFill);
        if (GUI.Button(rect, BgTexture2D, blank))
        {
            action.Invoke();
        }
    }
    
    protected void DrawBg(Rect rect, Color bgColor,float scale = 1)
    {
        GUI.color = bgColor;
        var copy = rect;
        copy.width *= scale;
        GUI.DrawTexture(copy, BgTexture2D, ScaleMode.StretchToFill);
    }
}


public class DuongPropertyDrawer : PropertyDrawer
{
    protected Texture2D BgTexture2D
    {
        get
        {
            return StaticTexture.SquareTexture;
//            switch (GameSetting.PointType)
//            {
//                case PointType.Square:
//                    return StaticTexture.SquareTexture;
//                case PointType.HexaHorizontal:
//                    return StaticTexture.HexaHorizontalTexture;
//                case PointType.HexaVertical:
//                    return StaticTexture.HexaVerticalTexture;
//                case PointType.Triangle:
//                    return StaticTexture.SquareTexture;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
        }
    }

    protected Rect GetRect(Point point, Vector2 root)
    {
        var hexaPos = (Vector2) point.GetPos(EditorSetting.Distance, EditorSetting.Distance);
        hexaPos.y *= -1;
        var pos = root + hexaPos;
        return new Rect(pos, new Vector2(EditorSetting.PointSize, EditorSetting.PointSize));
    }


    protected void DrawBgTile(Rect rect, Color bgColor)
    {
        GUI.color = bgColor;
        GUI.DrawTexture(rect, BgTexture2D, ScaleMode.StretchToFill);
    }

    GUIStyle blank = new GUIStyle();

    protected void DrawBgTileButton(Rect rect, Color bgColor, Action action)
    {
        GUI.color = bgColor;
        GUI.DrawTexture(rect, BgTexture2D, ScaleMode.StretchToFill);
        if (GUI.Button(rect, BgTexture2D, blank))
        {
            action.Invoke();
        }
    }

    protected void DrawBg(Rect rect, Color bgColor, float scale = 1)
    {
        GUI.color = bgColor;
        var copy = rect;
        copy.width *= scale;
        GUI.DrawTexture(copy, BgTexture2D, ScaleMode.StretchToFill);
    }
}

public abstract class DuongClassDrawer2 : DuongClassDrawer
{
    protected override bool HideLabel => false;

    public abstract string[] Displays { get; }
    
//    public override IList<DivideData> DivideData 
    public override IList<DivideData>  DivideData(SerializedProperty property) 
    {
        
            List<DivideData> data = new List<DivideData>();
            for (var i = 0; i < Displays.Length; i++)
                data.Add(new DivideData(1, Displays[i]));
            return data;
        
    }
}

public abstract class DuongConditionClassDrawer : DuongClassDrawer
{
    
}


public static class PropertyDrawerUtility
{
    public static T GetActualObjectForSerializedProperty<T>(FieldInfo fieldInfo, SerializedProperty property)
        where T : class
    {
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
        if (obj == null)
        {
            return null;
        }

        T actualObject = null;
        if (obj.GetType().IsArray)
        {
            var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
//            if (index > ((T[]) obj).Length)
//                index = ((T[]) obj).Length - 1;
            actualObject = ((T[]) obj)[index];
        }
        else
        {
            actualObject = obj as T;
        }

        return actualObject;
    }
}
