using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class QuantitySprite
{
    public Quantity quantity;
    public Sprite sprite;
}

[Serializable]
public class QuantityIndexSprite : QuantitySprite
{
    public int index;
}


[Serializable]
public class QuantityIndexText : QuantityText
{
    public int index;
}

[Serializable]
public class QuantityText
{
    public Quantity quantity;
    public string text;
}


[Serializable]
public class ScriptPropertyFloat
{
    [SerializeField] private ParameterScriptFloat source;
    [SerializeField] private int index;
    [SerializeField] private float value;
    public float Value => source != null ? source.GetValue(index) : value;
}

[Serializable]
public class ScriptPropertyInt
{
    [SerializeField] private ParameterScriptInt source;
    [SerializeField] private int index;
    [SerializeField] private int value;
    public int Value => source != null ? source.GetValue(index) : value;
}


[Serializable]
public struct ProductAction
{
    public int index;
    public int amount;
    public int parameter;
    public int parameter2;

    public static ProductAction FromProduct(Product product)
    {
        return new ProductAction()
        {
            index = product.index,
            amount = product.amount,
            parameter = product.parameter,
            parameter2 = product.parameter2,
        };
    }
}

[System.Serializable]
public class IntFloat
{
    public int value;
    public float value2;

    public IntFloat(int val, float val2)
    {
        value = val;
        value2 = val2;
    }

    public IntFloat()
    {
    }

    public void PlaySound()
    {
        value.PlaySound(value2);
    }
}

[System.Serializable]
public class IntIntPos : IntInt
{
    public Vector2 pos;

    public IntIntPos(int value, int value2, Vector2 pos) : base(value, value2)
    {
        this.pos = pos;
    }

    public IntIntPos Clone2() => new IntIntPos(value, value2, pos);
}

[System.Serializable]
public class IntIntInt : IntInt
{
    public int value3;

    public IntIntInt(int value, int value2, int value3) : base(value, value2)
    {
        this.value3 = value3;
    }

    public IntIntInt CloneCurrent() => new IntIntInt(value, value2, value3);
}


[System.Serializable]
public class IntIntBoolInt : IntInt
{
    public bool value3;
    public int value4;

    public IntIntBoolInt(int value, int value2, bool value3,int value4) : base(value, value2)
    {
        this.value3 = value3;
        this.value4 = value4;
    }

    public IntIntBoolInt CloneCurrent() => new IntIntBoolInt(value, value2, value3,value4);
}



[System.Serializable]
public class IntIntFloat : IntInt
{
    public float value3;

    public IntIntFloat(int value, int value2, float value3) : base(value, value2)
    {
        this.value3 = value3;
    }

    public IntIntFloat(Vector3 vec) : base(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y))
    {
        this.value3 = vec.z;
    }

    public IntIntFloat Clone2() => new IntIntFloat(value, value2, value3);
}

[System.Serializable]
public class IntBoolGameObject
{
    public int value;
    public bool value2;
    public GameObject value3;
}

[System.Serializable]
public class Float6
{
    public float value;
    public float value2;
    public float value3;
    public float value4;
    public float value5;
    public float value6;

    public Float6(float value, float value2, float value3, float value4, float value5, float value6)
    {
        this.value = value;
        this.value2 = value2;
        this.value3 = value3;
        this.value4 = value4;
        this.value5 = value5;
        this.value6 = value6;
    }
    
    public Float6(Transform tf)
    {
        Position = tf.localPosition;
        Euler = tf.localEulerAngles;
    }
    public void Apply(Transform tf)
    {
        tf.localPosition = Position;
        tf.localEulerAngles = Euler;
    }
    
    
    public Vector3 Position
    {
        get => new Vector3(value, value2, value3);
        set
        {
            this.value = value.x;
            value2 = value.y;
            value3 = value.z;
        }
    }

    public Vector3 Euler
    {
        get => new Vector3(value4, value5, value6);
        set
        {
            this.value4 = value.x;
            value5 = value.y;
            value6 = value.z;
        }
    }

   

