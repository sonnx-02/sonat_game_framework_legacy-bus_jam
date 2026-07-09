using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(ListPointPointNameAttribute))]
public class ListPointPointNameAttributeDrawer : DuongClassDrawer
{
    private PointPointParameter _currentPoint;
    private ListPointPointNameAttribute Attribute => attribute as ListPointPointNameAttribute;

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

    public override IList<DivideData> DivideData(SerializedProperty property) => Attribute.Data;

    private Color _color;
    private int PropertyRowCount => 2;

    private int _currentSelectColor;

    private Vector2Int _size;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _color = GUI.color;
        // var cast = PropertyDrawerUtility.GetActualObjectForSerializedProperty<ListPoint>(fieldInfo, property);
        var cast = property.DuongGetSerializedValue<ListPointPoint>();

        var relativeProperty = DuongEditorHelper.FindPropertyRelative(property, Attribute.SizePropertyName);
        _size = new Vector2Int(Attribute.Col,Attribute.Row);
        if (relativeProperty != null)
            _size = relativeProperty.vector2IntValue;
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
            DividedFieldPixel(divideData,
                divideData.Weight > 0 ? remain * divideData.Weight / total : Mathf.Abs(divideData.Weight));


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


            DrawIntFieldOrEnumSelect(rects[0], ref _currentPoint.value,
                Attribute.EnumLists.Length > 0 ? Attribute.EnumLists[0] : null);
            DrawIntFieldOrEnumSelect(rects[1], ref _currentPoint.value2,
                Attribute.EnumLists.Length > 1 ? Attribute.EnumLists[1] : null);
            DrawIntFieldOrEnumSelect(rects[2], ref _currentPoint.value3,
                Attribute.EnumLists.Length > 2 ? Attribute.EnumLists[2] : null);
            DrawIntFieldOrEnumSelect(rects[3], ref _currentPoint.value4,
                Attribute.EnumLists.Length > 3 ? Attribute.EnumLists[3] : null);
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

 
    private void DrawIntFieldOrEnumSelect(Rect rect, ref int value, EnumList enumList)
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

                value = EditorGUI.IntPopup(rect, value, names.ToArray(),
                    values.ToArray());
            }
            else
                value = EditorGUI.IntField(rect, value);
        }
    }

    private void DrawBoardRelative(Vector2 rootPos, ListPointPoint cast, SerializedProperty property)
    {
        // draw bg
        var rootPoint = new PointParameter(Attribute.Col, Attribute.Row, 0, 0);
        for (int i = 0; i < Attribute.Col; i++)
        for (int j = 0; j < Attribute.Row; j++)
        {
            var point = new PointParameter(i, j, 0, 0);
            var rect = GetRect(point, rootPos);

            var color =(i * Attribute.Col + j) % 2 == 0 ? Color.gray : Gray2;
            if (i >= _size.x || j >= _size.y)
                color *= 0.7f;
            DrawBgTileButton(rect, color, () =>
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
                        var p = new PointPointParameter()
                        {
                            col = x.col,
                            row = x.row,
                            col2 = x.col,
                            row2 = x.row,
                        };
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
                cast.points = new List<PointPointParameter>();
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

    private void DrawBoardAbsolute(Vector2 rootPos, ListPointPoint cast, SerializedProperty property)
    {
        // draw bg
        for (int i = 0; i < Attribute.Col; i++)
        for (int j = 0; j < Attribute.Row; j++)
        {
            var point = new PointParameter(i, j, 0, 0);
            var rect = GetRect(point, rootPos);

            var color = (i * Attribute.Row + j) % 2 == 0 ? Color.gray : Gray2;
            if (i >= _size.x || j >= _size.y)
                color *= 0.7f;
            DrawBgTileButton(rect, color, () =>
            {
                Event current = Event.current;
                Debug.Log("none = select or create, control to delete");
                if (current.shift && _currentPoint != null)
                {
                    _currentPoint.col2 = point.col;
                    _currentPoint.row2 = point.row;
                    EditorApplication.update.Invoke();
                }
                else if (cast.points.Count(x => x == point) > 0)
                {
                    if (current.alt)
                    {
                        cast.points.RemoveAll(x => x == point);
                        property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                        _currentPoint = null;
                        EditorApplication.update.Invoke();
                    }
                    else if (_currentPoint == point)
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

                        EditorApplication.update.Invoke();
                    }
                    else
                        _currentPoint = cast.points.Find(x => x == point);
                }
                else
                {
                    var p = new PointPointParameter()
                    {
                        col = point.col,
                        row = point.row,
                        col2 = point.col,
                        row2 = point.row,
                    };
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
                cast.points = new List<PointPointParameter>();
            for (var index = 0; index < cast.points.Count; index++)
            {
                var point = cast.points[index];
                var rect = GetRect(point, rootPos);
                if (point.col != point.col2 || point.row != point.row2)
                {
                    var rectPoint2 = GetRect(point.Point2Get, rootPos);
                    if(point == _currentPoint)
                        DrawBgTile(rectPoint2, Color.black); // draw point2
                    else
                        DrawBgTile(rectPoint2, Color.white); // draw point2
                    
                }
                if (point.value < Colors.Length)
                    DrawBgTile(rect, Colors[point.value]);
                else
                {
                    Debug.LogError("out of range color");
                    DrawBgTile(rect, Color.black);
                }

                var rect2 = rect;
                rect2.x += rect.width / 2;
                rect2.width /= 2;
                if (point.value2 < Colors.Length)
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
                    .GetActualObjectForSerializedProperty<List<ListPointPoint>>(fieldInfo, property);
            Debug.LogError("Its a fucking list, please use array");
        }
    }

    private void DrawBoardColorMap(Vector2 rootPos, ListPointPoint cast, SerializedProperty property)
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

            var color =(i * Attribute.Row + j) % 2 == 0 ? Color.gray : Gray2;
            if (i >= _size.x || j >= _size.y)
                color *= 0.7f;
            // click
            DrawBgTileButton(rect, color, () =>
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
                    var newP = new PointPointParameter()
                    {
                        col = point.col,
                        row = point.row,
                        col2 = point.col,
                        row2 = point.row,
                        value = _currentSelectColor,
                    };
                    cast.points.Add(newP);
                    property.serializedObject.Update(); //not sure if required for redraw, but MY code needs it
                    EditorApplication.update.Invoke();
                }
            });
        }

        if (cast != null)
        {
            if (cast.points == null)
                cast.points = new List<PointPointParameter>();
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

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorSetting.Distance) *
               (Attribute.Row + (Attribute.ListPointType == ListPointType.Relative ? 1 : 0)) +
               EditorGUIUtility.singleLineHeight * PropertyRowCount + 10;
    }
}