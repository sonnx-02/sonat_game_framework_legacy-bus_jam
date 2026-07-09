using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[System.Serializable]
public class TestArrayElementTitleByName 
{
    public Transform target;
}


public class AttributeExample : BaseFrameworkView
{
    [SerializeField] private TestFunction testFunction123 = new TestFunction()
    {
        method = "functionName"
    };
    
   
    private void TestShowMessage(int i)
    {
        Debug.Log("TestShowMessage="+i);
    }
    
    private void TestShowMessage2(int anim,int i)
    {
        Debug.Log("TestShowMessage2_"+ anim +"_"+i);
    }
    
#if UNITY_EDITOR
    [LabelHeader("1  Button IntInt As Enum",0,1,1)] [SerializeField]  private int testLabel16;
#endif
    [MyButtonInt(new []{nameof(TestShowMessage2)},BuiltInEnumType.SoundEnum)]
    public IntInt testAnims;
  
#if UNITY_EDITOR
    [LabelHeader("2 Test Button Int As Enum",0,1,1)] [SerializeField]  private int testLabel16_1;
#endif
    [MyButtonInt(new []{nameof(TestShowMessage)},BuiltInEnumType.SoundEnum)]
    public int testAnims2;
  
#if UNITY_EDITOR
    [LabelHeader("3 ConditionalField null",0,1,1)] [SerializeField]  private int testLabel16_1a;
#endif  
    [SerializeField] [ConditionalField(nameof(delayScript), new object[] {null})]
    private GameActionEvent action2;
    
#if UNITY_EDITOR
    [LabelHeader("4 IndexParam script with param",0,1,1)] [SerializeField]  private int testLabel1adasd;
#endif
    [IndexParam(nameof(delayScriptIndex))] public ParameterScriptFloat delayScript;
    [HideInInspector] [SerializeField] private int delayScriptIndex;

    
#if UNITY_EDITOR
    [LabelHeader("5 ConditionLabel",0,1,1)] [SerializeField]
    private int testLabel;
#endif
    [SerializeField] private BuiltInFunction Enum;
    [SerializeField]
    [ConditionLabel(nameof(Enum),
        new object[] {BuiltInFunction.Retry, BuiltInFunction.QuitGame},
        new[] {"ForceRetry", "ScreenTransition"})]
    private bool value123;
    
#if UNITY_EDITOR
    [LabelHeader("6 IndexByBuiltInEnum, reload to display",0,1,1)] [SerializeField]  private int testLabel2;
#endif
 
