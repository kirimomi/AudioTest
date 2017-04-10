using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    var audioSource = GetComponent<AudioSource>();

        audioSource.clip = Microphone.Start(null, true, 10, 24000);

        audioSource.loop = true;

        audioSource.pitch = 0.95f;

        while (Microphone.GetPosition(null) <= 0) { }
        Debug.Log("rec start.");
        audioSource.Play();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
