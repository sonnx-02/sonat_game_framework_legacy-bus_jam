using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;


[EditorTool("Rotate Tool", typeof(BaseFrameworkView))]
public class WidthTool : EditorTool, IDrawSelectedHandles
{
    GUIContent m_IconContent;
    public override GUIContent toolbarIcon => m_IconContent;

    bool m_DisableHandles = false;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = Resources.Load<Texture2D>("Icons/R"),
            text = "Rotate Tool",
            tooltip = "Rotate tool"
        };
    }

    public override void OnToolGUI(EditorWindow window)
    {
       
    }

    public void OnDrawHandles()
    {
       
    }

    public void CustomHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        if (m_DisableHandles) // If disabled, do nothing unless it's a repaint event
        {
            if (Event.current.type == EventType.Repaint)
                Handles.CubeHandleCap(controlID, position, rotation, size, eventType);
        }
        else
            Handles.CubeHandleCap(controlID, position, rotation, size, eventType);
    }
}