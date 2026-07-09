using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionNoteScript : MonoBehaviour
{
    #if UNITY_EDITOR
    [TextArea(5,20)]
    [SerializeField] private string description;
    #endif
}
