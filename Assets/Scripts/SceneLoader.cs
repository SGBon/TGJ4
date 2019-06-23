using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);

        AudioListener savedListener = AudioManager.GetInstance().Listener;

        foreach (AudioListener a in (FindObjectsOfType<AudioListener>() as AudioListener[]))
        {
            if (a != savedListener)
            {
                Destroy(a);
            }
        }
    }

    public void UnloadScene(int scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
}
