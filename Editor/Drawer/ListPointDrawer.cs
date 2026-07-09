using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalDivideDataAttribute))]
public class ConditionalDivideDataAttributeDrawer : DuongClassDrawer
{
    private ConditionalDivideDataAttribute Attribute => attribute as ConditionalDivideDataAttribute;

    public override IList<DivideData> DivideData(SerializedProperty property)
    {
        var relativeProperty = DuongEditorHelper.FindPropertyRelative(property, Attribute.EnumPropertyName);
        Debug.Log(relativeProperty.intValue);
        return new List<DivideData>();
        // calculate condition here
    }
    protected override bool HideLabel => false;
}




// default draw as relative
[CustomPropertyDrawer(typeof(ListPointNameAttribute))]
public class ListPointNameAttributeDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => Attribute.Data;

    private ListPointNameAttribute Attribute => attribute as ListPointNameAttribute;
    private Color _color;

    private int PropertyRowCount => 2;

    private PointParameter _currentPoint;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _color = GUI.color;
       // var cast = PropertyDrawerUtility.GetActualObjectForSerializedProperty<ListPoint>(fieldInfo, property);
        var cast = property.DuongGetSerializedValue<ListPoint>();
//

        // Draw label
        label = EditorGUI.BeginProperty(position, label, property);
        Init(position, property, label);

        // draw parameter
        var oldPos = position;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var item = property.FindPropertyRelative(nameof(ListPoint.customParameter));
        WholeRect = new Rect(position.x, 2 + oldPos.y, position.width, EditorGUIUtility.singleLineHeight);

        var data = DivideData(property);
        int total = data.Where(x => x.Weight > 0).Sum(x => x.Weight);
        var sumPixel = Mathf.Abs(data.Where(x => x.Weight < 0).Sum(x => x.Weight));
        var remain = TotalWidthPixel - sumPixel;
        foreach (var divideData in data)
            DividedFieldPixel(divideData,divideData.Weight > 0 ? remain * divideData.Weight / total : Mathf.Abs(divideData.Weight));

        if (_currentPoint != null)
        {
            Rect[] rects = new Rect[4];
            for (var i = 0; i < rects.Length; i++)
            {
                rects[i] = position;
                rects[i].width = position.width / rects.Length;
                rects[i].height = EditorGUIUtility.singleLineHeight;
                rects[i].x += i * rects[i].width;
                rects[i].y += EditorGUIUtility.singleLineHeight;
            }

         
            DrawIntFieldOrEnumSelect(rects[0], ref _currentPoint.value, Attribute.EnumLists.Length > 0 ? Attribute.EnumLists[0] : null);
            DrawIntFieldOrEnumSelect(rects[1], ref _currentPoint.value2, Attribute.EnumLists.Length > 1 ? Attribute.EnumLists[1] : null);
            DrawIntFieldOrEnumSelect(rects[2], ref _currentPoint.value3, Attribute.EnumLists.Length > 2 ? Attribute.EnumLists[2] : null);
            DrawIntFieldOrEnumSelect(rects[3], ref _currentPoint.value4, Attribute.EnumLists.Length > 3 ? Attribute.EnumLists[3] : null);
        }


        var rootPos = new Vector2(position.position.x,
            position.position.y + (EditorSetting.Distance) *
            (Attribute.Row + (Attribute.ListPointType == ListPointType.Relative ? 0 : -1)) + 10 +
            EditorGUIUtility.singleLineHeight * PropertyRowCount);
        switch (Attribute.ListPointType)
        {
            // draw board
            case ListPointType.Relative:
                DrawBoardRelative(rootPos, cast, property);
                break;
            case ListPointType.Absolute:
                DrawBoardAbsolute(rootPos, cast, property);
                break;
            case ListPointType.ColorMap:
                DrawBoardColorMap(rootPos, cast, property);
                break;
            default:
                Debug.LogError("not support this " + Attribute.ListPointType);
                break;
        }


        GUI.color = _color;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
