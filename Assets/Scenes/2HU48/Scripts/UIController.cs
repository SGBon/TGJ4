using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public Text message;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetMessageEnabled(bool enabled)
	{
		message.transform.parent.gameObject.SetActive(enabled);
	}

	public void SetMessage(string text)
	{
		message.text = text;
	}
}
