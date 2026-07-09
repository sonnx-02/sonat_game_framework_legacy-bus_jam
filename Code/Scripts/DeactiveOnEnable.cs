using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveOnEnable : MonoBehaviour
{
    [SerializeField] private float after;
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(IeStart());
    }
    
    private IEnumerator IeStart()
    {
        yield return new WaitForSeconds(after);
        gameObject.SetActive(false);
    }
}
