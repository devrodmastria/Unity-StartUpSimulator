using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// This script controls the behaviour of the game object(s) inside the research kit

public class InterviewKitCover : MonoBehaviour {

    void OnMouseEnter()
    {
        if (InterviewKit.kitOpen)
        {
            GameObject myGUITexture = GameObject.FindGameObjectWithTag("Messenger");
            Text myText = myGUITexture.GetComponent<Text>();
            myText.text = "The Research Kit: Hover over the items to learn more about them. Click to pick up.\nYou need them to interview future customers.";
        }
    }

    void OnMouseDown()
    {

        if (!CameraController.isKitLoaded)
        {
            CameraController.isKitLoaded = true;
            CameraController.showingRecorder = false;

            GameObject recorder = GameObject.FindGameObjectWithTag("newRecorder");
            recorder.SetActive(false);
        }


    }
}