    [IndexByBuiltInEnum(nameof(builtInEnumType))]
    public int value; 
    public BuiltInEnumType builtInEnumType;
    
#if UNITY_EDITOR
    [LabelHeader("7 display int as enum",0,1,1)] [SerializeField]  private int testLabel4;
#endif
    [IndexAsEnum(BuiltInEnumType.InScreenPopupEnum)]
    [SerializeField] private int value2; 

#if UNITY_EDITOR
    [LabelHeader("8 display int as flag",0,1,1)] [SerializeField]  private int testLabel6;
#endif
    [IndexAsEnumFlag(BuiltInEnumType.TutorialEnableFlag)] 
    public int tutEnables;

#if UNITY_EDITOR
    [LabelHeader("9 display as label cannot edit",0,1,1)] [SerializeField]  private int testLabel7;
#endif
    [DisplayStringAsLabel] public string currentText = "Hello";
   
    
#if UNITY_EDITOR
    [LabelHeader("10 display as disable",0,1,1)] [SerializeField]  private int testLabel8;
#endif
    [DisplayAsDisable]
    public int notEdit;
    
#if UNITY_EDITOR
    [LabelHeader("11 display by DataEnum",0,1,1)] [SerializeField]  private int testLabel9;
#endif
    [SerializeField] protected DataEnum dataEnum;
    [SerializeField]
    [IndexAsEnumCondition(nameof(dataEnum),
        new object[]
        {
            DataEnum.CustomPlayerDataProperty,
            DataEnum.CustomDailyDataProperty,
            DataEnum.CustomWeeklyDataProperty,
            DataEnum.CustomSessionDataProperty,
            DataEnum.CustomWaveDataProperty,
            DataEnum.GameSaveProperty,
        },
        new[]
        {
            nameof(BuiltInEnumType.CustomPlayerDataProperty),
            nameof(BuiltInEnumType.CustomDailyDataProperty),
            nameof(BuiltInEnumType.CustomWeeklyDataProperty),
            nameof(BuiltInEnumType.CustomSessionDataProperty),
            nameof(BuiltInEnumType.CustomWaveDataProperty),
            nameof(BuiltInEnumType.GameSaveKey),
        })]
    private int customPropertyEnum;
    
#if UNITY_EDITOR
    [LabelHeader("12 IndexAsEnumMultiple",0,1,1)] [SerializeField]  private int testLabel9_;
#endif
    [IndexAsEnumMultiple(new object[]
        {
            Quantity.SystemAction, 
            Quantity.GameAction, 
            Quantity.SystemGameAction,
            Quantity.Item,
            Quantity.CustomProperty,
            Quantity.Achievement,
            Quantity.Quest,
            Quantity.Subscription,
            Quantity.TutorialSkip,
        },
        new[]
        {
            nameof(BuiltInEnumType.ProductSystemAction),
            nameof(BuiltInEnumType.ProductGameAction),
            nameof(GameActionEvent), 
            nameof(BuiltInEnumType.ItemTypeEnum),
            nameof(BuiltInEnumType.CustomPlayerDataProperty),
            nameof(BuiltInEnumType.AchievementEnum),
            nameof(BuiltInEnumType.QuestEnum),
            nameof(BuiltInEnumType.SubscriptionKey),
            nameof(BuiltInEnumType.TutorialSkipType),
        },
        nameof(quantity))]
    public int index;
    public Quantity quantity;
    
    
    [ListPointName(
        new int []{10}, 
        new []{nameof(CustomParameter.boolValue),nameof(CustomParameter.boolValue),nameof(CustomParameter.intValue3),nameof(CustomParameter.boolValue)}, 
        new []{"InitMap"}, 
        new []{40,30} ,ListPointType.ColorMap,5,5)]
    public ListPoint map; 
    
    [ListPointPointName(
        new int[] { },
        //new int []{10}, 
        new[]
        {
            nameof(CustomParameter.intValue), nameof(CustomParameter.intValue2), nameof(CustomParameter.intValue3),
            nameof(CustomParameter.boolValue)
        }
        , new[] {"Index", "Type", ""}
        , new[] {40, 40, 30}, ListPointType.Absolute
        , 8, 12
        , new string[] {null, null}
    )]
    public ListPointPoint arrange2;

   
#if UNITY_EDITOR
    [LabelHeader("13 ArrayElementIndexTitleAttribute as index to enum",0,1,1)] [SerializeField]  private int testLabel10;
#endif
    [ArrayElementIndexTitleAttribute(nameof(DailyQuestItem.key),BuiltInEnumType.QuestEnum)]
    public DailyQuestItem[] dailyQuestItems;
    [ArrayElementIndexTitleAttribute(new []{"price",nameof(ShopPrice.priceKey),nameof(ShopStock.key)},
        BuiltInEnumType.ShopItemKey)] 
    [SerializeField] protected ShopItem[] shopItems;
    
