using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


public abstract class ParameterOnRentPoolItem : PoolItem
{
    public GameObject[] enableOnRent;

    public override void OnRent()
    {
        base.OnRent();
        foreach (var o in enableOnRent)
            o.SetActive(false);
    }

    // for 
    public abstract void SetParams(Point3Float para);
}


public class PoolManager : BaseView
{
    [MyButton(nameof(ReturnAll))] public int butReturnAll;

    public Transform uiFxParent;

    [SerializeField] private From3DToCanvasPoolDescriptor[] from3DToCanvasDescriptors;
    public IndexPoolCollection<From3DToCanvas> fromCanvasTo3D;

    [SerializeField] private MiscItemPoolDescriptor[] miscDescriptors;
    public IndexPoolCollection<MiscItemView> miscs;


    [SerializeField] private EffectPoolDescriptor[] effectDescriptors;
    public IndexPoolCollection<EffectView> effects;

    // generic pool for un-managed item
    [SerializeField] public PoolCollection<GenericPoolItem> genericPoolItem;

    protected override void Register()
    {
        base.Register();
        genericPoolItem.Init();
        for (var index = 0; index < effectDescriptors.Length; index++)
        {
            var effectPoolDescriptor = effectDescriptors[index];
            if (effectPoolDescriptor.prefab == null)
                Debug.Log("effect" + index + " is null", gameObject);
        }

        for (var index = 0; index < miscDescriptors.Length; index++)
        {
            var effectPoolDescriptor = miscDescriptors[index];
            if (effectPoolDescriptor.prefab == null)
                Debug.Log("misc" + index + " is null", gameObject);
        }

        fromCanvasTo3D = new IndexPoolCollection<From3DToCanvas>(from3DToCanvasDescriptors.Select(x => x.key).ToArray(),
            from3DToCanvasDescriptors.Select(x => x.prefab).ToArray(), uiFxParent);
        miscs = new IndexPoolCollection<MiscItemView>(miscDescriptors.Select(x => x.key).ToArray(),
            miscDescriptors.Select(x => x.prefab).ToArray());
        effects = new IndexPoolCollection<EffectView>(effectDescriptors.Select(x => x.key).ToArray(),
            effectDescriptors.Select(x => x.prefab).ToArray());
    }


    [MyButtonInt(nameof(RentFx),BuiltInEnumType.EffectEnum)] [SerializeField]
    private int testRentFx;
    
    [MyButtonInt(nameof(RentMisc),BuiltInEnumType.MiscEnum)] [SerializeField]
    private int testRentMisc;
    
    [MyButtonInt(nameof(RentGeneric),BuiltInEnumType.PoolItemEnum)] [SerializeField]
    private int testRentGeneric;

    
    public EffectView RentFx(int i)
    {
        var rent = effects[i].Rent(null, Vector3.zero);
        //Debug.Log(rent);
        return rent;
    }
    
    public EffectView RentFx(int i,Transform parent,Vector3 pos)
    {
        var rent = effects[i].Rent(parent, pos);
        //Debug.Log(rent);
        return rent;
    }
    
    public EffectView RentFxWorld(int i,Transform parent,Vector3 pos)
    {
        var rent = effects[i].RentWorld(parent, pos);
        //Debug.Log(rent);
        return rent;
    }
    
    public GenericPoolItem RentGeneric(int i,Transform parent,Vector3 pos)
    {
        foreach (var pool in genericPoolItem.pools)
        {
            if(pool.Prefab.Index == i)
                return pool.Rent(parent, pos); 
        }
        return null;
    }
    
    public GenericPoolItem RentGenericWorld(int i,Transform parent,Vector3 pos)
    {
        foreach (var pool in genericPoolItem.pools)
        {
            if(pool.Prefab.Index == i)
                return pool.RentWorld(parent, pos); 
        }
        return null;
    }

    public GenericPoolItem RentGeneric(int i)
    {
        foreach (var pool in genericPoolItem.pools)
        {
            if(pool.Prefab.Index == i)
                return pool.Rent(); 
        }
        return null;
    }

    
    public void RentMisc(int i)
    {
        var rent = miscs[i].Rent(null, Vector3.zero);
        Debug.Log(rent);
    }

    public void RentFx(IntFloat keyAndDelay, Vector3 pos, Transform parent = null, bool isWorld = true)
    {
        if (keyAndDelay.value >= 0)
            if (keyAndDelay.value2 >= 0)
                keyAndDelay.value2.Timer(Rent);
            else
                Rent();

        void Rent()
        {
            if (isWorld)
                effects[keyAndDelay.value].RentWorld(parent, pos);
            else
                effects[keyAndDelay.value].Rent(parent, pos);
        }
    }


    public void ReturnAll()
    {
        fromCanvasTo3D.ReturnAll();
        miscs.ReturnAll();
        effects.ReturnAll();
    }

    private readonly float[] _coolDown = new float[10]; 
    public bool CoolDownValidAndFill(int coolDownKey,float condition)
    {
        if (Time.time - _coolDown[coolDownKey] > condition)
        {
            _coolDown[coolDownKey] = Time.time;
            return true;
        }

        return false;
    }
}