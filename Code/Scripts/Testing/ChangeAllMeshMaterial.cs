using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ChangeAllMeshMaterial : MonoBehaviour
{
    [SerializeField] private Material _material;

    [SerializeField] private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            foreach (var meshRenderer in target.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.sharedMaterial = _material;
            }
        });
    }

    
}
