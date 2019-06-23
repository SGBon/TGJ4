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
        anim.SetBool("open_door", true);
        am.PlaySoundOnce(AudioManager.Sound.DoorOpen, transform, AudioManager.Priority.High, AudioManager.Pitches.VeryLow);
    }


    public void CloseDoor()
    {
        anim.SetBool("open_door", false);
    }
}

