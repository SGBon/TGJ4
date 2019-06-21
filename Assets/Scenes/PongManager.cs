using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongManager : MonoBehaviour
{

    public Orb orb;
    public Shield shield;

    public static Vector2 bottomLeft;
    public static Vector2 topRight;

    // Start is called before the first frame update
    void Start()
    {
        //Convert screen's coordinate to game's cordinate
        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        //Create energy orb
        Instantiate(orb);

        //Create two shields
        Shield shield1 = Instantiate(shield) as Shield;
        Shield shield2 = Instantiate(shield) as Shield;
        shield1.Init(true); //right shield
        shield2.Init(false); //left shield
    }
}