//        if (!DrawPointMap)
//            return GetPropertyHeight(property, label);
        
        return (EditorSetting.Distance) *
               (Attribute.Row + (Attribute.ListPointType == ListPointType.Relative ? 1 : 0)) +
               EditorGUIUtility.singleLineHeight * PropertyRowCount + 10;
    }
    
    private void DrawIntFieldOrEnumSelect(Rect rect,ref int value,EnumList enumList)
    {
        if (enumList == null)
            value = EditorGUI.IntField(rect, value);
        else
        {
            if (!enumList.Found)
                enumList.Reload(DuongEditorHelper.GetType(enumList.TypeName));

            if (enumList.Found && enumList.Values.Length > 0)
            {
                int index = -1;
                for (var i = 0; i < enumList.Values.Length; i++)
                    if (value == enumList.Values[i])
                    {
                        index = i;
                        break;
                    }

                IList<int> values = enumList.Values.ToList();
                IList<GUIContent> names = enumList.Names.ToList();
                
                if (index == -1)
                {
                    values = values.ArrayAdd(value);
                    names = names.ArrayAdd(new GUIContent($"NotFound {enumList.TypeName}_{value}"));
                }

                value = EditorGUI.IntPopup(rect,  value, names.ToArray(),
                    values.ToArray());
            }
            else
                value = EditorGUI.IntField(rect, value);
        }
    }

    private void DrawBoardRelative(Vector2 rootPos, ListPoint cast, SerializedProperty property)
    {
        // draw bg
        var rootPoint = new PointParameter(Attribute.Col, Attribute.Row, 0, 0);
        for (int i = 0; i < Attribute.Col; i++)
        for (int j = 0; j < Attribute.Row; j++)
        {
            var point = new PointParameter(i, j, 0, 0);
            var rect = GetRect(point, rootPos);

            DrawBgTileButton(rect, (i * Attribute.Col + j) % 2 == 0 ? Color.gray : Gray2, () =>
            {
                if (cast.points.Count(x => x == point - rootPoint) > 0)
                {
                    cast.points.RemoveAll(x => x == point - rootPoint);
                    property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                    EditorApplication.update.Invoke();
                }
                else
                {
                    var x = point - rootPoint;
                    if (x != Point.Zero)
                    {
                        var p = new PointParameter(x.col, x.row, 0, 0);
                        cast.points.Add(p);
                        _currentPoint = p;
                        property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                        EditorApplication.update.Invoke();
                    }
                }
            });
        }

        if (cast != null)
        {
            if (cast.points == null)
                cast.points = new List<PointParameter>();
            foreach (var point in cast.points)
            {
                var rect = GetRect(point + rootPoint, rootPos);
                DrawBgTile(rect, Colors[cast.customParameter.intValue % Colors.Length]);
            }

            // draw cemter
            var rect0 = GetRect(rootPoint, rootPos);
            DrawBgTile(rect0, Colors[cast.customParameter.intValue % Colors.Length] + new Color(0.2f, 0.2f, 0.2f));
        }
        else
        {
            var cast2 =
                PropertyDrawerUtility
                    .GetActualObjectForSerializedProperty<List<ListPoint>>(fieldInfo, property);
            Debug.LogError("Its a fucking list, please use array");
        }
    }

    private void DrawBoardAbsolute(Vector2 rootPos, ListPoint cast, SerializedProperty property)
    {
        // draw bg
        for (int i = 0; i < Attribute.Col; i++)
        for (int j = 0; j < Attribute.Row; j++)
        {
            var point = new PointParameter(i, j, 0, 0);
            var rect = GetRect(point, rootPos);

            DrawBgTileButton(rect, (i * Attribute.Row + j) % 2 == 0 ? Color.gray : Gray2, () =>
            {
                Event current = Event.current;
                Debug.Log("none = select or create, control to delete");
                if (cast.points.Count(x => x == point) > 0)
                {
                    if (current.alt)
                    {
                        cast.points.RemoveAll(x => x == point);
                        property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                        _currentPoint = null;
                        EditorApplication.update.Invoke();
                    }
                    else
                    {
                        if (_currentPoint == point)
                        {
                            if (current.button == 1)
                            {
                                _currentPoint.value = 0;
                                _currentPoint.value2 = 0;
                            }
                            else if (current.control)
                                _currentPoint.value2 = (_currentPoint.value2 + 1) % Colors.Length;
                            else
                                _currentPoint.value = (_currentPoint.value + 1) % Colors.Length;
                        }
                        else
                            _currentPoint = cast.points.Find(x => x == point);

                        EditorApplication.update.Invoke();
                    }
                }
                else
                {
                    var p = new PointParameter(point.col, point.row, 0, 0);
                    cast.points.Add(p);
                    property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                    _currentPoint = p;
                    EditorApplication.update.Invoke();
                }

                EditorGUI.FocusTextInControl(null);
            });
        }

        if (cast != null)
        {
            if (cast.points == null)
                cast.points = new List<PointParameter>();
            for (var index = 0; index < cast.points.Count; index++)
            {
                var point = cast.points[index];
                var rect = GetRect(point, rootPos);
                if(point.value < Colors.Length )
                    DrawBgTile(rect, Colors[point.value]);
                else
                {
                    Debug.LogError("out of range color");
                    DrawBgTile(rect, Color.black);
                }
             
                var rect2 = rect;
                rect2.x += rect.width / 2;
                rect2.width /= 2;
                if(point.value2 < Colors.Length )
                    DrawBgTile(rect2, Colors[point.value2]);
                else
                {
                    Debug.LogError("out of range color");
                    DrawBgTile(rect2, Color.black);
                }

                if (point == _currentPoint)
                {
                    var rt = rect;
                    rt.size /= 4f;
                    DrawBgTile(rt, Color.white);
                }
            }
        }
        else
        {
            var cast2 =
                PropertyDrawerUtility
                    .GetActualObjectForSerializedProperty<List<ListPoint>>(fieldInfo, property);
            Debug.LogError("Its a fucking list, please use array");
        }
    }

    private int _currentSelectColor;

    private void DrawBoardColorMap(Vector2 rootPos, ListPoint cast, SerializedProperty property)
    {
        // draw select color
        var rootPoint = new Point(-2, 0);
        var rect2 = GetRect(rootPoint, rootPos);
        DrawBgTileButton(rect2, Color.gray, () => { cast.points.Clear(); });
        rect2.x -= EditorSetting.Distance + 5;
        if (_currentSelectColor < 0)
            DrawBg(rect2, Color.white, 1.1f);
        DrawBgTileButton(rect2, Color.gray, () => { _currentSelectColor = -1; });

        for (int i = 0; i < Colors.Length; i++)
        {
            rootPoint = new Point(-2, i + 1);
            rect2 = GetRect(rootPoint, rootPos);
            var i1 = i;
            DrawBgTileButton(rect2, Colors[i], () =>
            {
                foreach (var pointWithTwoValue in cast.points)
                    pointWithTwoValue.value = i1;
            });
        }

        for (int i = 0; i < Colors.Length; i++)
        {
            var i1 = i;
            rootPoint = new Point(-2, i + 1);
            rect2 = GetRect(rootPoint, rootPos);
            rect2.x -= EditorSetting.Distance + 5;

            if (_currentSelectColor == i1)
                DrawBg(rect2, Color.white, 1.1f);
            DrawBgTileButton(rect2, Colors[i], () => { _currentSelectColor = i1; });
        }

        // draw board
        for (int i = 0; i < Attribute.Col; i++)
        for (int j = 0; j < Attribute.Row; j++)
        {
            var point = new Point(i, j);
            var rect = GetRect(point, rootPos);

            // click
            DrawBgTileButton(rect, (i * Attribute.Row + j) % 2 == 0 ? Color.gray : Gray2, () =>
            {
                var find = cast.points.Find(x => x == point);
                if (find != null)
                {
                    if (find.value < 0)
                        cast.points.Remove(find);
                    else if (_currentSelectColor >= 0)
                        find.value = _currentSelectColor;
                    else
                        cast.points.Remove(find);
                    property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                    EditorApplication.update.Invoke();
                }
                else if (_currentSelectColor >= 0)
                {
                    cast.points.Add(point.ToPointParameter(_currentSelectColor, 0));
                    property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                    EditorApplication.update.Invoke();
                }
            });
        }

        if (cast != null)
        {
            if (cast.points == null)
                cast.points = new List<PointParameter>();
            foreach (var point in cast.points)
            {
                var rect = GetRect(point, rootPos);
                DrawBgTile(rect, Colors[point.value]);
            }
        }
        else
        {
            var cast2 =
                PropertyDrawerUtility.GetActualObjectForSerializedProperty<List<MapListPoint>>(fieldInfo, property);
            Debug.LogError("Its a fucking list, please use array");
        }


        GUI.color = _color;
    }

    private static readonly Color Gray2 = new Color(0.45f, 0.45f, 0.45f);

    private static readonly Color[] Colors =
    {
        Color.green, Color.cyan, Color.yellow, Color.blue, Color.magenta, Color.red, new Color(0.7f, 0.4f, 0.6f),
        new Color(0.3f, 0.4f, 0.1f),
        new Color(0.3f, 0.8f, 0.3f),
        new Color(0.8f, 0.8f, 0.3f),
        new Color(0.8f, 0.2f, 0.3f),
        new Color(0.3f, 0.5f, 1f),
        new Color(0.8f, 0.1f, 1f),
    };


    private SerializedProperty _prop;
}

