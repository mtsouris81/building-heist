using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StyleSheet))]
public class OStylesheetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        StyleSheet myScript = (StyleSheet)target;
        if (GUILayout.Button("Apply Styles"))
        {
            StyleSheetConsumer[] consumers = GameObject.FindObjectsOfType<StyleSheetConsumer>();
            if (consumers != null)
            {
                foreach(var c in consumers)
                {
                    c.ApplyStyles(myScript);
                }
                SceneView.RepaintAll();
            }
        }
    }
}