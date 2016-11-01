using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointSystem : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();
    public bool curvesRequireCalculation = false;

    public void AddWaypoint()
    {
        Waypoint newWaypoint = new Waypoint(this, waypoints.Count);
        newWaypoint.GetObject().hideFlags = HideFlags.HideInHierarchy;
        waypoints.Add(newWaypoint);

        curvesRequireCalculation = true;
    }

    public void AddWaypointAt(int index)
    {
        Waypoint newWaypoint = new Waypoint(this, index);
        newWaypoint.GetObject().hideFlags = HideFlags.HideInHierarchy;
        waypoints.Insert(index, newWaypoint);

        if (index + 1 < waypoints.Count)
        {
            for (int i = index + 1; i < waypoints.Count; i++)
                waypoints[i].ChangeIndex(waypoints[i].index + 1);
        }

        curvesRequireCalculation = true;
    }

    public void RemoveWaypoint()
    {
        if (waypoints.Count <= 0)
        {
            Debug.LogError("Remove Waypoint : Index out of bounds.");
            return;
        }

        DestroyImmediate(waypoints[waypoints.Count - 1].GetObject());
        waypoints.RemoveAt(waypoints.Count - 1);

        if (waypoints.Count != 0)
            curvesRequireCalculation = true;
        else curvesRequireCalculation = false;
    }

    public void RemoveWaypointAt(int index)
    {
        if (index >= waypoints.Count || index < 0)
        {
            Debug.LogError("Remove Waypoint : Index out of bounds.");
            return;
        }

        DestroyImmediate(waypoints[index].GetObject());
        waypoints.RemoveAt(index);

        if (index < waypoints.Count)
        {
            for (int i = index; i < waypoints.Count; i++)
                waypoints[i].ChangeIndex(waypoints[i].index - 1);
        }

        if (waypoints.Count != 0)
            curvesRequireCalculation = true;
        else curvesRequireCalculation = false;
    }

    public void RecalculateCurves()
    {
        if (curvesRequireCalculation)
        {
            for (int i = 1; i < waypoints.Count - 1; i++)
            {
                waypoints[i].CalculateCurve(waypoints[i - 1].GetObject().transform.position,
                    waypoints[i + 1].GetObject().transform.position);
            }
        }

        curvesRequireCalculation = false;
    }
}