// default draw as relative
[CustomPropertyDrawer(typeof(ListPoint))]
public class ListPointDrawer : DuongPropertyDrawer
{
    private static readonly Color[] _colors = {
        Color.green, Color.cyan, Color.yellow, Color.blue, Color.magenta, Color.red, new Color(0.7f, 0.4f, 0.6f),
    };
    
    private Color _color;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _color = GUI.color;

        var obj = property.DuongGetSerializedValue<ListPoint>();
//        var cast = PropertyDrawerUtility.GetActualObjectForSerializedProperty<ListPoint>(fieldInfo, property);
        var cast = obj;
        // Draw label
        label = EditorGUI.BeginProperty(position, label, property);
        var oldPos = position;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var item = property.FindPropertyRelative(nameof(ListPoint.customParameter));
        EditorGUI.PropertyField(new Rect(oldPos.x, 2 + oldPos.y, oldPos.width, EditorGUIUtility.singleLineHeight), item,
            new GUIContent(" "));

        // draw board
        var rootPos = new Vector2(position.position.x,
            position.position.y + (EditorSetting.Distance) * (DrawerExtensions.MapSize - 1) + 10 +
            EditorGUIUtility.singleLineHeight);
        var rootPoint = new PointParameter(DrawerExtensions.MapUnit, DrawerExtensions.MapUnit, 0, 0);

