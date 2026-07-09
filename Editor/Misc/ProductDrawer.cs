using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


public static class TypeExtension
{
    public static System.Reflection.FieldInfo GetFieldViaPath(this System.Type type,
        string path)
    {
        var parent = type;
        var fi = parent.GetField(path);
        var paths = path.Split('.');

        for (int i = 0; i < paths.Length; i++)
        {
            fi = parent.GetField(paths[i]);
            // you can compare the name first to reduce the cost but it doesn't help a lot
            // if (i + 1 < paths.Length && paths[i + 1].CompareTo ("Array") == 0) 

            // there are only two container field type that can be serialized:
            // Array and List<T>
            if (fi.FieldType.IsArray)
            {
                parent = fi.FieldType.GetElementType();
                i += 2;
                continue;
            }

            if (fi.FieldType.IsGenericType)
            {
                parent = fi.FieldType.GetGenericArguments()[0];
                i += 2;
                continue;
            }

            if (fi != null)
            {
                parent = fi.FieldType;
            }
            else
            {
                return null;
            }
        }

        return fi;
    }

    static public SerializedProperty FindPropertyRelativeFix(this SerializedProperty sp, string name,
        ref SerializedObject objectToApplyChanges)
    {
        SerializedProperty result;
        if (typeof(ScriptableObject).IsAssignableFrom(sp.GetFieldType()))
        {
            if (sp.objectReferenceValue == null) return null;
            if (objectToApplyChanges == null)
                objectToApplyChanges = new SerializedObject(sp.objectReferenceValue);
            result = objectToApplyChanges.FindProperty(name);
        }
        else
        {
            objectToApplyChanges = null;
            result = sp.FindPropertyRelative(name);
        }

        return result;
    }

    static public System.Type GetFieldType(this SerializedProperty property)
    {
        if (property.serializedObject.targetObject == null) return null;
        System.Type parentType = property.serializedObject.targetObject.GetType();
        System.Reflection.FieldInfo fi = parentType.GetFieldViaPath(property.propertyPath);
        string path = property.propertyPath;
        if (fi == null)
            return null;

        return fi.FieldType;
    }
}

[CustomPropertyDrawer(typeof(PopUpAttribute))]
public class PopUpPropertyDrawer : PropertyDrawer
{
    private int _index = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        PopUpAttribute items = attribute as PopUpAttribute;

        // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
        if (property.propertyType == SerializedPropertyType.String)
        {
            var prevIndex = _index;
            _index = EditorGUI.Popup(position, property.displayName, _index, items.items);
            if (prevIndex != _index)
            {
                property.stringValue = items.items[_index];
            }
        }
        else
            EditorGUI.LabelField(position, label.text, "Use PopUp for strings.");
    }
}


[CustomPropertyDrawer(typeof(ArrayLabelSquare2Attribute))]
public class ArrayLabelSquare2AttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayLabelSquare2Attribute Atribute
    {
        get { return (ArrayLabelSquare2Attribute) attribute; }
    }

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        var index = Int32.Parse(Regex.Match(label.text, @"\d+").Value);
        string newLabel = Atribute.VarName + index + ": " + Mathf.Pow(2, index + 1);
        if (string.IsNullOrEmpty(newLabel))
            newLabel = label.text;
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true);
    }
}


[CustomPropertyDrawer(typeof(ArrayLabelIndexAsEnumAttribute))]
public class ArrayLabelIndexAsEnumAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayLabelIndexAsEnumAttribute Atribute
    {
        get { return (ArrayLabelIndexAsEnumAttribute) attribute; }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var index = Int32.Parse(Regex.Match(label.text, @"\d+").Value);
        if (Atribute.EnumList.Found)
        {
            var _index = -1;
            for (var i = 0; i < Atribute.EnumList.Values.Length; i++)
                if (index == Atribute.EnumList.Values[i])
                {
                    _index = i;
                    break;
                }

            string newLabel = Atribute.EnumList.Names.Count > _index && _index >= 0
                ? Atribute.EnumList.Names[_index].text
                : Atribute.EnumList.TypeName + index + "(notFound)";
            if (string.IsNullOrEmpty(newLabel))
                newLabel = label.text;
            EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true);
        }
        else
        {
            label.text = "notFound " + Atribute.EnumList.TypeName + " " + index;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}


[CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
public class ArrayElementTitleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayElementTitleAttribute Atribute
    {
        get { return (ArrayElementTitleAttribute) attribute; }
    }

    SerializedProperty _titleNameProp;

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        string fullPathName = property.propertyPath + "." + Atribute.varName;
        _titleNameProp = property.serializedObject.FindProperty(fullPathName);
        string newlabel = GetTitle();
        if (string.IsNullOrEmpty(newlabel))
            newlabel = label.text;
        EditorGUI.PropertyField(position, property, new GUIContent(newlabel, label.tooltip), true);
    }

    private string GetTitle()
    {
        switch (_titleNameProp.propertyType)
        {
            case SerializedPropertyType.Generic:
                break;
            case SerializedPropertyType.Integer:
                return _titleNameProp.intValue.ToString();
            case SerializedPropertyType.Boolean:
                return _titleNameProp.boolValue.ToString();
            case SerializedPropertyType.Float:
                return _titleNameProp.floatValue.ToString();
            case SerializedPropertyType.String:
                return _titleNameProp.stringValue;
            case SerializedPropertyType.Color:
                return _titleNameProp.colorValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return _titleNameProp.objectReferenceValue.ToString();
            case SerializedPropertyType.LayerMask:
                break;
            case SerializedPropertyType.Enum:
                if (_titleNameProp.enumNames.Length <= _titleNameProp.enumValueIndex || _titleNameProp.enumValueIndex < 0)
                    return "out of range";
                return _titleNameProp.enumNames[_titleNameProp.enumValueIndex];
            case SerializedPropertyType.Vector2:
                return _titleNameProp.vector2Value.ToString();
            case SerializedPropertyType.Vector3:
                return _titleNameProp.vector3Value.ToString();
            case SerializedPropertyType.Vector4:
                return _titleNameProp.vector4Value.ToString();
            case SerializedPropertyType.Rect:
                break;
            case SerializedPropertyType.ArraySize:
                break;
            case SerializedPropertyType.Character:
                break;
            case SerializedPropertyType.AnimationCurve:
                break;
            case SerializedPropertyType.Bounds:
                break;
            case SerializedPropertyType.Gradient:
                break;
            case SerializedPropertyType.Quaternion:
                break;
            default:
                break;
        }

        return "";
    }
}

[CustomPropertyDrawer(typeof(ArrayElementIndexTitleAttribute2))]
public class ArrayElementIndexTitleAttributeDrawer2 : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayElementIndexTitleAttribute2 Attribute => (ArrayElementIndexTitleAttribute2) attribute;

    SerializedProperty _titleNameProp;

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        if (Attribute.enumList.Found)
        {
            var index = Int32.Parse(Regex.Match(label.text, @"\d+").Value);
            for (var i = 0; i < Attribute.enumList.Values.Length; i++)
            {
                if (Attribute.enumList.Values[i] == index)
                {
                    label.text = Attribute.enumList.Names[i].text;
                }
            }
        }
        
        EditorGUI.PropertyField(position, property, label, true);
    }
}


