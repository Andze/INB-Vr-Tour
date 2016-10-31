using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(WaypointSystem))]
public class WS_Editor : Editor
{
    WaypointSystem t;

    void OnEnable()
    {
        t = (WaypointSystem)target;
    }

    public override void OnInspectorGUI()
    {
        
    }

    void OnSceneGUI()
    {
        
    }
}