        for (int i = 0; i < DrawerExtensions.MapSize; i++)
        for (int j = 0; j < DrawerExtensions.MapSize; j++)
        {
            var point = new PointParameter(i, j, 0, 0);
            var rect = GetRect(point, rootPos);

            DrawBgTileButton(rect, Color.gray, () =>
            {
                if (cast.points.Count(x => x == point - rootPoint) > 0)
                {
                    cast.points.RemoveAll(x => x == point - rootPoint);
                    property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                    EditorApplication.update.Invoke();
                }
                else
                {
                    var x = point - rootPoint;
                    if (x != Point.Zero)
                    {
                        cast.points.Add(new PointParameter(x.col, x.row, 0, 0));
                        property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                        EditorApplication.update.Invoke();
                    }
                }
            });
        }

        if (cast != null)
        {
            if (cast.points == null)
                cast.points = new List<PointParameter>();
            foreach (var point in cast.points)
            {
                var rect = GetRect(point + rootPoint, rootPos);
                DrawBgTile(rect, _colors[cast.customParameter.intValue % _colors.Length]);
            }

            var rect0 = GetRect(rootPoint, rootPos);
            DrawBgTile(rect0, _colors[cast.customParameter.intValue % _colors.Length] + new Color(0.2f, 0.2f, 0.2f));
        }
        else
        {
            var cast2 = PropertyDrawerUtility .GetActualObjectForSerializedProperty<List<ListPoint>>(fieldInfo, property);
            Debug.LogError("Its a fucking list, please use array");
        }

        GUI.color = _color;
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
//        return base.GetPropertyHeight(property, label) * EditorSetting.MapSizeY;
//        ListRelativePoint myDataClass = PropertyDrawerUtility.GetActualObjectForSerializedProperty<ListRelativePoint>(fieldInfo, property);
        return (EditorSetting.Distance) * DrawerExtensions.MapSize + EditorGUIUtility.singleLineHeight + 10;
    }
}



// default draw as relative
[CustomPropertyDrawer(typeof(MyMatrixAttribute))]
public class MatrixAttributeDrawer : DuongPropertyDrawer
{
    private static readonly Color[] _colors = {
        Color.green, Color.cyan, Color.yellow, Color.blue, Color.magenta, Color.red, new Color(0.7f, 0.4f, 0.6f),
    };
    
    private Color _color;

