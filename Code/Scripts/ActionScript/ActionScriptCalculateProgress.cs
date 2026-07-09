using System;
using System.Linq;
using UnityEngine.Serialization;

namespace CarJam
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    
    
    public class ActionScriptCalculateProgress : BaseActionScript
    {
        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)]
        [SerializeField] protected int start;
        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)]
        [SerializeField] protected int end;
        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)]
        [SerializeField] protected int from;
        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)]
        [SerializeField] protected int to;

        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)] [SerializeField]
        protected int quantity = (int) BaseFrameworkSharedData.ProductQuantity;
        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)] [SerializeField]
        protected int index = (int) BaseFrameworkSharedData.ProductIndex;
        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)] [SerializeField]
        protected int amount = (int) BaseFrameworkSharedData.ProductAmount;
        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)] [SerializeField]
        protected int parameter = (int) BaseFrameworkSharedData.ProductParameter1;
        [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)] [SerializeField]
        protected int parameter2 = (int) BaseFrameworkSharedData.ProductParameter2;


        //  key1 key2,progress from, progress to,
        [SerializeField] private ProductValue[] database;
        [SerializeField] private ProductValue defaultWhenOutRange;
       
//        [SerializeField] private UnlockEquipManagerDependency[] source;
        [FormerlySerializedAs("retriever")] 
        [SerializeField] private IntValueRetriever fromTo;

//        public override void LogCustom()
//        {
//            base.LogCustom();
//            List<ProductValue> temp = new List<ProductValue>();
//            foreach (var unlockEquipManagerDependency in source)
//            {
//                var conditions = unlockEquipManagerDependency.Conditions.ToArray();
//                for (var i = 0; i < conditions.Length; i++)
//                {
//                    var productUnlock = conditions[i];
//                    if (productUnlock.condition == UnlockEquipConditionEnum.FinishLevel)
//                        temp.Add(new ProductValue()
//                        {
//                            quantity = Quantity.EquipData,
//                            index = unlockEquipManagerDependency.UnlockEquipDataEnum,
//                            amount = 1,
//                            parameter = i,
//                            value1 = 0,
//                            value2 = productUnlock.amount,
//                        });
//                }
//            }
//            
//            database = temp.OrderBy(x => x.value2).ToArray();
//            for (var i = 1; i < database.Length; i++)
//            {
//                database[i].value1 = database[i - 1].value2;
//            }
//        }

        protected override void DoAction()
        {
            base.DoAction();
            var valueFrom = fromTo.Values[0];
            var valueTo = fromTo.Values[1];
            var current = GetCurrent(valueTo);
            
            SharedData[@from] = valueFrom;
            SharedData[@to] = valueTo;
            if (valueFrom < current.value2 && valueFrom >= current.value1)
            {
                SharedData[@start] = current.value1;
                SharedData[@end] = current.value2;
                SharedProduct = current;
            }
            else
            {
                current = defaultWhenOutRange;
                SharedData[@from] = valueFrom % current.value2;
                SharedData[@to] = valueTo % current.value2;
                if (SharedData[@to] == 0 && valueTo >= current.value2)
                    SharedData[@to] = current.value2;
                else 
                    SharedData[@to] = valueTo % current.value2;
                SharedData[@start] = 0;
                SharedData[@end] = current.value2;
                SharedProduct = current;
            }
        }


        private ProductValue GetCurrent(int compareValue)
        {
            ProductValue temp = database[0];
            foreach (var int4 in database)
            {
                if (temp.value2 < int4.value2 && compareValue > temp.value2)
                    temp = int4;
            }
            return temp;
        }
    }
}