    [ArrayElementTitleByName(nameof(TestArrayElementTitleByName.target))]
    public TestArrayElementTitleByName[] targets;
    
#if UNITY_EDITOR
    [LabelHeader("14 ArrayElementTitle enum to string",0,1,1)] [SerializeField]  private int testLabel11;
#endif
    [ArrayElementTitle("action")] [SerializeField]
    public RxSequenceNode2[] nodes;
  
#if UNITY_EDITOR
    [LabelHeader("15 First is IndexEnum",0,1,1)] [SerializeField]  private int testLabel12;
#endif
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private IntFloat sounds;
    
#if UNITY_EDITOR
    [LabelHeader("16 InLine",0,1,1)] [SerializeField]  private int testLabel13;
#endif
    [SerializeField] [InLine(nameof(dealDamageScript))] private float delayDamage;
    [SerializeField] [HideInInspector] private int dealDamageScript;
    
#if UNITY_EDITOR
    [LabelHeader("17 InLine",0,1,1)] [SerializeField]  private int testLabel14;
#endif
    [ArrayElementIndexTitleAttribute(
        new string[]{nameof(TestArrayElement2.key),nameof(IntInt.value)},
        "SoundEnum",
        new[]{nameof(TestArrayElement2.key),nameof(IntInt.value2)}
    )]
    [SerializeField] private TestArrayElement2[] testArray;
    
    
#if UNITY_EDITOR
    [LabelHeader("18 Index display by enum",0,1,1)] [SerializeField]  private int testLabel15;
#endif
    [ArrayElementIndexTitleAttribute2("SoundEnum")]
    [SerializeField] private int[] testArray2;
    
#if UNITY_EDITOR
    [LabelHeader("19 ConditionalField",0,1,1)] [SerializeField]  private int testLabel161;
#endif
    
    [SerializeField] [ConditionalField(nameof(overrideAction), new object[] {null})]
    private GameActionEvent action;
    [SerializeField] private GameActionEventByRemoteScript overrideAction;
    

    
#if UNITY_EDITOR
    [LabelHeader("20 Enum By State",0,1,1)] [SerializeField]  private int testLabel17;
#endif
    [IntRxAsEnumFlag(BuiltInEnumType.GameState)]
    public IntReactiveProperty gameState;
  
#if UNITY_EDITOR
    [LabelHeader("21 FlagEnum By State Condition",0,1,1)] [SerializeField]  private int testLabel17_;
#endif
    [SerializeField] protected FlagState flagStateEnum;
    [SerializeField] [IndexAsEnumFlagCondition(
        new object[]
        {
            FlagState.QuestState,
            FlagState.GameState,
            FlagState.LevelSelectState,
        },
        new[]
        {
            nameof(QuestState),
            "GameState",
            nameof(LevelSelectState),
        },nameof(flagStateEnum))]
    private int flag;
    
#if UNITY_EDITOR
    [LabelHeader("22 IndexByBuiltInEnum, reload after Select",0,1,1)] [SerializeField]  private int testLabel18;
#endif
    [IndexByBuiltInEnum(nameof(builtInEnumType2))] // for display
    public int value3; // x % 2 == 0, 0 is compare value
    public BuiltInEnumType builtInEnumType2;
    
#if UNITY_EDITOR
    [LabelHeader("23 IndexAsEnumFromProperty",0,1,1)] [SerializeField]  private int testLabel19;
#endif
    [SerializeField] private BuiltInEnumType builtInEnumType3;
    [SerializeField] [IndexAsEnumFromProperty(nameof(builtInEnumType3))]
    private int testEnumFrom;
    
    
    
    [MyButtonToggleEnumSelf(typeof(AddRemove))] [SerializeField]
    private IntReactiveProperty addRemove;
    [MyButtonToggleEnumSelf(typeof(AddRemove))] [SerializeField]
    private int addRemove2;
    
    
    [MyButtonToggleSelf("Clear On", "Clear Off")] [SerializeField]
    private BoolReactiveProperty clear;
    [MyButtonToggleSelf("Clear On", "Clear Off")] [SerializeField]
    private IntReactiveProperty clear2;
    [MyButtonToggleSelf("Clear On", "Clear Off")] [SerializeField]
    private int clear3;    
    [MyButtonToggleSelf("Clear On", "Clear Off")] [SerializeField]
    private bool clear4;
    
    
    [EditEnumPoint(1)]
    [SerializeField] private string typeNameValue1;
    [SerializeField] private PointParameter point;

    public enum AddRemove
    {
        Add,
        Remove,
        Nothing
    }
}


[Serializable]
public class TestArrayElement2
{
    [IndexAsEnum("SoundEnum")]
    public IntInt key;

    public string test;
}
