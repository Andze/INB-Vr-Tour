using UnityEngine;
using System.Collections;

public class WhizDish : MonoBehaviour {


    int iStartTimeout;

    // Use this for initialization
    void Start () {

        iStartTimeout = 25;

    }
	
	// Update is called once per frame
	void Update () {


        if (iStartTimeout > 0)
            iStartTimeout = iStartTimeout -1;
    }

}
