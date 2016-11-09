using UnityEngine;
using System.Collections;

public class LightCulling : MonoBehaviour
{
    public float distance = 20.0f;

    private Light[] lights;
    void Start()
    {
        GameObject[] lightObj = GameObject.FindGameObjectsWithTag("Light");
        lights = new Light[lightObj.Length];

        for (int i = 0; i < lightObj.Length; i++)
        {
            lights[i] = lightObj[i].GetComponentInChildren<Light>();
        }
    }

    void Update()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            if (Vector3.Distance(transform.position, lights[i].transform.position) > distance)
            {
                lights[i].enabled = false;
            }
            else lights[i].enabled = true;
        }
    }
}
