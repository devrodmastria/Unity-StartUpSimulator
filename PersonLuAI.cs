using UnityEngine;
using UnityEngine.UI;

// This script is the artificial intelligence for character Lu

public class PersonLuAI : MonoBehaviour {

    private float moveY = 0.1f;
    private float originalMoveY;

    public GameObject dialogBox;
    public GameObject segbox;

    private string[] dialog;
    private Text myText;
    private string msg;
    private bool amTalking = false;
    private Text hints;
    private GameObject messenger;

    // Use this for initialization
    void Start () {

        dialog = new string[]
        {
            "Sure",

            "Lu",

            "I am a teacher so I guess I am the one who makes the noise, haha!",

            "My house can be noisy sometimes. Specially when my kids are playing videogames.",

            "It feels like I am playing the games with them, when all I want is to quietly enjoy some wine with my husband.",

            "Sure!", // If person agrees to talk, show clipboard and ask interviewer to choose questions.

            // Answer to interview question one: How do you feel about...
            "I think it..."

        };
        myText = dialogBox.GetComponentInChildren<Text>();
        messenger = GameObject.FindGameObjectWithTag("Messenger");
        hints = messenger.GetComponent<Text>();

    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.tag == "MainCamera")
        {

            if (coll.gameObject.GetComponent<CameraController>().isTalking) // player is talking
            {

                //print("IsTalking OnTriggerStay by BossAI");
                switch (coll.gameObject.GetComponent<CameraController>().dialogIndex) // what did the player say?
                {
                    case (0):
                        if (!amTalking) // OnTriggerStay runs multiples times per second. This "if" statement makes sure each dialog msg appears only once.
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false; // I got your message, player. Now it's my turn to talk.

                            msg = dialog[1] + ": " + dialog[0];
                            Invoke("say", 3f);

                        }
                        break;
                    case (1):
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false;

                            // dialog[0] is a reference to the character's name
                            coll.gameObject.GetComponent<CameraController>().dialog[10] = dialog[0]; // update interlocutor's name on player's script 
                                                                                                     // in this case, it means to save the boss name in the player's memory

                            msg = dialog[1] + ": I am " + dialog[1] + ".";
                            Invoke("say", 2f);

                        }
                        break;
                    case (2):
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false;

                            msg = dialog[1] + ": " + dialog[2];
                            Invoke("say", 2f);

                        }
                        break;
                    case (3):
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false;

                            msg = dialog[1] + ": " + dialog[3];
                            Invoke("say", 2f);

                        }
                        break;

                    case (4):
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false;

                            msg = dialog[1] + ": " + dialog[4];
                            Invoke("say", 2f);

                        }
                        break;

                    case (20): // check if player entered the correct interview data
                        {
                            amTalking = true;
                            coll.gameObject.GetComponent<CameraController>().isTalking = false;

                            if (coll.gameObject.GetComponent<CameraController>().dialog[11].Contains("Teacher") &&
                                coll.gameObject.GetComponent<CameraController>().dialog[12].Contains("People who live in a noisy environment"))
                            {

                                segbox = GameObject.FindGameObjectWithTag("segmentsbox");
                                Text segments = segbox.gameObject.GetComponent<Text>();
                                segments.text += "\n" + coll.gameObject.GetComponent<CameraController>().dialog[11] + ": " +
                                    coll.gameObject.GetComponent<CameraController>().dialog[12];

                                hints.text = "You Win!";
                                coll.gameObject.GetComponent<CameraController>().play("win");
                                
                            }
                            else
                            {
                                hints.text = "Good start. Try again.";
                            }
                                
                        }
                        break;
                }
            }
        }
    }

    void OnTriggerEnter (Collider coll)
    {
        //print("OnTriggerEnter by PersonAI");
        if(coll.gameObject.tag == "MainCamera")
        {
            //print("main camera detected OnTriggerEnter by PersonAI");
            originalMoveY = moveY;
            moveY = 0f;
        }
        else
        {
            //print("main camera NOT detected OnTriggerEnter by PersonAI");
            moveY *= -1;
        }
    }

    void OnTriggerExit (Collider coll)
    {
        //print("OnTriggerExit by PersonAI");
        if (coll.gameObject.tag == "MainCamera")
        {
            //print("main camera detected OnTriggerExit by PersonAI");
            moveY = originalMoveY;
        }

    }

    void say() // adds linebreaks to keep dialog visible
    {
        myText.text += "\n" + msg + "\n";
        amTalking = false;
    }

    // Update is called once per frame
    void FixedUpdate () {

        gameObject.transform.position += new Vector3(0f, 0f, moveY);
	
	}
}
