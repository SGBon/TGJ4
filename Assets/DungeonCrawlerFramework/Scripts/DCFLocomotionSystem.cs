using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCFLocomotionSystem : MonoBehaviour
{
    [SerializeField]
    LayerMask wallLayer;

    [SerializeField]
    float wallCheckDistance = 5;

    [SerializeField]
    float moveSpeed = 1;

    [SerializeField]
    float turnSpeed = 1;

    Animator anim;

    Vector3 previousPos;
    float previousRot;

    bool movedForward;

    [SerializeField]
    int rotationAccumulation = 0;

    [SerializeField]
    int xAccumulation = 0;
    [SerializeField]
    int zAccumulation = 0;

    bool turnedLeft;
    bool turnedRight;

    AudioManager am;

    Coroutine moveForward;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        am = AudioManager.GetInstance();

        xAccumulation = Mathf.RoundToInt(transform.position.x) / 10;
        zAccumulation = Mathf.RoundToInt(transform.position.z) / 10;
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
                    if (moveForward != null)
                    {
                        StopCoroutine(moveForward);
                    }
                    moveForward = StartCoroutine(MoveForward());
                    am.PlaySoundOnce(AudioManager.Sound.Move);
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

    IEnumerator MoveForward()
    {
        float moveTime = 1;
        while (moveTime > 0)
        {
            moveTime -= Time.deltaTime * moveSpeed;
            transform.position += transform.forward * 10 * moveSpeed * Time.deltaTime;
            yield return null;
        }
        FinishAnimating();
    }

    IEnumerator TurnLeft()
    {
        float rotateTime = 1;
        while (rotateTime > 0)
        {
            rotateTime -= Time.deltaTime * turnSpeed;
            transform.Rotate(0, -90 * Time.deltaTime * turnSpeed, 0);
            yield return null;
        }
        SetDirection();
    }

    IEnumerator TurnRight()
    {
        float rotateTime = 1;
        while (rotateTime > 0)
        {
            rotateTime -= Time.deltaTime * turnSpeed;
            transform.Rotate(0, 90 * Time.deltaTime * turnSpeed, 0);
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

        transform.eulerAngles = new Vector3(0, Mathf.RoundToInt(rotationAccumulation * 90), 0);

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
            transform.eulerAngles = new Vector3(0, Mathf.RoundToInt(transform.eulerAngles.y), 0);
            if (Mathf.Approximately(transform.eulerAngles.y, 0) || Mathf.Approximately(transform.eulerAngles.y, 360)) zAccumulation++;
            else if (Mathf.Approximately(transform.eulerAngles.y, 270)) xAccumulation--;
            else if (Mathf.Approximately(transform.eulerAngles.y, 90)) xAccumulation++;
            else if (Mathf.Approximately(transform.eulerAngles.y, 180)) zAccumulation--;

            movedForward = false;
            //transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
            transform.position = new Vector3(xAccumulation * 10, Mathf.RoundToInt(transform.position.y), zAccumulation * 10);
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