[CustomPropertyDrawer(typeof(ArrayElementIndexTitleAttribute))]
public class ArrayElementIndexTitleAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayElementIndexTitleAttribute Attribute => (ArrayElementIndexTitleAttribute) attribute;

    SerializedProperty _titleNameProp;

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        string fullPathName = property.propertyPath + "." + Attribute.KeyVarName;
        _titleNameProp = property.serializedObject.FindProperty(fullPathName);
        if (_titleNameProp == null)
        {
            EditorGUI.PropertyField(position, property, new GUIContent("not found "+fullPathName, label.tooltip), true);
            return;
        }

        string newLabel = GetTitle();
        if (string.IsNullOrEmpty(newLabel))
            newLabel = label.text;
        if (Attribute.IndexVarName != null)
        {
            var indexProp = property.serializedObject.FindProperty(property.propertyPath + "." + Attribute.IndexVarName);
            newLabel += "_" + indexProp.intValue;
        }
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true);
    }

    

    private string GetTitle()
    {
        switch (_titleNameProp.propertyType)
        {
            case SerializedPropertyType.Generic:
                break;
            case SerializedPropertyType.Integer:
                if (Attribute.EnumList == null)
                {
                    return "NotFound";
                }

                return Attribute.EnumList.GetName(_titleNameProp.intValue).text;
            case SerializedPropertyType.Boolean:
                return _titleNameProp.boolValue.ToString();
            case SerializedPropertyType.Float:
                return _titleNameProp.floatValue.ToString();
            case SerializedPropertyType.String:
                return _titleNameProp.stringValue;
            case SerializedPropertyType.Color:
                return _titleNameProp.colorValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return _titleNameProp.objectReferenceValue.ToString();
            case SerializedPropertyType.LayerMask:
                break;
            case SerializedPropertyType.Enum:
                return _titleNameProp.enumNames[_titleNameProp.enumValueIndex];
            case SerializedPropertyType.Vector2:
                return _titleNameProp.vector2Value.ToString();
            case SerializedPropertyType.Vector3:
                return _titleNameProp.vector3Value.ToString();
            case SerializedPropertyType.Vector4:
                return _titleNameProp.vector4Value.ToString();
            case SerializedPropertyType.Rect:
                break;
            case SerializedPropertyType.ArraySize:
                break;
            case SerializedPropertyType.Character:
                break;
            case SerializedPropertyType.AnimationCurve:
                break;
            case SerializedPropertyType.Bounds:
                break;
            case SerializedPropertyType.Gradient:
                break;
            case SerializedPropertyType.Quaternion:
                break;
            default:
                break;
        }

        return "";
    }
}


[CustomPropertyDrawer(typeof(ArrayElementTitleByNameAttribute))]
public class ArrayElementTitleByNameAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    protected virtual ArrayElementTitleByNameAttribute Attribute => (ArrayElementTitleByNameAttribute) attribute;

    SerializedProperty _titleNameProp;

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        string fullPathName = property.propertyPath + "." + Attribute.KeyVarName;
        _titleNameProp = property.serializedObject.FindProperty(fullPathName);
        if (_titleNameProp == null)
        {
            EditorGUI.PropertyField(position, property, new GUIContent("not found "+fullPathName, label.tooltip), true);
            return;
        }

        var reference = _titleNameProp.objectReferenceValue;
        var index = Int32.Parse(Regex.Match(label.text, @"\d+").Value);
        string newLabel = reference != null ? index+"_"+reference.name : index+"_null";
        if (string.IsNullOrEmpty(newLabel))
            newLabel = label.text;
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel, label.tooltip), true);
    }
}


// IngredientDrawer
[CustomPropertyDrawer(typeof(DailyCustomDataSetting))]
public class DailyCustomDataSettingDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(DailyCustomDataSetting.key), "", 32),
        new DivideData(20, nameof(DailyCustomDataSetting.amount), "amount", 42)
    };

    protected override bool HideLabel => true;
}


// IngredientDrawer
[CustomPropertyDrawer(typeof(SortingOrderItem))]
public class SortingOrderItemDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(10, nameof(SortingOrderItem.key)),
        new DivideData(10, nameof(SortingOrderItem.layer), "layer", 32),
        new DivideData(10, nameof(SortingOrderItem.order), "order", 32),
    };

    protected override bool HideLabel => true;
}

// IngredientDrawer
[CustomPropertyDrawer(typeof(Product))]
public class ProductDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(Product.quantity)),
        new DivideData(15, nameof(Product.index), "index", 32),
        new DivideData(10, nameof(Product.amount), "a", 10),
        new DivideData(5, nameof(Product.parameter)),
        new DivideData(5, nameof(Product.parameter2))
    };

    protected override bool HideLabel => true;
}

[CustomPropertyDrawer(typeof(SoundDataItem))]
public class SoundDataItemDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(7, nameof(SoundDataItem.key)),
        new DivideData(7, nameof(SoundDataItem.clip)),
        new DivideData(5, nameof(SoundDataItem.volume), "volume", 40),
        new DivideData(5, nameof(SoundDataItem.delay), "delay", 40),
    };

    protected override bool HideLabel => true;
}

[CustomPropertyDrawer(typeof(QuantityShopItemPair))]
public class QuantityShopItemPairDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(QuantityShopItemPair.quantity)),
        new DivideData(20, nameof(QuantityShopItemPair.shopItemKey)),
    };

    protected override bool HideLabel => true;
}

