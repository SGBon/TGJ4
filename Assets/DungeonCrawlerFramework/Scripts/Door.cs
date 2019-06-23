using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator anim;

    AudioManager am;

    void Start()
    {
        anim = GetComponent<Animator>();
        am = AudioManager.GetInstance();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}

    public void OpenDoor()
    {
        if (!anim.GetBool("open_door"))
        {
            am.PlaySoundOnce(AudioManager.Sound.DoorOpen, transform, AudioManager.Priority.High, AudioManager.Pitches.VeryLow);
        }
        anim.SetBool("open_door", true);
    }


    public void CloseDoor()
    {
        if (anim.GetBool("open_door"))
        {
            am.PlaySoundOnce(AudioManager.Sound.DoorClose, transform, AudioManager.Priority.High, AudioManager.Pitches.VeryLow);
        }
        anim.SetBool("open_door", false);
    }
}

