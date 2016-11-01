using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorTools
{
    static GameObject[] allObjects;
    static HideFlags[] hideData;

    [MenuItem("Custom/Editor Tools/Un-hide Hierarchy", false, 1)]
    static void Unhide()
    {
        allObjects = Object.FindObjectsOfType<GameObject>();
        hideData = new HideFlags[allObjects.Length];

        for (int i = 0; i < allObjects.Length; i++)
        {
            hideData[i] = allObjects[i].hideFlags;
            allObjects[i].hideFlags = HideFlags.None;
        }
    }

    [MenuItem("Custom/Editor Tools/Re-hide Hierarchy", false, 2)]
    static void Rehide()
    {
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i] != null)
                allObjects[i].hideFlags = hideData[i];
        }

        allObjects = null;
        hideData = null;
    }
}
