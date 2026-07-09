using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;


[Overlay(typeof(SceneView),"Junction Builder",true)]
public class JunctionBuiderOverlay : Overlay
{
    private Label SelectionInfoLabel;
    private Button Build;
    public override VisualElement CreatePanelContent()
    {
        VisualElement root = new VisualElement();
        SelectionInfoLabel = new Label();
        Build = new Button();
        Build.clickable = new Clickable(OnBuildJunction);
        Build.text = "Build";
        
        root.Add(SelectionInfoLabel);
        root.Add(Build);
//        SplineToolUltility.RegisterSelectionChangedEvent();
//        SplineToolUltility.Changed += OnSelectionChanged;
        
        return root;
    }

    public override void OnCreated()
    {
        base.OnCreated();
    //    SplineToolUltility.Changed += OnSelectionChanged;
    }

    public override void OnWillBeDestroyed()
    {
        base.OnWillBeDestroyed();
     //   SplineToolUltility.Changed -= OnSelectionChanged;
    }

    private void OnBuildJunction()
    {
//        var intersection = new Intersection();
//        foreach (var item in SplineToolUltility.GetSelection())
//        {
//            SplineContainer container = (SplineContainer) item.target;
//            var spline = container.Splines[item.targetIndex];
//            intersection.AddJunction(item.targetIndex,item.knotIndex,spline,spline.Knots.ToList()[item.knotIndex]);
//        }
//        (Selection.activeObject as GameObject).GetComponent<SplineRoad>().AddJuntion(intersection);
    }

    private void OnSelectionChanged()
    {
        UpdateSelectionInfo();
    }

    private void ClearSelectionInfo()
    {
        SelectionInfoLabel.text = "";
    }

    private void UpdateSelectionInfo()
    {
        ClearSelectionInfo();
        SelectionInfoLabel.text = "";
//        foreach (var select in SplineToolUltility.GetSelection())
//        {
//            SelectionInfoLabel.text += $"Spline {select.targetIndex}, Knot {select.knotIndex} \n";
//        }
    }
}
