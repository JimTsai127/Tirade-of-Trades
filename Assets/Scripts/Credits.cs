using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Go back to main menu
	public void MainMenu() {
		// Load the Main Menu scene
		SceneManager.LoadScene("MainMenuScreen");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
