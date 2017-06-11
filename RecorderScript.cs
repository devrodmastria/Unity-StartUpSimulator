using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// This script controls the visibility of the recorder button.

public class RecorderScript : MonoBehaviour {

    private string previousHint = "";
    private GameObject Texture;
    private Text myText;

    void Start()
    {
        GameObject Texture = GameObject.FindGameObjectWithTag("Messenger");
        myText = Texture.GetComponent<Text>();
    }

    void OnMouseEnter()
    {
        
        previousHint = myText.text;

        myText.text = "Audio Recorder. An essential item for quickly and accurately collecting data.\n"+
            "Always use it during interviews.";
        //print("OnMouseEnter RecorderScript");

    }

    void OnMouseExit()
    {
        
        myText.text = previousHint;
        //print("OnMouseExit RecorderScript");

    }

    void OnMouseDown()
    {

        if (!CameraController.isKitLoaded)
        {
            CameraController.isKitLoaded = true;
            CameraController.showingRecorder = false;
            //CameraController.firstTime = true;

            gameObject.SetActive(false);
        }

    }

}