[CustomPropertyDrawer(typeof(ProductUnlock))]
public class ProductUnlockDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(ProductUnlock.condition)),
        new DivideData(20, nameof(Product.quantity)),
        new DivideData(15, nameof(Product.index), "index", 32),
        new DivideData(10, nameof(Product.amount), "a", 10),
        new DivideData(5, nameof(Product.parameter)),
        new DivideData(5, nameof(Product.parameter2))
    };

    protected override bool HideLabel => true;
}


[CustomPropertyDrawer(typeof(ProductSprite))]
 public class ProductSpriteDrawer : DuongClassDrawer
 {
     public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
     {
         new DivideData(20, nameof(Product.quantity)),
         new DivideData(15, nameof(Product.index), "index", 32),
         new DivideData(5, nameof(Product.parameter)),
         new DivideData(10, nameof(ProductSprite.sprite)),
         new DivideData(10, nameof(ProductSprite.text)),
 
     };
 
     protected override bool HideLabel => true;
 }

[CustomPropertyDrawer(typeof(ProductValue))]
public class ProductValueDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(Product.quantity)),
        new DivideData(15, nameof(Product.index), "i", 10),
        new DivideData(5, nameof(Product.amount), "a", 10),
        new DivideData(5, nameof(Product.parameter)),
        new DivideData(5, nameof(Product.parameter2)),
        new DivideData(5, nameof(ProductValue.value1), "val", 20),
        new DivideData(5, nameof(ProductValue.value2)),
        new DivideData(5, nameof(ProductValue.value3)),
 
    };
    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(IntValueRetrieverOrDefault))]
public class IntValueRetrieverOrDefaultDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(IntValueRetrieverOrDefault.defaultValue)),
        new DivideData(20, nameof(IntValueRetrieverOrDefault.retriever)),
    };
    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(IntIntSetActive))]
public class IntIntSetActiveDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, nameof(IntIntSetActive.value)),
        new DivideData(5, nameof(IntIntSetActive.value2)),
        new DivideData(5, nameof(IntIntSetActive.reverse)),
        new DivideData(10, nameof(IntIntSetActive.go)),
        new DivideData(10, nameof(IntIntSetActive.toggle)),

    };

    protected override bool HideLabel => true;
}


[CustomPropertyDrawer(typeof(ProductBind))]
public class ProductBindDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(6, nameof(ProductBind.bindQuantity)),
        new DivideData(20, nameof(ProductBind.quantity)),
        new DivideData(6, nameof(ProductBind.bindIndex)),
        new DivideData(20, nameof(ProductBind.index), "i", 10),
        new DivideData(6, nameof(ProductBind.bindParameter)),
        new DivideData(20, nameof(ProductBind.parameter), "p", 10),

    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(ProductAssetBind))]
public class ProductAssetBindDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(ProductAssetBind.quantity)),
        new DivideData(6, nameof(ProductAssetBind.includeParameter), " ", 2),
        new DivideData(20, nameof(ProductAssetBind.index), "i", 10),
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(BoolBool))]
public class BoolBoolDrawer : DuongClassDrawer
{
    
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, nameof(BoolBool.value)),
        new DivideData(5, nameof(BoolBool.value2))
    };

    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(Bool3))]
public class Bool3Drawer : DuongClassDrawer
{
    
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, nameof(Bool3.value1)),
        new DivideData(5, nameof(Bool3.value2)),
        new DivideData(5, nameof(Bool3.value3))
    };

    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(ToggleAction))]
public class ToggleActionDrawer : DuongClassDrawer
{
    
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, nameof(ToggleAction.value)),
        new DivideData(5, nameof(ToggleAction.script))
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(IndexAction))]
public class IndexActionDrawer : DuongClassDrawer
{
    
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, nameof(IndexAction.value)),
        new DivideData(5, nameof(IndexAction.script))
    };

    protected override bool HideLabel => false;
}


//[CustomPropertyDrawer(typeof(PlayerPrefRemoteBool))]
//public class PlayerPrefDrawer : DuongClassDrawer
//{
//    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData> {new DivideData(20, "value")};
//    protected override bool HideLabel => false;
//}

