using UnityEngine;
using System.Collections;

public class Move_W : MonoBehaviour {


    public float MovementSpeed = 5.0f;
  
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * MovementSpeed;
        }
    }
}
