using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
[Overlay(typeof(SceneView), "Panel Overlay Example", true)]
public class MyToolButtonOverlay : Overlay
{
    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement() { name = "My Toolbar Root" };
        root.Add(new Label() { text = "Hello" });
        return root;

    }
}