using System;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FlexibleCondition))]
public class FlexibleConditionDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property)
    {
//        var activeProperty =  DuongEditorHelper.FindPropertyRelative(property,"active");
//        if (activeProperty != null)
//            Debug.Log(activeProperty.boolValue);
        var childProperty = property.FindPropertyRelative(nameof(FlexibleCondition.dataType));

        var list = new List<DivideData>
        {
            new DivideData(5, nameof(FlexibleCondition.dataType), "DataType", 1),
        };

        switch ((DataType) childProperty.intValue)
        {
            case DataType.String:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueString)));
                break;
            case DataType.Int:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueInt)));
                break;
            case DataType.Float:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueFloat)));
                break;
            case DataType.Boolean:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueBool)));
                break;
            case DataType.Json:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueString)));
                break;
            case DataType.Vector2:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueFloat)));
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueFloat2)));
                break;
            case DataType.Vector3:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueFloat)));
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueFloat2)));
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueFloat3)));
                break;
            case DataType.Point:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueInt)));
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueInt2)));
                break;
            case DataType.Point3:
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueInt)));
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueInt2)));
                list.Add(new DivideData(10, nameof(FlexibleCondition.valueInt3)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return list;
    }

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(Point))]
public class PointDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(Point.col), "", 1),
        new DivideData(1, nameof(Point.row), "", 1)
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(IndexActive))]
public class IndexActiveDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(2, nameof(IndexActive.value), "", 1),
        new DivideData(1, nameof(IndexActive.reverse), "r!", 20),
        new DivideData(1, nameof(IndexActive.bit), "bit", 20),
        new DivideData(4, nameof(IndexActive.target)),
    };

    protected override bool HideLabel => true;
}

[CustomPropertyDrawer(typeof(IndexActiveToggle))]
public class IndexActiveToggleDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(2, nameof(IndexActive.value), "", 1),
        new DivideData(1, nameof(IndexActive.reverse), "r", 6),
        new DivideData(1, nameof(IndexActive.bit), "bit", 20),
        new DivideData(3, nameof(IndexActive.target)),
        new DivideData(3, nameof(IndexActiveToggle.toggleScript)),
    };

    protected override bool HideLabel => true;
}


[CustomPropertyDrawer(typeof(Vector2Int))]
public class Vector2IntDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(Vector2Int.x), "", 1),
        new DivideData(1, nameof(Vector2Int.y), "", 1)
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(StringIntFloatFloat))]
public class StringIntFloatFloatDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(StringIntFloatFloat.valueStr), "", 1),
        new DivideData(1, nameof(StringIntFloatFloat.valueInt), "", 1),
        new DivideData(1, nameof(StringIntFloatFloat.value1), "", 1),
        new DivideData(1, nameof(StringIntFloatFloat.value2), "", 1)
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(IntTransform))]
public class IntTransformDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(IntTransform.value), "", 1),
        new DivideData(1, nameof(IntTransform.value2), "", 1)
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(IntInt))]
[CustomPropertyDrawer(typeof(StringBool))]
[CustomPropertyDrawer(typeof(IntActionScript))]
public class IntIntDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(IntInt.value), "", 1),
        new DivideData(1, nameof(IntInt.value2), "", 1)
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(IntBool))]
public class IntBoolDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(IntBool.value2), "   ", 4),
        new DivideData(3, nameof(IntBool.value), "", 1),
    };

    protected override bool HideLabel => false;
}

//[CustomPropertyDrawer(typeof(IntBoolWhen))]
//public class IntBoolWhenDrawer : DuongClassDrawer
//{
//    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
//    {
//        new DivideData(3, nameof(IntBoolWhen.value), "", 1),
//        new DivideData(1, nameof(IntBoolWhen.value2), "   ", 4),
//        new DivideData(1, nameof(IntBoolWhen.value3))
//    };
//
//    protected override bool HideLabel => false;
//}


