using UniRx;
using UnityEngine;

public class ButtonPoint : BaseButton
{
    [IndexAsEnum(BuiltInEnumType.ButtonPointAction)]
    public int action;
    [EditEnumPoint(1,"point")]
    [SerializeField] private string typeNameValue1;
    [EditEnumPoint(2)]
    [SerializeField] private string typeNameValue2;
    [EditEnumPoint(3)]
    [SerializeField] private string typeNameValue3;
    [EditEnumPoint(4)]
    [SerializeField] private string typeNameValue4;
    
    [SerializeField] private PointParameter point;

    [SerializeField] private IndexBindingScript[] bindValues;
    [SerializeField] private IndexBindingScript[] bindValues2;
    [SerializeField] private IndexBindingScript[] bindValues3;
    [SerializeField] private IndexBindingScript[] bindValues4;

    public PointParameter Point => point;
    protected override void Register()
    {
        base.Register();
        Bind();
    }

    public override void LogCustom()
    {
        base.LogCustom();
        Bind();
    }

    public void Change(PointParameterBase p)
    {
        point.CopyValueFrom(p);
        Bind();
    }

    public void ChangeRef(ref PointParameterBase p)
    {
        point = (PointParameter)p;
    }

    public void Bind()
    {
        bindValues.OnChanged(point.value);
        bindValues2.OnChanged(point.value2);
        bindValues3.OnChanged(point.value3);
        bindValues4.OnChanged(point.value4);
    }

    public void Alter(int valueI, int val)
    {
        point[valueI] = val;
        Bind();
    }
    
    public void Alter(PointParameterBase data)
    {
        point.CopyValueFrom(data);
        Bind();
    }

    public PointParameter ClonePoint() => point.CloneCurrent();

    protected override void OnClick()
    {
        base.OnClick();
        onClick.OnNext(new ButtonPointClick(action,Point));
    }
    
    public Subject<ButtonPointClick> onClick = new Subject<ButtonPointClick>();
    
    public struct ButtonPointClick
    {
        public int Action;
        public PointParameter PointParameter;

        public ButtonPointClick(int a, PointParameter p)
        {
            Action = a;
            PointParameter = p;
        }
    }
}
