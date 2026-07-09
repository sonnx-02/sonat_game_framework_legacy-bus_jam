using UnityEngine;
using UnityEngine.Video;

public class VideoTutorialManager : MonoBehaviour
{
    public VideoPlayer player;
    public VideoClip[] clips;

    public void SetupVideo(int tut)
    {
        player.clip = clips[tut - 1];
        player.isLooping = true;
        player.Prepare();

        player.Play();
    }

    public void StopVideo()
    { 
        player.Stop();
    }

    public bool IsTutVideoReady()
    {
        return player.isPrepared;
    }
}