    private int _current = -1;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _color = GUI.color;

//        var cast = PropertyDrawerUtility.GetActualObjectForSerializedProperty<ListPoint>(fieldInfo, property);
        var cast = property.DuongGetSerializedValue<ListInt>();
        if (cast == null || cast.value.Count %2 == 1)
        {
            OnGUI(position,property,label);
            return;
        }
        
        if (cast.value.Count < 2)
        {
            cast.value.Add(5);
            cast.value.Add(5);
        }
        // Draw label
        label = EditorGUI.BeginProperty(position, label, property);
        var oldPos = position;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//        
        cast.active = EditorGUI.Toggle(
            new Rect(position.x, 2 + oldPos.y, 25, EditorGUIUtility.singleLineHeight),
            cast.active);
        cast.value[0] = EditorGUI.IntField(
            new Rect(position.x +30, 2 + oldPos.y, position.width/3-5 -15, EditorGUIUtility.singleLineHeight),
            cast.value[0]);
        cast.value[1] = EditorGUI.IntField(
            new Rect(position.x+position.width/3+15, 2 + oldPos.y, position.width/3 -15, EditorGUIUtility.singleLineHeight),
            cast.value[1]);
        
        if(_current < cast.value.Count && _current >= 2)
            cast.value[_current+1] = EditorGUI.IntField(
                new Rect(position.x + position.width / 3 * 2 + 15, 2 + oldPos.y, position.width / 3 - 15,
                    EditorGUIUtility.singleLineHeight),
                cast.value[_current+1]);
//
//        // draw board

        if (cast.active)
        {
            var rootPos = new Vector2(position.position.x,
                position.position.y + (EditorSetting.Distance) * (cast.value[1] - 1) + 10 +
                EditorGUIUtility.singleLineHeight);
            var rootPoint = new PointParameter(cast.value[0], cast.value[1], 0, 0);

            Event current = Event.current;
            for (int i = 0; i < cast.value[0]; i++)
            for (int j = 0; j < cast.value[1]; j++)
            {
                var point = new PointParameter(i, j, 0, 0);
                var rect = GetRect(point, rootPos);

                var i2 = i;
                var j1 = j;
                DrawBgTileButton(rect, Color.gray, () =>
                {
                    bool found = false;
                    if (current.alt && current.shift)
                    {
                        cast.value = cast.value.Take(2).ToList();
                                
                        property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                        EditorApplication.update.Invoke();
                        return;
                    }
                    for (var i1 = 2; i1 < cast.value.Count; i1+=2)
                    {
                        if (cast.value[i1] == PointExtension.ToIndex(i2,j1,cast.value[0]))
                        {
                          
                            _current = i1;
                            if (current.alt)
                            {
                                cast.value.RemoveAt(i1+1);
                                cast.value.RemoveAt(i1);
                                _current = -1;
                                found = true;
                            }
                            else
                            {
                                cast.value[i1 + 1] += 1;
                                found = true;
                            }
                            break;
                        }
                    }

                    if (!found)
                    {
                        cast.value.Add(PointExtension.ToIndex(i2,j1,cast.value[0]));
                        cast.value.Add(0);
                        _current = cast.value.Count - 2;
                    }
                    
                    property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                    EditorApplication.update.Invoke();

//                if (cast.points.Count(x => x == point - rootPoint) > 0)
//                {
//                    cast.points.RemoveAll(x => x == point - rootPoint);
//                    property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
//                    EditorApplication.update.Invoke();
//                }
//                else
//                {
//                    var x = point - rootPoint;
//                    if (x != Point.Zero)
//                    {
//                        cast.points.Add(new PointParameter(x.col, x.row, 0, 0));
//                        property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
//                        EditorApplication.update.Invoke();
//                    }
//                }
                });
            }
            
            for (var i = 2; i < cast.value.Count; i+=2)
            {
                var point = cast.value[i].ToPoint(cast.value[0]);
                var rect = GetRect(point , rootPos);
                DrawBgTile(rect, _colors[cast.value[i+1] % _colors.Length]);
            }
        }
      
       
//
      


        GUI.color = _color;
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var cast = property.DuongGetSerializedValue<ListInt>();
        if (cast == null || !cast.active)
            return base.GetPropertyHeight(property, label);
        
        if (cast.value.Count < 2)
        {
            cast.value.Add(5);
            cast.value.Add(5);
        }
        return (EditorSetting.Distance) * cast.value[1] + EditorGUIUtility.singleLineHeight + 10;
    }
}