﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}

    public void OpenDoor()
    {
        anim.SetBool("open_door", true);
    }


    public void CloseDoor()
    {
        anim.SetBool("open_door", false);
    }
}
