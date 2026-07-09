using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridlayoutGroupAutoResize : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    // Start is called before the first frame update
    [ContextMenu("start")]
    void Start()
    {
       var grid = GetComponent<GridLayoutGroup>();
       var width = this.gameObject.GetComponent<RectTransform>().rect.width;
       Vector2 newSize = new Vector2(width / 2, this.gameObject.GetComponent<GridLayoutGroup>().cellSize.y);
       this.gameObject.GetComponent<GridLayoutGroup>().cellSize = newSize;
    }

    
}
