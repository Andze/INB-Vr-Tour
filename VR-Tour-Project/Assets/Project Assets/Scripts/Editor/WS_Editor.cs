using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CustomEditor(typeof(WaypointSystem))]
public class WS_Editor : Editor
{
    [SerializeField]
    private WaypointSystem _target;

    void OnEnable()
    {
        _target = (WaypointSystem)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel(_target.waypoints.Count + " waypoints");
        if (GUILayout.Button("+"))
            _target.AddWaypoint();
        if (GUILayout.Button("-"))
            _target.RemoveWaypoint();

        EditorGUILayout.EndHorizontal();

        if (_target.curvesRequireCalculation)
        {
            EditorGUILayout.HelpBox("Curves require re-calculation for smoothing to occur.", MessageType.Warning);

            if (GUILayout.Button("Recalculate Now"))
                _target.RecalculateCurves();
        }
    }

    void OnSceneGUI()
    {
        for (int i = 0; i < _target.waypoints.Count; i++)
        {
            Waypoint w = _target.waypoints[i];
            Transform t = w.GetTransform();

            // Marker
            Handles.color = new Color(0.5f, 0f, 1f, 1f);
            Handles.SphereCap(0, t.position, t.rotation, 0.1f);
            
            EditorGUI.BeginChangeCheck();

            // Position handle
            Vector3 position = Handles.PositionHandle(t.position, t.rotation);

            #region Directional Handles
            // Rotation Handle
            Quaternion rotation = Handles.Disc(t.rotation, t.position, t.up, w.curveRadius, false, 1f);

            // Radius Handle
            float radius = Handles.ScaleValueHandle(w.curveRadius, t.position + (t.transform.forward * w.curveRadius), t.rotation, 0.5f, Handles.ConeCap, 1f);

            if (EditorGUI.EndChangeCheck())
            {
                if (t.position != position || t.rotation != rotation || w.curveRadius != radius)
                    _target.curvesRequireCalculation = true;

                Undo.RecordObject(target, "Changed Target Position.");
                t.position = position;

                Undo.RecordObject(target, "Changed Target Rotation.");
                t.rotation = rotation;

                Undo.RecordObject(target, "Changed Target Radius.");
                w.curveRadius = radius;
            }
            #endregion

            #region Connections
            // Connection to previous waypoint.
            if (i > 0)
            {
                Transform targetT = _target.waypoints[i - 1].GetTransform();
                Handles.DrawLine(t.position, targetT.position);

                if (!_target.curvesRequireCalculation && w.GetBezierPoints() != null)
                {
                    Vector3[] bezierPts = w.GetBezierPoints();
                    for (int j = 0; j < bezierPts.Length - 1; j++)
                        Handles.DrawDottedLine(bezierPts[j], bezierPts[j + 1], 3f);
                }
            }

            // Connection to next waypoint.
            if (i < _target.waypoints.Count - 2)
            {
                Transform targetT = _target.waypoints[i + 1].GetTransform();
                Handles.DrawLine(t.position, targetT.position);

                if (!_target.curvesRequireCalculation && w.GetBezierPoints() != null)
                {
                    Vector3[] bezierPts = w.GetBezierPoints();
                    for (int j = 0; j < bezierPts.Length - 1; j++)
                        Handles.DrawDottedLine(bezierPts[j], bezierPts[j + 1], 3f);
                }
            }
            #endregion

            #region Interface
            // Text
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            Handles.Label(t.position + new Vector3(0f, 0.05f, 0f), w.index.ToString(), style);

            // Buttons
            Transform cameraT = Camera.current.transform;
            Vector3 offsetLR = cameraT.right.normalized * 0.2f;

            // Plus
            Handles.color = Color.green;
            Quaternion plusRotation = cameraT.rotation * Quaternion.Euler(-90, 0, 0);
            if (Handles.Button(t.position + offsetLR, plusRotation, 0.05f, 0.05f, Handles.ConeCap))
                _target.AddWaypointAt(w.index + 1);

            // Minus
            Handles.color = Color.red;
            Quaternion minusRotation = cameraT.rotation * Quaternion.Euler(90, 0, 0);
            if (Handles.Button(t.position - offsetLR, minusRotation, 0.05f, 0.05f, Handles.ConeCap))
                _target.RemoveWaypointAt(w.index);

            /* MINUS SHOULD ALWAYS RUN LAST */
            #endregion
        }

        Repaint();
    }
}