    public Float6 Clone() => new Float6(value, value2, value3, value4, value5, value6);
}

[System.Serializable]
public class Float5
{
    public float sourceFrom;
    public float sourceTo;
    public float destination;
}

[System.Serializable]
public class IntFloatFloat
{
    public int value;
    public float value2;
    public float value3;

    public IntFloatFloat(int value, float value2, float value3)
    {
        this.value = value;
        this.value2 = value2;
        this.value3 = value3;
    }

    public IntFloatFloat Clone() => new IntFloatFloat(value, value2, value3);
}

[System.Serializable]
public class Int5 : Int4
{
    public int value5;

    public Int5(int value, int value2, int value3, int value4,int value5) : base(value, value2, value3,value4)
    {
        this.value4 = value4;
    }
    
    public Int5(int[] arr) : base(arr[0], arr[1], arr[2],arr[3])
    {
        this.value5 = arr[4];
    }
    public new Int5 CloneCurrent() => new Int5(value, value2, value3, value4,value5);
}

[System.Serializable]
public class Int4 : IntIntInt
{
    public int value4;

    public Int4(int value, int value2, int value3, int value4) : base(value, value2, value3)
    {
        this.value4 = value4;
    }
    
    public Int4(int[] arr) : base(arr[0], arr[1], arr[2])
    {
        this.value4 = arr[3];
    }


    public new Int4 CloneCurrent() => new Int4(value, value2, value3, value4);
    
    
    // progress unlock
    public int Start => value;
    public int End => value2;
    public int From => value3;
    public int To => value4;
    public bool IsProgressValid()
    {
        return End > @From;
    }
}

[System.Serializable]
public class StringIntFloatFloat
{
    public string valueStr;
    public int valueInt;
    public float value1;
    public float value2;

    public float GetRandom() => Random.Range(value1, value2);

    public StringIntFloatFloat(string valueStr, int valueInt, float value1, float value2)
    {
        this.valueStr = valueStr;
        this.valueInt = valueInt;
        this.value1 = value1;
        this.value2 = value2;
    }

    public StringIntFloatFloat CloneCurrent() => new StringIntFloatFloat(valueStr, valueInt, value1, value2);
}

[System.Serializable]
public class IndexActive
{
    public int value;
    public bool reverse;
    public bool bit;
    public GameObject target;

    public virtual void OnChanged(int val)
    {
        if (bit)
        {
            bool hasBit = (value & 1 << val) != 0;
            target.SetActive(reverse ? !hasBit : hasBit);
        }
        else
        {
            #if UNITY_EDITOR
            if(target == null)
                Debug.LogError("target null");
            #endif
            if (reverse)
                target.SetActive(val != value);
            else
                target.SetActive(val == value);
        }
    }
}

[System.Serializable]
public class IndexActiveToggle : IndexActive
{
    public ToggleScript toggleScript;

    public override void OnChanged(int val)
    {
        if (bit)
        {
            bool hasBit = (value & 1 << val) != 0;
            if (target != null)
                target.SetActive(reverse ? !hasBit : hasBit);
            if (toggleScript != null)
                toggleScript.OnChanged(reverse ? !hasBit : hasBit);
        }
        else
        {
            if (target != null)
            {
                if (reverse)
                    target.SetActive(val != value);
                else
                    target.SetActive(val == value);
            }

            if (toggleScript != null)
            {
                if (reverse)
                    toggleScript.OnChanged(val != value);
                else
                    toggleScript.OnChanged(val == value);
            }
        }
    }
}

[System.Serializable]
public class SpeedCurve
{
    public float speedFrom = 1;
    public float speedTo = 2;
    public float timeCurve;
    public AnimationCurve curve;

    public float GetSpeed(float time)
    {
        return Mathf.Lerp(speedFrom, speedTo, curve.Evaluate(time / timeCurve));
    }
}

