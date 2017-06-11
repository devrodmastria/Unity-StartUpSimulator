using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

// This script does most of the hard work. It is the manager of the game.

public class CameraController : MonoBehaviour {

    public float rotationSpeed = 30.0f;
    public float verticalMouseSpeed = 50.0F;
    public float movementSpeed = 0.5f;
    static public float camY;

    private string welcomeMsg = "Welcome to your new internship as a market researcher. This is your new workplace, try to find your mentor! Press A, W, S, or D to move around. Esc to exit.";
    static public bool isKitLoaded = false;
    static public bool questionsLoaded = false;
    static public bool showingRecorder = false;
    private bool showingScoreBoard = false;
    public bool hitWall = false;

    public GameObject recorder;
    public GameObject ceilingPrefab;
    public GameObject smartw; // display debugging messages on the wall ( for VR )
    private GameObject ceiling;
    public GameObject segbox;

    public GameObject chatBox;

    private GameObject pilot;

    private GameObject messenger; // used to display general messages such as what the user should do during gameplay
    public GameObject dialogBox;
    private GameObject scoreboard;
    private GameObject kitButtons;
    private GameObject dialogBtn;
    private GameObject playernameField;
    private InputField playername;

    // Interview Questions menu
    public Dropdown questionDropdown;
    public Dropdown titleDropdown;
    public Dropdown customerSegmentDropdown;
    public Button addinterviewButton;

    static public AudioSource audio;

    private bool GUIenabled = true;
    static public bool firstTalkWithMentor = true;
    private bool usertyping = true; // start as true so that user can type their name when the game starts without calling the other methods that also rely on the keyboard input

    static public int level = 1;

    private string question;

    private Text hints;
    private Text myText; // Text object of the dialog box object
    private string msg; // message to be displayed in the messenger.

    static int interviewDataSize = 100;
    public string[] interviewData = new string[interviewDataSize]; // player has to give this information to the Mentor.
                                                                   // Mentor determines if player wins the game / earn points.


    public bool isTalking = false; // is the player talking (did she send a message to another character?) 
                                   // it allows the interlocutor (other character) to respond only when the player is done talking,
                                   // after a few seconds, so that it looks like a human conversation.

    private string interlocutor = ""; // depending on who is the interlocutor detected by the collider (Mentor, stranger, nobody) the interview questions/dialog button changes.

    public int dialogIndex = 0; // responsible for coordinating the communitcation between player and characters in the game. 
                                // each index represents a specific type of message (see dialog string array below)
                                // each time the player talks (sends a message) to another character, this index is updated with the kind of message
                                // so that the other character knows what to reply.

    // dialog string array contains the recurring pieces of the player's dialog. Replaced by Unity's visual editor - > Canvas, Scoreboard, Question_Dropdown
    public string[] dialog;
        /*
        //Index 0 : player name
        "Player", // prompt player for their name on start

        //Index 1 : intro
        "I am a researcher with the National Science Foundation and part of my work involves understanding the impact of noise in the work environment. " + 
            "Can we talk for a minute?", 

        //Index 2 : ask name
        "What's your name?", 

        //Index 3 : disclaimer (give a chance to one grumpy character to say "No! Don't record me.")
        "Thank you so much for talking with me. This conversation will be very helpful as I work to understand this issue. " + 
            "I am going to record our conversation to be sure that I don't miss anything and so that I can go back and listen when we're done.", 

        //Index 4 :  close ended question 1 - work
        "Do you work in a noisy environment?", 

        //Index 5 :  close ended question 2 - home
        "Do you live in a noisy environment?",

        //Index 6 : open ended question
        "Can you describe how does it feel like?",

        //Index 7 : follow up with people
        "It has been great talking to you. Can I contact you if I have more questions?", 

        //Index 8 : thank you
        "Great! Thank you so much for your time.", 

        //Index 9 : deliverable - checking with Mentor 
        "Mr. B, this is the data I collected from the interviews.",

        //Index 10: interlocutor name
        "name", // replace with real name on OnTriggerEnter - changes every interview

        //Index 11: Customer Title - changes every interview
        "title",

        //Index 12: Customer Segment - changes every interview
        "segment"


    };
    */
    static public List<string> peopleMetTag;
    
    void initializeInterviewData()
    {
        interviewData[0] = "null";
        for (int x = 1; x < interviewData.Length; x++){
            // string[] interviewData = new string[interviewDataSize];
            interviewData[x] = "null" + x;
        }

    }

