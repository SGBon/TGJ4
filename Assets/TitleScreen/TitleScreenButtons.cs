using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenButtons : MonoBehaviour
{
    // Start Button
    public void StartGame()
    {
        // Start the game
        Debug.Log("Game Start!");
    }

    // Continue
    public void ContinueGame()
    {
        // Select savegame to continue from
        Debug.Log("Continuing Game...");
    }

    // Quit Button
    public void Quit()
    {
        // Quit the game application
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
