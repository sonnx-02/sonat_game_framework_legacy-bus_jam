using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameworkHandler : MonoBehaviour
{
    [SerializeField] private BaseFrameworkView[] checkRegisterOnStart;
    // Start is called before the first frame update
    private void Start()
    {
        foreach (var baseFrameworkView in checkRegisterOnStart)
            baseFrameworkView.CheckRegister();
    }
}
