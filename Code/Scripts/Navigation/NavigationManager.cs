using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UniRx;
using UnityEngine;

// ReSharper disable IdentifierTypo


public class NavigationManager : BaseFrameworkView
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.NavigationEnum)]
    private int[] navigations; // name of navigations
    [SerializeField] [IndexAsEnum(BuiltInEnumType.NavigationEnum)]
    private int current = -1; // current navigation
    [SerializeField] private NavigationLocation[] locations; // all location of navigation
    [SerializeField] [DisplayAsDisable] private int step; // step of navigation
    [SerializeField] [DisplayAsDisable] private NavigationLocation currentLocation; // current location of navigation

    protected override void Register()
    {
        base.Register();
        SubjectController.ScreenShowed.Subscribe(data => { CheckActivate(); });
    }

    public void LocationClick(BaseFocusLocation location)
    {
        if (currentLocation == location)
        {
            new SonatLogTutorialComplete(BuiltInEnumType.NavigationEnum.GetName(current),step).Post();
            location.Activate(false);
            if (!currentLocation.isFinish)
            {
                step++;
                FindAndStartLocation();
            }
            else
            {
                PlayerData.navigationChecker.Add(current);
                currentLocation = null;
                current = 0;
            }
        }
    }

    public void LocationDispose(BaseFocusLocation location)
    {
        if (currentLocation == location)
        {
            new SonatLogTutorialComplete(BuiltInEnumType.NavigationEnum.GetName(current)+"_auto_complete",step).Post();
            PlayerData.navigationChecker.Add(current);
            currentLocation = null;
            current = 0;
        }
    }
    

    private void FindAndStartLocation()
    {
        currentLocation = FindLocation();
        if (currentLocation != null)
            Observable.FromCoroutine(WaitForActiveLocation).Subscribe();
    }

    /// <summary>
    /// wait for location is ready on scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForActiveLocation()
    {
        while (currentLocation != null && !currentLocation.gameObject.activeInHierarchy)
            yield return null;
        yield return new WaitForSeconds(0.25f);
        ActivateLocation();
    }

    /// <summary>
    /// activate a location
    /// </summary>
    private void ActivateLocation()
    {
        Debug.Log(nameof(ActivateLocation));
        bool haveLocation = currentLocation != null;
        if (haveLocation)
            currentLocation.Activate(true);
    }

    /// <summary>
    /// find location of of navigation
    /// </summary>
    /// <returns></returns>
    private NavigationLocation FindLocation()
    {
        foreach (var navigationLocation in locations)
            if (navigationLocation.navigationEnum == current && navigationLocation.step == step)
                return navigationLocation;
        return null;
    }

    /// <summary>
    ///  check if no navigation is now active, find and active one
    /// </summary>
    /// <returns>any navigation if active</returns>
    public bool CheckActivate()
    {
        if (current > 0)
            return true; // return if is on a navigation already
        foreach (var nav in navigations)
        {
            if (PlayerData.navigationChecker.Contains(nav)) // skip if navigation passed
                continue;
            // check if a navigation is ready to active through game controller
            bool check = BaseGameController.CheckNavigation(nav);
            if (check)
            {
                current = nav;
                step = 0;
                FindAndStartLocation();
                new SonatLogTutorialBegin(BuiltInEnumType.NavigationEnum.GetName(nav),0).Post();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///  for disable show offer
    /// </summary>
    /// <returns></returns>
    public bool IsOnNavigation()
    {
        return currentLocation != null;
    }
    
    public bool StartNaviWithCheck(int key,Action action)
    {
        onNavi = action;
        if(PlayerData.navigationChecker.Contains(key))
            return false;
        StartNavi(key);
        return true;
    }
    
    public bool StartNaviWithCheck(int key)
    {
        onNavi = null;
        if(PlayerData.navigationChecker.Contains(key))
            return false;
        StartNavi(key);
        return true;
    }

    [IndexAsEnum(BuiltInEnumType.NavigationEnum)]
    [SerializeField] private int[] tutorialPopups;

    public void StartNavi(int key)
    {
        // check show tutorial popup
        if (!tutorialPopups.Contains(key))
            Navi();
        else
            ScreenRoot.popupManager.ShowPopup<PopupTutorial>().SetAction(key,Navi);

        void Navi()
        {
            current = key;
            step = 0;
            FindAndStartLocation();
            onNavi?.Invoke();
            new SonatLogTutorialBegin(BuiltInEnumType.NavigationEnum.GetName(key),0).Post();
        }
    }

    private Action onNavi;
}