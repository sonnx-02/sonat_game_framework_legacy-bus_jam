using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene(sceneName); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
