using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// This script controls the text on the laptop screen. It keeps track of each character the player interacts with.

public class Laptop : MonoBehaviour {

    private GameObject screen;
    private TextMesh txt;
    private string originalMsg;

    // Use this for initialization
    void Start()
    {
        screen = GameObject.FindGameObjectWithTag("PcScreenText");
        txt = screen.GetComponent<TextMesh>();
    }

    void OnTriggerEnter(Collider c)
    {

        // save original message for future reference
        originalMsg = txt.text;

        if (c.gameObject.tag == "MainCamera")
        {

            txt.text = "People met:\n";

            if (CameraController.peopleMetTag.Count < 1)
            {
                txt.text += "\nNo person met yet.";
            }
            else
            {
                foreach (string name in CameraController.peopleMetTag)
                {
                    string shortName = "";
                    if (name.Contains("Person_")) shortName = name.Replace("Person_", "");
                    txt.text += "\n" + shortName;
                }
            }
               
            

        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "MainCamera")
        {
            // retrieves original message
            txt.text = originalMsg;
        }
    }

    public void print(string s)
    {
        txt.text = s;
    }
}
