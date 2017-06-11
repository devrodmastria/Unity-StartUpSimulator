using UnityEngine;
using UnityEngine.UI;

// This script is the boss's artificial intelligence.
// The main purpose of the boss is to instruct the player and analyze the data he/she collects until he/she gets it right (win level 1)

public class BossAI : MonoBehaviour {
    
    // public fields
    public GameObject dialogBox;
    public GameObject customerSegmentBox; // mentor only

    // private fields
    private string[] dialog;
    private Text myText;
    private string msg;
    private bool amTalking = false;
    private string playername;
   
    // Use this for initialization
    void Start()
    {
        dialog = new string[]
        {
            "Mr. M",
            "Welcome to the office. You Research Kit is on the table. Go get it and come back for training.",
        };
        
        myText = dialogBox.GetComponentInChildren<Text>();

    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "MainCamera")
        {

            if (CameraController.isKitLoaded)
            {
                // Good job!
                msg = dialog[0] + ": Good job, " + playername + 
                    ".\nClick on Canvas to get started training. Select from the dropdown menus considering that we work in a noisy environment. Click 'Add Interview' to finish.";
                Invoke("say", 2f);
            }
        }// end tag maincamera
    }

    void OnTriggerStay(Collider coll)
    {
        //print("OnTriggerStay by BossAI");
        if (coll.gameObject.tag == "MainCamera")
        {
            //print("Collide MainCamera OnTriggerStay by BossAI");

            if (coll.gameObject.GetComponent<CameraController>().isTalking) // oh ok, player is talking....
            {

                //print("IsTalking OnTriggerStay by BossAI");
                switch (coll.gameObject.GetComponent<CameraController>().dialogIndex) // what did the player say?
                {
                    case (0):
                        if (!amTalking) // OnTriggerStay runs multiples times per second. This if statement makes sure each dialog msg appears only once.
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false; // I got your message, player. Now it's my turn to talk.

                            // dialog[0] is a reference to the character's name
                            coll.gameObject.GetComponent<CameraController>().dialog[10] = dialog[0]; // update interlocutor's name on player's script 
                                                                                                     // in this case, it means to save the boss name in the player's memory

                            playername = coll.gameObject.GetComponent<CameraController>().dialog[0]; // get the player name as soon as she introduces herself

                            msg = dialog[0] + ": Hi, " + playername + ". I am " + dialog[0] + ", your new mentor. " + dialog[1];
                            Invoke("say", 2f);

                        }
                        break;
                    case (9):
                        print("Case 9 OnTriggerStay by BossAI");
                        if (!amTalking)
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false;

                            print(coll.gameObject.GetComponent<CameraController>().interviewData.Length);

                            if (coll.gameObject.GetComponent<CameraController>().interviewData[0].Equals("null"))
                            {

                                // Future implementation
                                print("collectedData Not Null OnTriggerStay by BossAI");
                                //if (goodDataCollected(coll.gameObject.GetComponent<CameraController>().interviewData)) winnerFirework(); // checks if the data collected by the user is good by checking if it matches the boss' reference data
                                //else feedback(); // gives feedback to player if data does not match reference (what it really does is not check the data but how the player collected and analyzed it) 

                            }
                            else
                            {
                                print("collectedData Null OnTriggerStay by BossAI");
                                msg = dialog[0] + ": You need to review your data.";
                                Invoke("say", 2f);
                            }
                        }

                        break;
                    case (11): // check if player selected the correct options about mentor during training
                        if (!amTalking)
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false;

                            if (coll.gameObject.GetComponent<CameraController>().dialog[11].Contains("Mentor") && 
                                coll.gameObject.GetComponent<CameraController>().dialog[12].Contains("People who work in a noisy environment"))
                            {
                                // show new interview in business canvas
                                customerSegmentBox = GameObject.FindGameObjectWithTag("segmentsbox");
                                Text segments = customerSegmentBox.gameObject.GetComponent<Text>();
                                segments.text += "\n" + coll.gameObject.GetComponent<CameraController>().dialog[11] + ": " +
                                    coll.gameObject.GetComponent<CameraController>().dialog[12];

                                msg = dialog[0] + ": Great Job! You are ready for level 2. Get out of the building and interview the first person you can find.";
                                Invoke("say", 2f);

                                //PlayerPrefs.SetInt("level", 2);
                                print("Uncomment SetInt to level 2 in PlayerPrefs BossAI");
                                CameraController.level = 2;
                                /////////////////////////////////////////////////////////// Player passes level 1... yaaaay!
                            }
                            else if (!coll.gameObject.GetComponent<CameraController>().dialog[11].Contains("Mentor") ||
                                    !coll.gameObject.GetComponent<CameraController>().dialog[12].Contains("People who live in a noisy environment"))
                            {

                                //print("Boss AI: else if");
                                msg = dialog[0] + ": Great start! Try again.";
                                Invoke("say", 2f);
                            }
                            else
                            {
                                //print("Boss AI: else");
                                msg = dialog[0] + ": Great! You almost got it. Try again.";
                                Invoke("say", 2f);
                            }
                        }
                        break;
                } // end of switch
            } // end of if player isTalking
        } // end of if tag main camera
    }
/*
    private bool goodDataCollected(string [] data)
    {
        bool truth = false;
        for(int x = 0; x < data.Length; x++){
            if (data[x].Equals(referenceData[x])) truth = true;
            else
            {
                truth =  false;
                break; // it stops checking whenever a data item does not match the reference data
                
            }

        }
        return truth;
    }
    */
    void winnerFirework()
    {
        // player wins, show fireworks...
    }
    void feedback()
    {
        // ask player to review their conclusions from the data collected
    }
    
    void say()  // adds linebreaks to keep dialog visible
    {
        myText.text += "\n" + msg + "\n";
        amTalking = false;
    }
}