// IngredientDrawer
[CustomPropertyDrawer(typeof(IndexScriptTrigger))]
public class IndexScriptTriggerDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(15, nameof(IndexScriptTrigger.script), "script", 30),
        new DivideData(10, nameof(IndexScriptTrigger.value), "value", 30)
    };

    protected override bool HideLabel => true;
}

// IngredientDrawer
[CustomPropertyDrawer(typeof(SoundItem))]
public class SoundItemDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, nameof(SoundItem.key), " ", 1),
        new DivideData(3, nameof(SoundItem.audioClip)),
        new DivideData(8, nameof(SoundItem.volume), " ", 1)
    };

    protected override bool HideLabel => true;
}

[CustomPropertyDrawer(typeof(ScriptPropertyFloat))]
public class ScriptPropertyFloatDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, "source", " ", 1),
        new DivideData(1, "index"),
        new DivideData(2, "value", " ", 1)
    };

    protected override bool HideLabel => false;
}

[CustomPropertyDrawer(typeof(ScriptPropertyInt))]
public class ScriptPropertyIntDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, "source", " ", 1),
        new DivideData(1, "index"),
        new DivideData(2, "value", " ", 1)
    };

    protected override bool HideLabel => false;
}

// IngredientDrawer
[CustomPropertyDrawer(typeof(MusicItem))]
public class MusicItemDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, nameof(MusicItem.key), " ", 1),
        new DivideData(3, nameof(MusicItem.audioClip)),
        new DivideData(8, nameof(MusicItem.volume), " ", 1)
    };

    protected override bool HideLabel => true;
}

[CustomPropertyDrawer(typeof(ShopStock))]
public class ShopPriceKeyDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(ShopStock.key), "", 1),
        new DivideData(20, nameof(ShopStock.stockType)),
        new DivideData(10, nameof(ShopStock.stock))
    };
}


[CustomPropertyDrawer(typeof(ActiveToggleScript))]
public class ActiveToggleScriptDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(4, nameof(ActiveToggleScript.active)),
        new DivideData(4, nameof(ActiveToggleScript.twoWays)),
        new DivideData(20, nameof(ActiveToggleScript.script)),
    };
}

[CustomPropertyDrawer(typeof(ActiveGameObject))]
public class ActiveGameObjectDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(4, nameof(ActiveGameObject.active)),
        new DivideData(6, nameof(ActiveGameObject.twoWays),"2w",24f),
        new DivideData(20, nameof(ActiveGameObject.go)),
    };
}

[CustomPropertyDrawer(typeof(ActiveResetRx))]
public class ActiveResetRxDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(4, nameof(ActiveResetRx.active)),
        new DivideData(4, nameof(ActiveResetRx.twoWays)),
        new DivideData(20, nameof(ActiveResetRx.go)),
    };
}

[CustomPropertyDrawer(typeof(ActiveSetAtInRx))]
public class ActiveSetAtInRxDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(4, nameof(ActiveSetAtInRx.active)),
        new DivideData(4, nameof(ActiveSetAtInRx.twoWays)),
        new DivideData(20, nameof(ActiveSetAtInRx.go)),
    };
}


[CustomPropertyDrawer(typeof(ActiveEnableComponent))]
public class ActiveEnableComponentDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(4, nameof(ActiveEnableComponent.active)),
        new DivideData(4, nameof(ActiveEnableComponent.twoWays)),
        new DivideData(20, nameof(ActiveEnableComponent.mono)),
    };
}


[CustomPropertyDrawer(typeof(ActiveIndexScript))]
public class ActiveIndexScriptDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(5, nameof(ActiveIndexScript.active)),
        new DivideData(20, nameof(ActiveIndexScript.script)),
    };
}


[CustomPropertyDrawer(typeof(EffectPoolDescriptor))]
public class EffectPoolDescriptorDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(20, nameof(EffectPoolDescriptor.key), "", 1),
        new DivideData(20, nameof(EffectPoolDescriptor.prefab)),
        new DivideData(5, nameof(EffectPoolDescriptor.prePool)),
        new DivideData(5, nameof(EffectPoolDescriptor.maxSize))
    };

    protected override bool HideLabel => false;
}


