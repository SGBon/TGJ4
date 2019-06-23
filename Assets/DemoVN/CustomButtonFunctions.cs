using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButtonFunctions : MonoBehaviour
{
    // Quit Button
    public void Quit()
    {
        // Quit the game application
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
