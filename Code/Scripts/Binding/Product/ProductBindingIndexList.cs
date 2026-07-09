using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductBindingIndexList : BaseProductBindingToggle
{
    [SerializeField] private Quantity quantity;
    
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
            nameof(BuiltInEnumType.EventEnum),
        },
        nameof(quantity))]
    public int[] indexes;

    
//    [SerializeField] private IndexBindRules indexBindRules; // value = is enable,value 2 is reverse
    
    public override void StartAction(Product p)
    {
        base.StartAction(p);
        bool valid = false;
        if (quantity == p.quantity)
            foreach (var index in indexes)
                if (index == p.index)
                {
                    valid = true;
                    break;
                }
       
        Toggle(valid);
    }
}