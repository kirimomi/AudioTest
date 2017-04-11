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
#if (UNITY_IOS || UNITY_ANDROID)
        Handheld.PlayFullScreenMovie("mov1.mp4", Color.black, FullScreenMovieControlMode.Full);
#endif

    }

    public void OnButton2Pressed()
    {
#if (UNITY_IOS || UNITY_ANDROID)
        Handheld.PlayFullScreenMovie("mov2.mp4", Color.black, FullScreenMovieControlMode.Full);
#endif
    }

    public void OnHomePressed()
    {
        SceneManager.LoadScene("Main");
    }
}