[System.Serializable]
public class FlexibleCondition
{
    public DataType dataType;
    public string valueString;
    public float valueFloat;
    public float valueFloat2;
    public float valueFloat3;
    public int valueInt;
    public int valueInt2;
    public int valueInt3;
    public bool valueBool;

    public Point Point => new Point(valueInt, valueInt2);
    public Point Point3 => new Point3(valueInt, valueInt2, valueInt3);
    public Vector2 Vector2 => new Vector2(valueFloat, valueFloat2);
    public Vector3 Vector3 => new Vector3(valueFloat, valueFloat2, valueFloat3);
}

[System.Serializable]
public class IntActionScript
{
    public int value;
    public BaseBaseActionScript value2;
}

[System.Serializable]
public class PairGameObject
{
    public GameObject GameObject1;
    public GameObject GameObject2;

    public (T, T) Cast<T>()
    {
        return (GameObject1.GetComponent<T>(), GameObject2.GetComponent<T>());
    }

    public PairGameObject(GameObject go1, GameObject go2)
    {
        GameObject1 = go1;
        GameObject2 = go2;
    }
}


[System.Serializable]
public class IntInt
{
    public int value;
    public int value2;

    public IntInt(int value, int value2)
    {
        this.value = value;
        this.value2 = value2;
    }

    public IntInt()
    {
    }

    public int ToSingleInt(int mod = 1000000)
    {
        return value * mod + value2;
    }

    public override bool Equals(object other)
    {
        return other is IntInt otherKey && ((value == otherKey.value
                                             && value2 == otherKey.value2
                   ));
    }

    public static bool operator ==(IntInt emp1, IntInt emp2)
    {
        if (ReferenceEquals(emp1, emp2)) return true;
        if (ReferenceEquals(emp1, null)) return false;
        if (ReferenceEquals(emp2, null)) return false;

        return emp1.Equals(emp2);
    }

    public static bool operator !=(IntInt emp1, IntInt emp2)
    {
        if (ReferenceEquals(emp1, emp2)) return false;
        if (ReferenceEquals(emp1, null)) return true;
        if (ReferenceEquals(emp2, null)) return true;

        return !emp1.Equals(emp2);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public string GetString()
    {
        return value + "_" + value2;
    }

    public IntInt Clone() => new IntInt(value, value2);

    public bool InRange(in int computedValue)
    {
        return computedValue >= value && computedValue < value2;
    }

    public bool IsZero()
    {
        return value == 0 && value2 == 0;
    }

    public bool IsEqualAboveZero()
    {
        return value >= 0 && value2 >= 0;
    }
}

[System.Serializable]
public class IntTransform
{
    public int value;
    public Transform value2;
}


public class EItemKeyEqualityComparer : IEqualityComparer<IntInt>
{
    #region IEqualityComparer<Customer> Members

    public bool Equals(IntInt x, IntInt y)
    {
        return x == y;
    }

    public int GetHashCode(IntInt obj)
    {
        string combined = obj.value + "|" + obj.value2;
        return (combined.GetHashCode());
    }

    #endregion
}

[System.Serializable]
public class ToggleAction
{
    public ToggleScript script;
    public bool value;


    public void Invoke()
    {
        script.OnChanged(value);
    }
}


[System.Serializable]
public class IndexAction
{
    public IndexBindingScriptBase script;
    public int value;

    public void Invoke()
    {
        script.OnChanged(value);
    }
}


[System.Serializable]
public class BoolBool
{
    public bool value;
    public bool value2;

    public BoolBool(bool value, bool value2)
    {
        this.value = value;
        this.value2 = value2;
    }
}


[System.Serializable]
public class BoolFloat
{
    public bool value;
    [FormerlySerializedAs("valueFloat")] public float value2;

    public BoolFloat(bool value, float value2)
    {
        this.value = value;
        this.value2 = value2;
    }
}


[System.Serializable]
public class BoolFloatFloat : BoolFloat
{
   public float value3;

    public BoolFloatFloat(bool value, float value2, float value3) : base(value,value2)
    {
        this.value3 = value3;
    }
}



[System.Serializable]
public class Bool3
{
    public bool value1;
    public bool value2;
    public bool value3;

