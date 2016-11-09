using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Teleport : MonoBehaviour
{
    public bool active = true;
    private Transform target;

    void Start()
    {
        Teleport[] allTeleporters = transform.parent.GetComponentsInChildren<Teleport>();

        for (int i = 0; i < allTeleporters.Length; i++)
        {
            if (GetComponent<Teleport>() != allTeleporters[i])
                target = allTeleporters[i].transform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Transform otherRigid = other.attachedRigidbody.transform;
        if (otherRigid.tag == "Player" && active)
        {
            Vector3 direction = target.transform.position - transform.position;

            Transform nvrC = otherRigid.parent.GetComponentInChildren<Camera>().transform;
            otherRigid.parent.position += direction;
            nvrC.position += direction;

            target.GetComponent<Teleport>().active = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Transform otherRigid = other.attachedRigidbody.transform;
        if (otherRigid.tag == "Player" && !active)
            active = true;
    }
}