    void Awake()
    {
        if (PlayerPrefs.HasKey("level"))
        {
            level = PlayerPrefs.GetInt("level"); // if there is a level saved, update variable "level"
            //print("Saved highest level: " + level);
        }
        PlayerPrefs.SetInt("level", level); // if there is no saved level yet, set it to level 1

    }

    void Start()
    {

        print("If Dialog box is not showing any text, check if the canvas scrollview shows anything prior to playing");
        print("Z activates Research Kit");
        print("Esc quits the game");
        print("Enter sets game player name to default");
        print("1 sets game level to 1");
        print("2 sets game level to 2");

        PlayerPrefs.SetInt("level", 1);

        dialog = new string[15];
        dialog[0] = "Player";
        dialog[1] = "I am a researcher with the National Science Foundation and part of my work involves understanding the impact of noise in the work environment. " +
            "Can we talk for a minute?";
        for (int x = 2; x < dialog.Length; x++)
        {
            dialog[x] = "Player";
        }

        questionDropdown.onValueChanged.AddListener(delegate
        {
            questionDropdownValueChangedHandler(questionDropdown);
        });

        titleDropdown.onValueChanged.AddListener(delegate
        {
            titleDropdownValueChangedHandler(titleDropdown);

        });

        customerSegmentDropdown.onValueChanged.AddListener(delegate
        {
            customerSegmentDropdownValueChangedListener(customerSegmentDropdown);
        });

        addinterviewButton.onClick.AddListener(delegate
        {
            addinterviewButtonHandler();
        });

        peopleMetTag = new List<string>();

        initializeInterviewData();

        ceiling = Instantiate(ceilingPrefab) as GameObject;
        ceiling.SetActive(true);

        messenger = GameObject.FindGameObjectWithTag("Messenger");
        hints = messenger.GetComponent<Text>();
        hints.text = welcomeMsg;

        recorder = GameObject.FindGameObjectWithTag("mobileRecorder");
        recorder.SetActive(false);

        kitButtons = GameObject.FindGameObjectWithTag("Kit");
        kitButtons.SetActive(false);

        playernameField = GameObject.FindGameObjectWithTag("playername");
        playername = playernameField.GetComponent<InputField>();
        
        dialogBox.SetActive(false);
        myText = dialogBox.GetComponentInChildren<Text>();

        scoreboard = GameObject.FindGameObjectWithTag("score");
        scoreboard.SetActive(false);

        dialogBtn = GameObject.FindGameObjectWithTag("dialogbutton");
        dialogBtn.SetActive(false);

        pilot = GameObject.FindGameObjectWithTag("pilot");

    }

    void Destroy()
    {
        questionDropdown.onValueChanged.RemoveAllListeners();
        titleDropdown.onValueChanged.RemoveAllListeners();
        customerSegmentDropdown.onValueChanged.RemoveAllListeners();
    }

    public void questionDropdownValueChangedHandler(Dropdown target)
    {
        // get list of questions
        List<Dropdown.OptionData> questions = questionDropdown.options;

        //get current question by index
        question = questions[target.value].text;

        isTalking = true;
        dialogIndex = target.value;
        msg = dialog[0] + ": " + questions[target.value].text;
        say();


        print(question + " test question on CameraController index: " + target.value);
    }

    public void titleDropdownValueChangedHandler(Dropdown target)
    {
        // get list of questions
        List<Dropdown.OptionData> titles = titleDropdown.options;

        //get current question by index
        dialog[11] = titles[target.value].text;

        print(dialog[11] + " test on CameraController");
    }

    public void customerSegmentDropdownValueChangedListener(Dropdown target)
    {
        // get list of questions
        List<Dropdown.OptionData> segments = customerSegmentDropdown.options;

        //get current question by index
        dialog[12] = segments[target.value].text;
        print(dialog[12] + " test on CameraController");
    }

    public void addinterviewButtonHandler()
    {

        // level 1 is for training so check level when click on interview kit buttons and 
        // show guide (Mentor responds according to what player clicks) during level 1.
        if (interlocutor.Contains("Mentor")) // ensure that traning dialog only happens when player is near Mentor
        {

            // tell Mentor what options you selected (dropdown lists)
            isTalking = true;
            dialogIndex = 11; // title
            msg = dialog[0] + ": Please check my answers";
            say();

        }else
        {

            isTalking = true;
            dialogIndex = 20; // check if interview data is correct


        }
        // end of training dialog script 

        //print("Title and segment, respectively " + dialog[11] + " " + dialog[12]); // title and segment
    }