    public Bool3(bool value1, bool value2, bool value3)
    {
        this.value1 = value1;
        this.value2 = value2;
        this.value3 = value3;
    }
}


[System.Serializable]
public class IntBool
{
    public int value;
    public bool value2;

    public IntBool(int value, bool value2)
    {
        this.value = value;
        this.value2 = value2;
    }
}


[System.Serializable]
public class StringBool
{
    public string value;
    public bool value2;

    public StringBool(string value, bool value2)
    {
        this.value = value;
        this.value2 = value2;
    }
}


//[System.Serializable]
//public class IntBoolWhen
//{
//    public int value;
//    public bool value2;
//    public PropertyBindTime value3;
//
//    public IntBoolWhen(int value,bool value2,PropertyBindTime when)
//    {
//        this.value = value;
//        this.value2 = value2;
//        this.value3 = when;
//    }
//}

public enum BindWhen
{
    OnEnable,
    AlwaysSubcribe
}


[System.Serializable]
public struct IntString
{
    [FormerlySerializedAs("value1")] // do not removeeeeeeeee
    public int value;
    public string value2;
}


[Serializable]
public class IntArrayInfinitive
{
    [SerializeField] private int[] values;

    public int this[int key]
    {
        get
        {
            if (key < values.Length)
                return values[key];
            var sub = key - values.Length;
            var lastChange = values[values.Length - 1] - values[values.Length - 2];
            return values[values.Length - 1] + sub * lastChange;
        }
    }

    public int AmountToLevel(int level)
    {
        int sum = 0;
        for (int i = 0; i < level; i++)
            sum += this[i];

        return sum;
    }
}


[System.Serializable]
public class ListProduct
{
    public Product[] products;
}


[System.Serializable]
public class ProductValue : Product
{
    public int value1;
    public int value2;
    public int value3;

    public bool IsProgressValid()
    {
        throw new NotImplementedException();
    }
}


[System.Serializable]
public class ProductUnlock : Product
{
    public UnlockEquipConditionEnum condition;

    public bool Equal(ProductUnlock productUnlock)
    {
        return productUnlock.condition == condition
               && productUnlock.Quantity == Quantity
               && productUnlock.index == index
               && productUnlock.amount == amount
               && productUnlock.parameter == parameter
               && productUnlock.parameter2 == parameter2
            ;
    }
}

[System.Serializable]
public class ProductSprite : Product
{
    public Sprite sprite;
    public string text;
}


[System.Serializable]
public class ProductAssetBind : Product
{
    public bool includeParameter;
}


[System.Serializable]
public class ProductBind : Product
{
    public bool bindQuantity;
    public bool bindIndex;
    public bool bindParameter;

    public bool Valid(Product product)
    {
        return (!bindQuantity || quantity == product.quantity)
               && (!bindIndex || index == product.index)
               && (!bindParameter || parameter == product.parameter);
    }
}


[System.Serializable]
public class Product
{
    [FormerlySerializedAs("Quantity")] public Quantity quantity;
    [FormerlySerializedAs("Amount")] public int amount;
    [FormerlySerializedAs("Index")] public int parameter;
    public int parameter2;

    public bool IsActive
    {
        get => parameter2 == 1;
        set => parameter2 = value ? 1 : 0;
    }

