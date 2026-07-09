//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UniRx;
//using UnityEngine.Serialization;
//
//public class PlayScreen2 : BaseScreen
//{
//    [SerializeField] public ToggleScript[] onReady;
//    [SerializeField] public ToggleScript[] onPlaying;
//    [SerializeField] private IndexBindingScript[] startGameLevelBindings; // update only when start game not always correspond with current level
//    public ProgressScript gameProgress;
//
//    public void SetupStartLevel(int currentLevelValue)
//    {
//        Show(true);
//        startGameLevelBindings.OnChanged(currentLevelValue);
//        if (onReady != null) onReady.OnChanged(true);
//        if (onPlaying != null) onPlaying.OnChanged(false);
//    }
//}
