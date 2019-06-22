using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
	public TextMesh valueLabel;
	private int value = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void setValue(int value)
	{
		this.value = value;
		valueLabel.text = ""+value;
	}

	public int getValue()
	{
		return value;
	}
}
