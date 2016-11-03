using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaypointSystem : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();
    public bool curvesRequireCalculation = false;
    
    private bool done = false;
    private int waypointIndex = 0;
    private Vector3 nextPoint;
    private Quaternion lastRotation;
    private float totalDistance;

    void Start()
    {
        if (waypoints.Count > 0)
        {
            totalDistance = Vector3.Distance(transform.position, waypoints[waypointIndex].GetTransform().position);
            lastRotation = transform.rotation;

            waypoints[waypointIndex++].GetNextPoint(out nextPoint);
        }
        else Done();
    }

    void Update()
    {
        if (!done)
        {
            if (transform.position == nextPoint)
            {
                if (waypointIndex >= waypoints.Count)
                {
                    Done();
                    return;
                }

                if (!waypoints[waypointIndex].GetNextPoint(out nextPoint))
                {
                    if (++waypointIndex < waypoints.Count)
                        waypoints[waypointIndex].GetNextPoint(out nextPoint);
                }

                totalDistance = Vector3.Distance(transform.position, waypoints[waypointIndex].GetTransform().position);
                lastRotation = transform.rotation;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, nextPoint, 0.5f * Time.deltaTime);

            float currentDistance = Vector3.Distance(transform.position, waypoints[waypointIndex].GetTransform().position);
            if (currentDistance != 0.0f)
                transform.rotation = Quaternion.Lerp(waypoints[waypointIndex].GetTransform().rotation, lastRotation, currentDistance / totalDistance);
        }
    }

    private void Done()
    {
        done = true;
        waypointIndex = 0;

        for (int i = 0; i < waypoints.Count; i++)
            waypoints[i].bezierIndex = 0;
    }

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
