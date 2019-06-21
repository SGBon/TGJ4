﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCFLocomotionSystem : MonoBehaviour
{
    [SerializeField]
    LayerMask wallLayer;

    [SerializeField]
    float wallCheckDistance = 5;

    [SerializeField]
    float turnSpeed = 0.75f;

    Animator anim;

    Vector3 previousPos;
    float previousRot;

    bool movedForward;

    [SerializeField]
    int rotationAccumulation = 0;

    bool turnedLeft;
    bool turnedRight;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsMoving())
        {
            if (!WallInFront())
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    anim.SetTrigger("MoveForward");
                    movedForward = true;
                }
            }

            if (!movedForward)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    StartCoroutine(TurnLeft());
                    turnedLeft = true;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    StartCoroutine(TurnRight());
                    turnedRight = true;
                }
            }
        }
    }

    IEnumerator TurnLeft()
    {
        float rotateTime = turnSpeed;
        while (rotateTime > 0)
        {
            rotateTime -= Time.deltaTime;
            transform.Rotate(0, -115 * Time.deltaTime, 0);
            yield return null;
        }
        SetDirection();
    }

    IEnumerator TurnRight()
    {
        float rotateTime = turnSpeed;
        while (rotateTime > 0)
        {
            rotateTime -= Time.deltaTime;
            transform.Rotate(0, 115 * Time.deltaTime, 0);
            yield return null;
        }
        SetDirection();
    }

    public bool IsMoving()
    {
        if (turnedLeft || turnedRight || movedForward) return true;
        return false;
    }

    public void SetDirection()
    {
        if (turnedLeft) rotationAccumulation--;
        else if (turnedRight) rotationAccumulation++;

        transform.eulerAngles = new Vector3(0, rotationAccumulation * 90, 0);

        turnedLeft = false;
        turnedRight = false;
    }

    /// <summary>
    /// Because animators aren't perfect
    /// </summary>
    public void FinishAnimating()
    {
        if (movedForward)
        {
            movedForward = false;
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        }

        SetDirection();
    }

    public bool WallInFront()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.forward, out hit, wallCheckDistance, wallLayer))
        {
            Debug.DrawRay(transform.position, transform.forward * wallCheckDistance, Color.yellow);
            Debug.Log("Did Hit");
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
            return false;
        }
    }

    public void FadeInVision()
    {
        anim.SetTrigger("FadeIn");
    }

    public void FadeOutVision()
    {
        anim.SetTrigger("FadeOut");
    }
}