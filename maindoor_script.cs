using UnityEngine;
using System.Collections;

// This script works as the door lock. It only opens the door when the player has finished level 1 (what a mean boss!)

public class maindoor_script : MonoBehaviour {

    void OnTriggerEnter (Collider coll)
    {

        if(coll.gameObject.tag == "MainCamera")
        {
                    
            if(CameraController.level == 2) gameObject.transform.position = new Vector3(-177.36f, 10.0f, 200.32f);

        }
    } // end OnTriggerEnter

}
