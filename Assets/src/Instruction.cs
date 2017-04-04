using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Instruction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnButton1Pressed()
    {
        Handheld.PlayFullScreenMovie("mov1.mp4", Color.black, FullScreenMovieControlMode.Full);

    }

    public void OnButton2Pressed()
    {
        Handheld.PlayFullScreenMovie("mov2.mp4", Color.black, FullScreenMovieControlMode.Full);
    }

    public void OnHomePressed()
    {
        SceneManager.LoadScene("Main");
    }
}
