using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	
	void Start () {
	}

	// Endless play
	public void AllLevels()
    {
        // Load the endless play screen
        SceneManager.LoadScene("StockMarketScreen");
    }

	// Tutorial
	public void Tutorial() {
		SceneManager.LoadScene("TutorialScreen");
	}

    // Credits screen
    public void Credits()
	{
		// Load the Credits scene
		SceneManager.LoadScene("CreditsScreen");
	}

	// Reset the user level back to 1
	public void ResetProgress() {
		PlayerPrefs.SetInt("PlayerLevel", 1);
		PlayerPrefs.Save();
	}

	// Quit the game
	public void Quit()
	{
		Application.Quit();
	}
}
