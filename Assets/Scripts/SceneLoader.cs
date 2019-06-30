using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Singleton;

    [SerializeField]
    UnityEngine.Events.UnityEvent pongClear;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
            Destroy(gameObject);
    }

    public static SceneLoader GetInstance()
    {
        return Singleton;
    }

    public void PongClear()
    {
        pongClear.Invoke();
    }

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

    public void SwitchScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void UnloadScene(int scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
}