    public void onPlayerNameClick()
    {
        if (!playername.text.Equals(""))
        {
            dialog[0] = playername.text;
        }

        usertyping = false;
        GUIenabled = false;
        //print("onPlayerNameClick by CameraController");
        playernameField.SetActive(false);

    }

    public void selfIntro()
    {
        peopleMetTag.Add(interlocutor); // if they haven't met, add person to list of people met

        if (interlocutor.Contains("Mentor"))
        {
            if (firstTalkWithMentor) // REPLACE with metBefore method
            {
                isTalking = true;
                firstTalkWithMentor = false;
                dialogIndex = 0;
                msg = dialog[dialogIndex] + ": Hi, I am " + dialog[dialogIndex] + ".";
                say();

            }
        }
        else 
        {

            dialogIndex = 0;
            msg = dialog[0] + ": Hi, I am " + dialog[0] + ". " + dialog[1];
            say();

            isTalking = true;

        }
        dialogBtn.SetActive(false);

    }

    void OnGUI()
    {
        GUIStyle customButton = new GUIStyle("button");
        customButton.fontSize = 20;

        if (isKitLoaded)
        {
            kitButtons.SetActive(true);
        }
        
    }

    void say()
    {
        
        myText.text += "\n" + msg + "\n";

    }


    void clearDialog()
    {
        myText.text = "Dialog\n";
    }

    public void showIntroButton(bool state)
    {
        dialogBtn.SetActive(state);
        GUIenabled = state;
    }

    public void showRecorder()
    {

        if (isKitLoaded)
        {

            if (!showingRecorder)
            {
                showingRecorder = true;
                recorder.SetActive(true);


            }
            else
            {
                showingRecorder = false;
                recorder.SetActive(false);

            }
        }

    }

    public void showScoreBoard()
    {
        if (showingScoreBoard) // hide scoreboard
        {
            showingScoreBoard = false;
            GUIenabled = false;
            scoreboard.SetActive(false);
        }
        else
        {
            showingScoreBoard = true;
            GUIenabled = true;
            scoreboard.SetActive(true);
        }
    }

    bool metBefore(string tag)
    {
        bool met = false;

        if (peopleMetTag != null)
        {
            foreach (string name in peopleMetTag)
            {
                if (name == tag) // assign tag to peopleMetTag strings OnTriggerEnter
                {
                    met = true;
                    break;
                }
            }
            
        }
        else
        {
            print("peopleMetTat is NULL");
        }
        
        return met;
    }

    void OnTriggerEnter(Collider coll)
    {
        //print("OnTriggerEnter by CameraController");
        if (coll.tag.Contains("Person"))
        {


            if (!metBefore(coll.tag))
            {
                showIntroButton(true);
                clearDialog();
            }
            
            hint(coll.tag.Replace("Person_", ""));
            interlocutor = coll.tag; // needed to show dialog button when meeting Mentor

            GUIenabled = true;
            dialogBox.SetActive(true);
            

        }
        else if (coll.tag == "wall")
        {

            hitWall = true;
            print("Hit wall OnCollisionEnter by CameraController");
        }
        else if(coll.tag == "Kit")
        {
            GUIenabled = true;

            if (firstTalkWithMentor)
            {
                hint("This is the Research Kit (box on the table)\nTalk to Mentor to get access to it");
            }
        }
        else if(coll.tag == "maindoor")
        {
            if (PlayerPrefs.HasKey("level"))
            {
                if (PlayerPrefs.GetInt("level") == 1)
                { // only open door if player has passed level one
                  
                    hitWall = true;
                }

            }
            
        }
        else if (coll.tag == "Desk")
        {

            hitWall = true;
        }
        else if (coll.tag == "Chair")
        {

            hitWall = true;
        }

    }

