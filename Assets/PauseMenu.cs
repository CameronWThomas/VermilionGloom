using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public static bool GameIsPaused = false;
	public GameObject pauseMenuUI;

	void Start()
	{
		pauseMenuUI.SetActive(false); // Ensure the pause menu is hidden at the start
	}
	void Update()
	{
		// Check if the player presses the Escape key
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameIsPaused)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
	}

	// Resume the game
	public void Resume()
	{
		pauseMenuUI.SetActive(false); // Hide the pause menu
		Time.timeScale = 1f; // Set game speed back to normal
		GameIsPaused = false;
	}

	// Pause the game
	void Pause()
	{
		pauseMenuUI.SetActive(true); // Show the pause menu
		Time.timeScale = 0f; // Freeze the game
		GameIsPaused = true;
	}

	// Load the main menu (usable when we have a main menu)
	public void LoadMainMenu()
	{
		Time.timeScale = 1f; // Make sure the time scale is normal when switching scenes (This there to unpause the game before loading the main menu, so the new scene behaves properly without being affected by the paused time scale from the previous scene.)
		SceneManager.LoadScene("MainMenu");
	}

	public void ResetTutorialMessages()
	{
		GameState.Instance.ClearTutorials();
	}

	// Quit the game
	public void QuitGame()
	{
		Debug.Log("Quitting game..."); // This will be seen in the editor
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false; // Stop playing the game in the editor
#else
		Application.Quit(); // Quit the game when built
#endif
	}
}