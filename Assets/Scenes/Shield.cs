using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(bool isRightShield)
    {
        Vector2 position = Vector2.zero;

        if (isRightShield)
        {
            //Place the shield to right of the screen
            position = new Vector2(PongManager.topRight.x, 0);
        } else {
            //Place the shield to left of the screen
            position = new Vector2(PongManager.bottomLeft.x, 0);
        }

        //Update the position of the shield
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
