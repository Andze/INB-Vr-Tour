using UnityEngine;
using System.Collections;

[System.Serializable]
public class Waypoint
{
    public int index = -1;
    public float curveRadius = 0.5f;
    public int bezierIndex = 0;

    [SerializeField]
    private WaypointSystem _waypointSystem;
    [SerializeField]
    private Vector3[] bezierPts;
    [SerializeField]
    private GameObject _object;

    public Waypoint(WaypointSystem waypointSystem, int index)
    {
        _waypointSystem = waypointSystem;
        this.index = index;

        Initialize();
    }

    private void Initialize()
    {
        _object = new GameObject("Waypoint " + index);
        
        if (_waypointSystem != null)
        {
            if (index > 0)
            {
                _object.transform.position = _waypointSystem.waypoints[index - 1].GetTransform().position;
                _object.transform.rotation = _waypointSystem.waypoints[index - 1].GetTransform().rotation;
            }
            else
            {
                _object.transform.position = _waypointSystem.transform.position;
                _object.transform.rotation = _waypointSystem.transform.rotation;
            }
        }
        else
        {
            Debug.LogError("Waypoint doesn't belong to a system!");
        }
    }

    public Vector3[] CalculateCurve(Vector3 prevWaypoint, Vector3 nextWaypoint)
    {
        Vector3 aHead = prevWaypoint - _object.transform.position;
        Vector3 aDir = aHead / aHead.magnitude;

        Vector3 bHead = nextWaypoint - _object.transform.position;
        Vector3 bDir = bHead / bHead.magnitude;

        Vector3 p0 = _object.transform.position + (aDir * curveRadius);
        Vector3 p1 = _object.transform.position + (aDir * (curveRadius / 2f));
        Vector3 p2 = _object.transform.position + (bDir * (curveRadius / 2f));
        Vector3 p3 = _object.transform.position + (bDir * curveRadius);

        bezierPts = new Vector3[10];
        for (int i = 1; i <= bezierPts.Length; i++)
        {
            float t = (float)i / 10f;
            float omt = 1f - t;
            float omt2 = omt * omt;
            float t2 = t * t;

            bezierPts[i - 1] = p0 * (omt2 * omt)
                + p1 * (3f * omt2 * t)
                + p2 * (3f * omt * t2)
                + p3 * (t2 * t);
        }

        return bezierPts;
    }

    public void ChangeIndex(int newIndex)
    {
        index = newIndex;
        _object.name = "Waypoint " + index;
    }

    public GameObject GetObject()
    {
        return _object;
    }

    public Transform GetTransform()
    {
        return _object.transform;
    }

    public string GetName()
    {
        return _object.name;
    }

    public Vector3[] GetBezierPoints()
    {
        return bezierPts;
    }

    public bool GetNextPoint(out Vector3 value)
    {
        if (_waypointSystem.curvesRequireCalculation || bezierPts.Length == 0)
        {
            value = _object.transform.position;
            return true;
        }
        else if (bezierIndex < bezierPts.Length)
        {
            value = bezierPts[bezierIndex++];
            return true;
        }
        else
        {
            value = Vector3.zero;
            return false;
        }
    }
}
