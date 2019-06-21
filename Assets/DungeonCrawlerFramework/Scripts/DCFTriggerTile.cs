using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCFTriggerTile : MonoBehaviour
{
    [SerializeField]
    bool onlyTriggerOnce;

    [SerializeField]
    UnityEngine.Events.UnityEvent tileStepEvent;

    bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if ((onlyTriggerOnce && !activated) || !onlyTriggerOnce)
        {
            tileStepEvent.Invoke();
        }
    }
}
