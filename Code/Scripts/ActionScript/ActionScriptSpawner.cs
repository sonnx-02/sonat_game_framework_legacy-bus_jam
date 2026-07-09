using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ActionScriptSpawner : BaseActionScript
{
    [SerializeField] private PoolCollection<PoolItemGeneric> pools;
    [SerializeField] private SpawnPosEnum posEnum;
    [SerializeField] private SpawnType spawnType;
    [SerializeField] private Vector3[] possiblePositions;
    [ConditionalField(nameof(posEnum),new object[]{SpawnPosEnum.RandomRange})]
    [SerializeField] private Vector3 randomFrom;
    [ConditionalField(nameof(posEnum),new object[]{SpawnPosEnum.RandomRange})]
    [SerializeField] private Vector3 randomTo;
    [SerializeField] private int maxQuantity;
    [SerializeField] private float coolDown;

    [SerializeField] private bool coolDownCurve;
    [SerializeField] private float coolDownDecreaseTo;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float timeCooldownCurve;
    [SerializeField] private float timeThrottle;

    [ConditionalField(nameof(spawnType),new object[]{SpawnType.SpawnRandom})]
    [SerializeField] private int[] randomIndexes;
    
    [Serializable]
    private enum SpawnPosEnum
    {
        Default,
        Possible,
        RandomRange
    }
    
    [Serializable]
    private enum SpawnType
    {
        Spawn0,
        SpawnRandom,
    }

    protected override void Register()
    {
        base.Register();
        pools.Init();
    }

    private float _lastTimeSpawn = -999;
    protected override void DoAction()
    {
        base.DoAction();
        CheckRegister();

        switch (spawnType)
        {
            case SpawnType.Spawn0:
                Spawn(0);
                break;
            case SpawnType.SpawnRandom:
                Spawn(randomIndexes.TakeRandom());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    
    }

    private float _increase = 0;
    public void Spawn(int i)
    {
        if (coolDownCurve)
        {
            if (Time.time - _lastTimeSpawn > timeThrottle)
                _increase = 0;
            else
                _increase += Time.deltaTime;
        }
        
        if (Valid() && pools.CountAll() < maxQuantity)
        {
            _lastTimeSpawn = Time.time;
            var rent = pools.pools[i].RentWorld(null,transform.position);
            rent.StartAction();
            switch (posEnum)
            {
                case SpawnPosEnum.Default:
                    break;
                case SpawnPosEnum.Possible:
                    rent.transform.localPosition = possiblePositions.TakeRandom();
                    break;
                case SpawnPosEnum.RandomRange:
                    rent.transform.localPosition = randomFrom.Random(randomTo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private bool Valid()
    {
        if(!coolDownCurve)
            return Time.time - _lastTimeSpawn >= coolDown;
        
        var t = _increase / timeCooldownCurve;
        return Time.time - _lastTimeSpawn >= Mathf.Lerp(coolDown,coolDownDecreaseTo,curve.Evaluate(t));
    }


    
}
