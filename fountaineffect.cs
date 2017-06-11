using UnityEngine;
using System.Collections;

// This script controls the audio component of the water fountain (particle system)

public class fountaineffect : MonoBehaviour {

    public AudioSource audio;

    // Use this for initialization
    void Start () {

        audio = gameObject.AddComponent<AudioSource>();
        AudioClip myClip;
        myClip = (AudioClip)Resources.Load("water2");
        audio.clip = myClip;
        audio.loop = true;
    }
	

    void OnTriggerEnter (Collider coll)
    {
        if (coll.gameObject.tag.Contains("MainCamera"))
        {
            print("Fountain playing");
            audio.Play();
            //audio.Play(44100);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag.Contains("MainCamera"))
        {

            print("Fountain not playing");
            audio.Stop();
            
        }
    }
}
