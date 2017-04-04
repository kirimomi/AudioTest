using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnButton1Pressed()
    {
        SceneManager.LoadScene("Instruction");
    }

    public void OnButton2Pressed()
    {
        SceneManager.LoadScene("SoundCheck");
    }
}
