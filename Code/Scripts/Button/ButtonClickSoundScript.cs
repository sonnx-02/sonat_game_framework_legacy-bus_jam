using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSoundScript : MonoBehaviour
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private int sound = 1;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            ShortHand.LightFeedBack();
            sound.PlaySound();
        });
    }

}
