using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : BaseScreen
{
    [SerializeField] private string autoLoadScene;
    [SerializeField] private Image loadingTransition;
    [SerializeField] private AskScriptBase[] askScripts;
    [SerializeField] private BaseRxAnimator[] rxLoadings;
    [SerializeField] private ProgressScript[] progress;
    [SerializeField] private BaseBaseActionScript[] actionBeforeUnload;
    [SerializeField] private GameObject[] deactivateAfterTransition;

    public BoolReactiveProperty isActive;
    public Camera cam;


    protected override void Awake()
    {
        base.Awake();
        if (!string.IsNullOrEmpty(autoLoadScene))
        {
            ScreenRoot.LoadFromIntroScene = true;
            Observable.FromCoroutine(() => IeLoadScene(autoLoadScene)).Subscribe();
        }
    }

    private IEnumerator IeLoadScene(string sceneName)
    {
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        float totalTime = 1.5f;
        float deltaTime = 0;
        
        float first = totalTime * 0.35f;
        while (deltaTime < first)
        {
            foreach (var progressScript in progress)
                progressScript.OnChanged(deltaTime / totalTime);
            deltaTime = Mathf.Min(first,Mathf.Min(Time.deltaTime,0.05f)+deltaTime);
            yield return null;
        }
        foreach (var progressScript in progress)
            progressScript.OnChanged(deltaTime / totalTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.899)
                break;
            yield return null;
        }

        float second = totalTime * 0.8f;
        while (deltaTime < second)
        {
            foreach (var progressScript in progress)
                progressScript.OnChanged(deltaTime / totalTime);
            deltaTime = Mathf.Min(second,Mathf.Min(Time.deltaTime,0.05f)+deltaTime);
            yield return null;
        }
        foreach (var progressScript in progress)
            progressScript.OnChanged(deltaTime / totalTime);
        
        foreach (var baseActionScript in actionBeforeUnload)
            baseActionScript.StartAction();
        
 
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
            yield return null;
        ScreenRoot.OnSceneStartLoading();

        while (!Kernel.IsReady())
            yield return null;
        
        foreach (var askScript in askScripts)
            askScript.Ask(ConsentAskComplete, ConsentUpdateStatus);
        while (askScripts.Any(x => !x.Ready))
            yield return null;
        
        while (deltaTime < totalTime)
        {
            foreach (var progressScript in progress)
                progressScript.OnChanged(deltaTime / totalTime);
            deltaTime = Mathf.Min(totalTime,Mathf.Min(Time.deltaTime,0.05f)+deltaTime);
            yield return null;
        }
        foreach (var progressScript in progress)
            progressScript.OnChanged(deltaTime / totalTime);
        

        yield return null;
        if (loadingTransition != null)
        {
            loadingTransition.gameObject.SetActive(true);
            float t1 = 0f;
            float time = 0.3f;
            while (t1 < time)
            {
                t1 += Time.deltaTime;
                loadingTransition.color = new Color(0, 0, 0, t1 / time);
                yield return null;
            }
        }
        deactivateAfterTransition.SetActiveAll(false);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(autoLoadScene));
        ScreenRoot.OnFinishScreenLoading();
        
        
        if (loadingTransition != null)
        {
            loadingTransition.gameObject.SetActive(true);
            float t1 = 0f;
            float time = 0.3f;
            while (t1 < time)
            {
                t1 += Time.deltaTime;
                loadingTransition.color = new Color(0, 0, 0, 1 - t1 / time);
                yield return null;
            }
        }

        cam.gameObject.SetActive(false);
        ScreenRoot.OnSceneFinishLoading();


        AsyncOperation asyncUnLoad = SceneManager.UnloadSceneAsync(sceneIndex);
        while (!asyncUnLoad.isDone)
            yield return null;
       
        yield return new WaitForSeconds(0.3f);
        IsInitialized = true;
        if( ScreenRoot.CurrentScreen != null)
            ScreenRoot.CurrentScreen.ShowSomeThing();
    }

    [SerializeField] private bool transition;

    public void StartLoad(bool setInitialized = false)
    {
        IsInitialized = setInitialized;
        rxLoadings.Select(x => x.IO_AnimatorInThenOut())
            .WhenAll()
            .SelectMany(WaitKernelReady)
            .Subscribe(data =>
            {
                if (!transition)
                {
                    ScreenRoot.OnFinishScreenLoading();
                }
                else
                {
                    ScreenRoot.IsInTransition = true;
                    ScreenRoot.ScreenTransition.IO_AnimatorIn() // transition fade in
                        .SelectMany(IeFinishLoading)
                        .SelectMany(ScreenRoot.ScreenTransition.IO_AnimatorOut()) // transition fade out
                        .Subscribe(_ => { ScreenRoot.IsInTransition = false; });
                }
            });
    }

    private IEnumerator WaitKernelReady()
    {
        while (!Kernel.IsReady())
            yield return null;
    }

    private IEnumerator IeFinishLoading()
    {
        yield return null;
        ScreenRoot.OnFinishScreenLoading();
    }


    private IDisposable _disposable;

    public void InjectProgress(FloatReactiveProperty property)
    {
        if (_disposable != null)
            _disposable.Dispose();
        _disposable = property.TakeUntilDisable(this)
            .Subscribe(data =>
            {
                if (data >= 1f)
                    data = 1f;

                foreach (var progressScript in progress)
                    progressScript.OnChanged(data);

                if (data >= 1)
                    _disposable.Dispose();
            });
    }

    public override void OnShowHandler()
    {
        base.OnShowHandler();

        //Hide banner
        Kernel.Resolve<AdsManager>().HideBanner();
        //if (Kernel.Resolve<AdsManager>().IsBannerShowing())
        //    Kernel.Resolve<AdsManager>().HideBanner();
        //Kernel.Resolve<AdsManager>().onBannerShow.Action += HideBanner;
    }

    public override void OnHideHandler()
    {
        base.OnHideHandler();

        //Kernel.Resolve<AdsManager>().onBannerShow.Action -= HideBanner;
    }

    private void ConsentAskComplete()
    { 
    
    }

    private void ConsentUpdateStatus(bool check)
    {

    }

    private void HideBanner(bool isLoaded)
    {
        if (isLoaded)
        {
            Kernel.Resolve<AdsManager>().HideBanner();
        }
    }
}