[CustomPropertyDrawer(typeof(TestFunction))]
public class TestFunctionDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => null;
    protected override bool HideLabel => true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUIUtility.labelWidth = 80 + EditorGUI.indentLevel * 20;
        label = EditorGUI.BeginProperty(position, label, property);
        Init(position, property, label);

        var lastIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var target = property.serializedObject.targetObject;
        var type = target.GetType();
        var methodName = property.FindPropertyRelative(nameof(TestFunction.method)).stringValue;

        System.Reflection.MethodInfo method = null;

      
        try
        {
            method = type.GetMethod(methodName);
        }
        catch (Exception e)
        {
          //  Debug.LogError(e);
        }

        if (method == null)
        {
            method =
                type.GetMethod(
                    methodName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, 
                    Type.DefaultBinder,
                    new[] { typeof (int) },
                    null);
        }
        
        if (method == null)
        {
            method =
                type.GetMethod(
                    methodName,
                    BindingFlags.Instance | BindingFlags.NonPublic| BindingFlags.Public, 
                    Type.DefaultBinder,
                    new[] {  typeof(bool) },
                    null);
        }

        
        
        if (method == null)
        {
            method =
                type.GetMethod(
                    methodName,
                    BindingFlags.Instance | BindingFlags.NonPublic| BindingFlags.Public, 
                    Type.DefaultBinder,
                    new Type[]  {  },
                    null);
        }

        int count = method == null ? 0 : method.GetParameters().Length;

        List<DivideData> data2 = new List<DivideData>();


        data2.Add(new DivideData(15, nameof(TestFunction.method)));
        if (count >= 1)
            data2.Add(new DivideData(5, nameof(TestFunction.parameter1), "p1", 10));
        if (count >= 2)
            data2.Add(new DivideData(5, nameof(TestFunction.parameter2), "p2", 10));
        SetUp(data2, TotalWidthPixel, 10);


        void SetUp(IList<DivideData> data, float totalPixel, int buttonWeight)
        {
            int total = data.Where(x => x.Weight > 0).Sum(x => x.Weight) + buttonWeight;
            var sumPixel = Mathf.Abs(data.Where(x => x.Weight < 0).Sum(x => x.Weight));
            var remain = totalPixel - sumPixel; //remain after label
            int c = -1;
            foreach (var divideData in data)
            {
                GUI.enabled = c < count;
                DividedFieldPixel(divideData,divideData.Weight > 0 ? remain * divideData.Weight / total : Mathf.Abs(divideData.Weight));
                c++;
            }

            GUI.enabled = true;
            DividedButton(remain * buttonWeight / total);
        }

        void DividedButton(float width)
        {
            var rect = new Rect(WholeRect.x + PartialSum, WholeRect.y, width - 1, WholeRect.height);
            PartialSum += width;
            if (method == null)
            {
                GUI.enabled = false;
                if (GUI.Button(rect, "Invoke"))
                {
//                method.Invoke(target, new object[] {});
                }

                GUI.enabled = true;

                return;
            }

            if (GUI.Button(rect, "Invoke"))
            {
                if (method.GetParameters().Length == 0)
                {
                    method.Invoke(target, null);
                }

                if (method.GetParameters().Length == 1)
                {
                    if (method.GetParameters()[0].ParameterType == typeof(int))
                        method.Invoke(target, new object[]
                        {
                            property.FindPropertyRelative(nameof(TestFunction.parameter1)).intValue,
                        });
                    else if (method.GetParameters()[0].ParameterType == typeof(bool))
                        method.Invoke(target, new object[]
                        {
                            property.FindPropertyRelative(nameof(TestFunction.parameter1)).intValue > 0,
                        });
                    else if (method.GetParameters()[0].ParameterType == typeof(float))
                        method.Invoke(target, new object[]
                        {
                            property.FindPropertyRelative(nameof(TestFunction.parameter1)).intValue / 100f,
                        });
                }

                if (method.GetParameters().Length == 2)
                {
                    method.Invoke(target, new object[]
                    {
                        property.FindPropertyRelative(nameof(TestFunction.parameter1)).intValue,
                        property.FindPropertyRelative(nameof(TestFunction.parameter2)).intValue,
                    });
                }
            }
        }

        EditorGUI.indentLevel = lastIndentLevel;
        EditorGUI.EndProperty();
    }
}