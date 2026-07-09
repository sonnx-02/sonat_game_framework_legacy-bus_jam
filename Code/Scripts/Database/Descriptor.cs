
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public struct AnimationData
{
    public bool active;
    public float x;
    public float y;
    public float z;
    public float time;
    public Ease ease;
    
    public Vector3 vec => new Vector3(x,y,z);
}