[CustomPropertyDrawer(typeof(IntIntInt))]
public class IntIntIntDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(IntIntInt.value), "", 1),
        new DivideData(1, nameof(IntIntInt.value2), "", 1),
        new DivideData(1, nameof(IntIntInt.value3), "", 1),
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(QuantitySprite))]
public class QuantitySpriteDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(QuantitySprite.quantity), "", 1),
        new DivideData(1, nameof(QuantitySprite.sprite), "", 1),
    };

    protected override bool HideLabel => false;
}
[CustomPropertyDrawer(typeof(QuantityText))]
public class QuantityTextDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(QuantityText.quantity), "", 1),
        new DivideData(1, nameof(QuantityText.text), "", 1),
    };

    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(QuantityIndexText))]
public class QuantityIndexTextDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(QuantityIndexText.quantity), "", 1),
        new DivideData(1, nameof(QuantityIndexText.text), "", 1),
        new DivideData(1, nameof(QuantityIndexText.index), "", 1),
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(QuantityIndexSprite))]
public class QuantityIndexSpriteDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(QuantityIndexSprite.quantity), "", 1),
        new DivideData(1, nameof(QuantityIndexSprite.sprite), "", 1),
        new DivideData(1, nameof(QuantityIndexSprite.index), "", 1),
    };

    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(IntIntBoolInt))]
public class IntIntBoolIntDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(4, nameof(IntIntBoolInt.value), "", 1),
        new DivideData(2, nameof(IntIntBoolInt.value2), "", 1),
        new DivideData(1, nameof(IntIntBoolInt.value3), "", 1),
        new DivideData(2, nameof(IntIntBoolInt.value4), "", 1),
    };

    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(BoolFloat))]
public class BoolFloatDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(BoolFloat.value), "", 1),
        new DivideData(1, nameof(BoolFloat.value2), "", 1),
    };

    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(IntIntFloat))]
[CustomPropertyDrawer(typeof(BoolFloatFloat))]
public class IntIntFloatDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(IntIntFloat.value), "", 1),
        new DivideData(1, nameof(IntIntFloat.value2), "", 1),
        new DivideData(1, nameof(IntIntFloat.value3), "", 1),
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(IntFloat))]
public class IntFloatDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(IntFloat.value), "", 1),
        new DivideData(1, nameof(IntFloat.value2), "", 1),
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(IntFloatScript))]
public class IntFloatScriptDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(IntFloatScript.value), "i", 10),
        new DivideData(1, nameof(IntFloatScript.value2), "v", 10),
        new DivideData(1, nameof(IntFloatScript.value3), "", 1),
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(AnimationData))]
public class AnimationDataDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(AnimationData.active), "", 1),
        new DivideData(1, nameof(AnimationData.x), "", 1),
        new DivideData(1, nameof(AnimationData.y), "", 1),
        new DivideData(1, nameof(AnimationData.z), "", 1),
        new DivideData(1, nameof(AnimationData.time), "t", 5),
        new DivideData(1, nameof(AnimationData.ease), "", 1),
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(PointBoolean))]
public class PointBooleanDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(Point.col), "", 1),
        new DivideData(1, nameof(Point.row), " ", 3),
        new DivideData(1, nameof(PointBoolean.value), " ", 3)
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(PointParameter))]
public class PointParameterDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(10, nameof(PointParameter.col), nameof(PointParameter.col), 20),
        new DivideData(10, nameof(PointParameter.row), nameof(PointParameter.row), 25),
        new DivideData(12, nameof(PointParameter.value), "params", 42),
        new DivideData(5, nameof(PointParameter.value2), "", 2),
        new DivideData(5, nameof(PointParameter.value3), "", 2),
        new DivideData(5, nameof(PointParameter.value4), "", 2),
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(Point2Float))]
public class Point2FloatDrawer : DuongClassDrawer2
{
    public override string[] Displays => new[]
    {
        nameof(Point2Float.x),
        nameof(Point2Float.y),
    };
}

[CustomPropertyDrawer(typeof(Point3Float))]
public class Point3FloatDrawer : DuongClassDrawer2
{
    public override string[] Displays => new[]
    {
        nameof(Point3Float.x),
        nameof(Point3Float.y),
        nameof(Point3Float.z),
    };
}

#endif