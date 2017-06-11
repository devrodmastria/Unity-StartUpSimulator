using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// This script controls the position of the research kit cover (open it when player gets closer, put it down when they move away)

public class InterviewKit : MonoBehaviour {
    
    public GameObject cover;
    
    public GameObject recorderPrefab;
    private GameObject newRecorder;
    static public bool kitOpen = false;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "MainCamera") //print("OnTriggerEnter detected MainCamera on InterviewKit");
        {

            bool firstTalk = CameraController.firstTalkWithMentor;
            if (!firstTalk) // only open kit if player has already talked to mentor
            {
                // Kit
                cover.transform.position = new Vector3(-220.88f, 6.403f, 235.422f);
                cover.transform.Rotate(90f, 0f, 0f);

                // show items in front of kit cover's surface. user has to click on them to get them. 
                // each time they click on an item, it shows what the item does.

                kitOpen = true;

                GameObject myGUITexture = GameObject.FindGameObjectWithTag("Messenger");
                Text myText = myGUITexture.GetComponent<Text>();
                myText.text = "The Research Kit: Hover over the items to learn more about them. "+
                    "Click to pick up.\nYou need them to interview future customers.";

                // Recorder
                if (!CameraController.isKitLoaded) //checks if user has already taken the recorder
                {
                    if (newRecorder == null)
                    {
                        newRecorder = Instantiate(recorderPrefab) as GameObject; // sanity check
                    }
                    newRecorder.transform.position = new Vector3(-221.88f, 6.45f, 235f);
                    newRecorder.SetActive(true);
                    //print("Interview Kit instantiated recorder");
                }
            }
            
        } // end if

    }

    void OnTriggerExit(Collider coll)
    {

        if (coll.gameObject.tag == "MainCamera") //print("OnTriggerExit detected MainCamera on InterviewKit");
        {
            kitOpen = false;

            bool firstTalk = CameraController.firstTalkWithMentor;
            if (!firstTalk) // this prevents Unity from closing the kit if it was never opened
            {
                // Briefcase
                cover.transform.position = new Vector3(-220.88f, 5.501f, 234.33f);
                cover.transform.Rotate(-90f, 0f, 0f);

                // Recorder
                if (newRecorder != null)
                {
                    newRecorder.SetActive(false);
                }
                
            }
        }

    }

	// Use this for initialization
	void Start()
    {
        if(cover == null) cover = GameObject.FindGameObjectWithTag("Kit");
	}
   
}
