using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public abstract class BaseShopItemViewClickHandler : BaseFrameworkView
{
    public abstract void Handler(BaseBaseShopItemView shopItemView);
}