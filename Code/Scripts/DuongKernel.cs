using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DuongKernel : Kernel
{
    public static readonly Subject<bool> EventKernelLoaded = new Subject<bool>();
}
