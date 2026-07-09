using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteDataButton : MonoBehaviour
{
   private void Start()
   {
      GetComponent<Button>().onClick.AddListener(PlayerPrefs.DeleteAll);
   }
}
