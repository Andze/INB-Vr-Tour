using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

public class AssignMaterial : ScriptableWizard
{
    public Material theMaterial;
    String strHelp = "Select Game Objects";

    void OnWizardUpdate()
    {
        helpString = strHelp;
        isValid = (theMaterial != null);
    }

    void OnWizardCreate()
    {
        GameObject[] objs = Selection.gameObjects;
        for (int i = 0; i < objs.Length; i++)
            objs[i].GetComponent<MeshRenderer>().material = theMaterial;
    }

    [MenuItem("Custom/Assign Material", false, 4)]
    static void assignMaterial()
    {
        DisplayWizard("Assign Material", typeof(AssignMaterial), "Assign");
    }
}