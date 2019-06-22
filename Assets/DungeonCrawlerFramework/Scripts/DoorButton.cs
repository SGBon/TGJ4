using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
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

    public void ButtonUp()
    {
        anim.SetBool("button_position", false);
    }


    public void ButtonDown()
    {
        anim.SetBool("button_position", true);
    }
}
