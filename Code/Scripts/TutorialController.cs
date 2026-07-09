using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEngine;
using UniRx;

public class TutorialController : BaseFrameworkView
{
    private BooleanSerializableDictionary _tutorialPass;
    [SerializeField] private TutorialButton[] tutorialButtons;
    [SerializeField] private TutorialHand tutorialHand;
    [SerializeField] private GameMainEvent[] checkOnEvent;
    [SerializeField] private bool checkOnPopupChanged;

    [SerializeField] private TutorialButton testFocus;

    public override void LogCustom()
    {
        base.LogCustom();
        FocusButton(testFocus);
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        _tutorialPass = PlayerPrefs.HasKey(nameof(_tutorialPass))
            ? JsonUtility.FromJson<BooleanSerializableDictionary>(PlayerPrefs.GetString(nameof(_tutorialPass)))
            : new BooleanSerializableDictionary();

        if (!FinishAll())
        {
            if (checkOnEvent.Length > 0)
                SubjectController.OnGameMainEvent.Subscribe(data =>
                {
                    if (checkOnEvent.Contains(data))
                        DuongExtensions.ExecuteInNextFrame(CheckTut);
                });
        }

        SubjectController.ScreenClosed.Subscribe(data => { FocusButton(null); });

        SubjectController.PopupChanged.Subscribe(data =>
        {
            FocusButton(null);
            if (checkOnPopupChanged)
                DuongExtensions.ExecuteInNextFrame(CheckTut);
        });
    }

    private bool FinishAll()
    {
        foreach (var tutorialButton in tutorialButtons)
            if (tutorialButton != null && !_tutorialPass[tutorialButton.ButtonEnum])
                return false;
        return true;
    }

    private TutorialButton _lastButton;

    public void FocusButton(TutorialButton tutorialButton)
    {
        if (tutorialButton == _lastButton)
            return;
        if (_lastButton != null)
        {
            _lastButton.Focus(false);
            if (_lastButton.TimeScaleZero)
                Time.timeScale = 1;
            if (_lastButton.DisableInput)
                BaseGameController.DisableInput = false;
        }

        if (tutorialButton != null)
        {
            if (!tutorialButton.gameObject.activeInHierarchy)
                return;

            if (tutorialButton.DisableOtherRaycaster)
                ScreenRoot.TurnGraphicRaycaster(false);
            tutorialButton.Focus(true);
            new SonatLogTutorialBegin(
                "tutorial_button_" + BuiltInEnumType.TutorialButtonEnum.GetName(tutorialButton.ButtonEnum), 0).Post();
            tutorialButton.Button.onClick.AddListener(() => FinishAnTutorial(tutorialButton.ButtonEnum));
            if (tutorialButton.TimeScaleZero)
                Time.timeScale = 0;
            if (tutorialButton.DisableInput)
                BaseGameController.DisableInput = true;
        }
        else
        {
            //  if(tutorialButton != null && tutorialButton.DisableOtherRaycaster)
            ScreenRoot.TurnGraphicRaycaster(true);
        }

        tutorialHand.FocusOn(tutorialButton);
        _lastButton = tutorialButton;
    }

    public void CheckTut()
    {
        foreach (var tutorialButton in tutorialButtons)
            if (tutorialButton != null && !_tutorialPass[tutorialButton.ButtonEnum])
            {
                var validEnum = BaseGameController.CheckValidTutorialButton(tutorialButton.ButtonEnum);
                if (validEnum == ValidEnum.Valid)
                {
                    if (tutorialButton.DelayToStart <= 0)
                        FocusButton(tutorialButton);
                    else // event will recheck to start after
                    {
                        tutorialButton.DelayToStart.Timer(() =>
                        {
                            if (RootView.rootView.gameController.CheckValidTutorialButton(tutorialButton.ButtonEnum) ==
                                ValidEnum.Valid)
                                FocusButton(tutorialButton);
                        });
                    }

                    break;
                }

                if (validEnum == ValidEnum.Skip)
                {
                    _tutorialPass[tutorialButton.ButtonEnum] = true;
                }
            }
    }

    private void FinishAnTutorial(int key)
    {
        new SonatLogTutorialBegin("tutorial_button_" + BuiltInEnumType.TutorialButtonEnum.GetName(key), 0).Post();

        _tutorialPass[key] = true;
        FocusButton(null);
        0.5f.Timer(CheckTut);
    }
}