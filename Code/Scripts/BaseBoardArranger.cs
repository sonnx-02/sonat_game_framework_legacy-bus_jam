using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public interface ISetPos
{
    void SetPos(Point point,Vector3 centered);
}


public class BaseBoardArranger<T> : MonoBehaviour where T : MonoBehaviour, ISetPos
{
    public Vector3 CenteredPoint { get; set; }
    [SerializeField] private PointSetting data;

    public void Setup(PointSetting pointSetting)
    {
        data = pointSetting;
    }

    [SerializeField] private T[] items;
    [SerializeField] private Transform container; // bottom left pos 
    [SerializeField] private Transform minPosScale; // bottom left pos 
    [SerializeField] private Transform maxPosScale; // top right pos

    [MyButtonInt(nameof(Arrange1), nameof(LoadChildren))]
    public int testFunction;

    public void Arrange1(int unit)
    {
        Arrange(new Vector2Int(unit, unit));
    }

    public void LoadChildren()
    {
        items = (container == null ? transform : container).GetComponentsInFirstBornChildren<T>(true).ToArray();
    }

    public void Arrange(Vector2Int size)
    {
        LoadChildren();
        items.SetActiveAll(false);
        CenteredPoint = PointGetPosition.GetBoardCenteredPosition(data, size);

        Vector3 maxSize =
            new Point(size.x + 1, size.y + 1).GetPos(data);

        if (minPosScale != null && maxPosScale != null)
        {
            Vector3 boardSize = maxPosScale.position - minPosScale.position;
            var scale = Mathf.Min(boardSize.x / maxSize.x, boardSize.y / maxSize.y);
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            transform.localScale = Vector3.one;
        }


        int count = 0;
        for (int i = 0; i < size.x; i++)
        for (int j = 0; j < size.y; j++)
        {
            var point = new PointParameter(i, j, -1,0);
            if (count < items.Length)
            {
                items[count].gameObject.SetActive(true);
                items[count].SetPos(point, CenteredPoint);
            }

            count++;
        }
    }
}