    [IndexAsEnumMultiple(new object[]
        {
            Quantity.SystemAction,
            Quantity.GameAction,
            Quantity.SystemGameAction,
            Quantity.Item,
            Quantity.CustomProperty,
            Quantity.CustomDailyProperty,
            Quantity.CustomTimeTick,
            Quantity.EquipData,
            Quantity.Achievement,
            Quantity.Quest,
            Quantity.Subscription,
            Quantity.TutorialSkip,
            Quantity.CustomProduct,
            Quantity.Random,
            Quantity.IapBuying,
            Quantity.EventItem,
        },
        new[]
        {
            nameof(BuiltInEnumType.ProductSystemAction),
            nameof(BuiltInEnumType.ProductGameAction),
            nameof(GameActionEvent),
            nameof(BuiltInEnumType.ItemTypeEnum),
            nameof(BuiltInEnumType.CustomPlayerDataProperty),
            nameof(BuiltInEnumType.CustomDailyDataProperty),
            nameof(BuiltInEnumType.CustomTimeTick),
            nameof(BuiltInEnumType.UnlockEquipDataEnum),
            nameof(BuiltInEnumType.AchievementEnum),
            nameof(BuiltInEnumType.QuestEnum),
            nameof(BuiltInEnumType.SubscriptionKey),
            nameof(BuiltInEnumType.TutorialSkipType),
            nameof(BuiltInEnumType.CustomProductEnum),
            nameof(BuiltInEnumType.RandomItem),
            nameof(BuiltInEnumType.ShopItemKey),
            nameof(BuiltInEnumType.EventEnum),
        },
        nameof(quantity))]
    public int index;


    public Quantity Quantity
    {
        get => quantity;
        set => quantity = value;
    }

    public int Amount
    {
        get => amount;
        set => amount = value;
    }


    public Product(Quantity q, int amount)
    {
        Quantity = q;
        this.amount = amount;
    }

    public Product(Quantity q, int amount, int i)
    {
        Quantity = q;
        this.amount = amount;
        index = i;
    }

    public Product AlterAmount(int value)
    {
        return new Product(quantity, value, index, parameter, parameter2);
    }


    public Product(Quantity quantity, int amount, int index, int param1, int param2)
    {
        this.quantity = quantity;
        this.amount = amount;
        this.index = index;
        parameter = param1;
        parameter2 = param2;
    }

    public Product()
    {
    }

//    public Product(Quantity quantity, int amount, int index)
//    {
//        this.quantity = quantity;
//        this.amount = amount;
//        this.index = index;
//    }

    public static Product Free => new Product(Quantity.Free, 0, 0, 0, 0);

    public Product Multiple(int multiple)
    {
        return new Product(quantity, amount * multiple, index, parameter, parameter2);
    }

    public Product Multiple(float multiple)
    {
        return new Product(quantity, (int) (amount * multiple), index, parameter, parameter2);
    }

    public Product Clone()
    {
        return new Product(quantity, amount, index, parameter, parameter2);
    }

    public Product Clone(int newAmount)
    {
        return new Product(quantity, newAmount, index, parameter, parameter2);
    }


    public Product ReverseQuantity()
    {
        return new Product(quantity, -amount, index, parameter, parameter2);
    }

    public ClaimedProduct CloneAsClaimed()
    {
        return new ClaimedProduct(quantity, amount, index, parameter, parameter2);
    }


    public ClaimedProduct ConvertRandom()
    {
        var clone = new ClaimedProduct(quantity, amount, index, parameter, parameter2);

        return clone;
    }

    public void MultipleReward(int multiple, bool claim = true)
    {
//        var origin = Amount;
//        Amount = origin * (multiple - 1);
//        if (claim)
//            GameView.PlayerData.Claim(this);
//        Amount = origin * multiple;
    }

    public ShopPrice ToShopPrice(ShopStock pricePriceKey)
    {
        return new ShopPrice()
        {
            amount = amount,
            index = index,
            quantity = quantity,
            parameter = parameter,
            parameter2 = parameter2,
            priceKey = pricePriceKey
        };
    }

    public string GetString()
    {
        return $"{quantity}-{index}-{amount}-{parameter}-{parameter2}";
    }

    public static Product CustomQuantity(int index)
    {
       return new Product(Quantity.CustomQuantity,1,index);
    }
}

[System.Serializable]
public class ClaimedProduct : Product
{
    public ClaimedProduct()
    {
    }

    public ClaimedProduct(Quantity quantity, int amount, int index, int param1, int param2) : base(quantity, amount,
        index, param1, param2)
    {
    }
}