    void OnTriggerExit(Collider coll)
    {

        if (level == 1)
        {
            hint("Level 1 Goal: Learn how to use the Business Canvas");

        }

        else if (level == 2)
        {
            hint("Level 2 Goal: Conduct your first interview");

        }

        GameObject collideWith = coll.gameObject;

        if (collideWith.tag == "wall")
        {
            hitWall = false;
        }
        else if (coll.tag.Contains("Person"))
        {
            showIntroButton(false); // it will hide the intro button
            interlocutor = "";
            GUIenabled = false;
            dialogBox.SetActive(false);

            if (coll.tag.Contains("Mentor"))
            {
                //segbox = GameObject.FindGameObjectWithTag("segmentsbox");
                Text segments = segbox.gameObject.GetComponent<Text>();
                segments.text = "Customer Segments";
            }

        }
        else if (coll.tag == "Kit")
        {
            GUIenabled = false;
        }
        else if (coll.tag == "maindoor")
        {
            hitWall = false;
        }

    }

    //int num = 0; // for VR debugging
    
    void FixedUpdate()
    {
        // Movement and rotation
        Vector3 positionCam = Vector3.zero;
        Vector3 camEuler = Camera.main.transform.rotation.eulerAngles;
        camEuler.x = 0f;
        Quaternion normalizedRotation = Quaternion.Euler(camEuler);

        // Rotate camera with mouse
        //float translation = Input.GetAxis("Mouse Y") * verticalMouseSpeed;
        //float rotation = Input.GetAxis("Mouse X") * rotationSpeed;
        //translation *= -Time.deltaTime; // used to look up and down with mouse. negative gives it a more natural movement
        //rotation *= Time.deltaTime;
        //transform.Translate(0, 0, translation);

        //if (!GUIenabled)
        //    Camera.main.transform.Rotate(0, rotation, 0); // only rotate camera with mouse when GUI (scoreboard & text input field) is not active

        
        if (hitWall)
        {
            //positionCam += normalizedRotation * Vector3.back * movementSpeed * 1;
            pilot.transform.position += normalizedRotation * Vector3.back * 0.25f;
            hitWall = false;

        }
        else if (!usertyping)
        {

            if (Input.GetKey("left") || Input.GetKey("a"))
            {
                camY = -rotationSpeed * Time.deltaTime;
                Camera.main.transform.Rotate(0.0f, camY, 0.0f);
            }
            else if (Input.GetKey("right") || Input.GetKey("d"))
            {
                camY = rotationSpeed * Time.deltaTime;
                Camera.main.transform.Rotate(0.0f, camY, 0.0f);

            }

            float gamepadSpeedVertical = Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed;
            Vector3 newPos = Camera.main.transform.forward;
            newPos.y = 0f;
            pilot.transform.position += newPos * gamepadSpeedVertical;

            //Research KIT
            if (Input.GetKeyUp("r"))
            {

                showRecorder();


            }
            else if (Input.GetKeyUp("k"))
            {
                showScoreBoard();
            }

        } // end of if user not typing statement
        

        if (Input.GetKeyUp(KeyCode.Z)) // for debugging only, skips level 1 tasks.
        {
            onPlayerNameClick();
            isKitLoaded = true;
            showingRecorder = false;
            firstTalkWithMentor = false;
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            onPlayerNameClick();
        }
        else if (Input.GetKeyUp("1"))
        {
            level = 1;
            PlayerPrefs.SetInt("level", 1);
            print("Reset to Level 1");
        }
        else if (Input.GetKeyUp("2"))
        {
            level = 2;
            PlayerPrefs.SetInt("level", 2);
            print("Reset to Level 2");
        }

        else if (Input.GetKey(KeyCode.Joystick1Button3))
        {

            showScoreBoard();
        }

        else if (Input.GetKey(KeyCode.Joystick1Button0))
        {

            showRecorder();
        }

    // This code display the key pressed on the virtual pc in the game (for debugging)
//        if (Input.anyKey)
//        {
//           foreach (KeyCode keyco in Enum.GetValues(typeof(KeyCode)))
//                {
//
//                    GameObject gameObj = GameObject.FindGameObjectWithTag("PcScreenText");
//                    TextMesh txt = gameObj.GetComponent<TextMesh>();
//
//                    string theKey = keyco.ToString();
//                    txt.text = theKey;
//            }
//        }


    } // end FixedUpdate

    void hint(string s)
    {
        Text banner = messenger.GetComponent<Text>();
        banner.text = s;
    }

    void clearMessenger()
    {
        Text banner = messenger.GetComponent<Text>();
        banner.text = "";
    }

    public void play(string au)
    {
        AudioClip myClip = (AudioClip)Resources.Load(au);
        audio = gameObject.AddComponent<AudioSource>();
        audio.clip = myClip;
        audio.Play();
    }
}