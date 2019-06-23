using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
	public TextMesh valueLabel;
	private int value = 0;

	public float growSpeed;
	public float maxSize;
	public float minSize;
	private bool growing = false;

	// position to interpolate to when updating and not at the position
	public float interpSpeed;
	private Vector3 interpPosition;
	private bool interpolating = false;
	private float interpThreshold = 0.5f;
	private bool noInterp = false;

	private MeshRenderer meshRenderer;
	public Color lowColour;
	public Color midColour;
	public Color highColour;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (growing)
		{
			float dscale = growSpeed * Time.deltaTime;
			transform.localScale += new Vector3(dscale, dscale);
			if (transform.localScale.x >= maxSize)
			{
				transform.localScale = new Vector3(maxSize, maxSize, 0.1f);
				growing = false;
			}
		}

		if (interpolating)
		{
				float dx = Time.deltaTime * interpSpeed;
				Vector3 direction = (interpPosition - transform.position).normalized;
				transform.Translate(direction * dx);
			
			if ((interpPosition - transform.position).magnitude < interpThreshold) {
				interpolating = false;
				transform.position = interpPosition;
			}
		}
	}

	public void SetValue(int value)
	{
		if(meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}

		this.value = value;
		valueLabel.text = "" + value;

		if(value <= 32)
		{
			meshRenderer.material.color = lowColour;	
		}else if(value <= 128)
		{
			meshRenderer.material.color = midColour;
		}
		else
		{
			meshRenderer.material.color = highColour;
		}
	}

	public int GetValue()
	{
		return value;
	}

	public void Shrink()
	{
		transform.localScale = new Vector3(minSize, minSize, 0.1f);
	}

	public void Grow()
	{
		growing = true;
	}

	/**
	 * pulse the size
	 */
	public void Pulse()
	{
		growing = true;
		Vector3 scale = transform.localScale;
		scale.x = maxSize * 0.6f;
		scale.y = maxSize * 0.6f;
		transform.localScale = scale;
	}

	/**
	 * set the position of this tile, will interpolate to the position on every update unless the noInterp flag is set
	 */
	public void SetPosition(Vector3 position)
	{
		interpPosition = position;
		if (noInterp)
		{
			transform.position = interpPosition;
			noInterp = false;
		}
		else
		{
			interpolating = true;
		}
	}

	/**
	 * Call this to finish interpolating and just translate the transform to the required position
	 */
	public void FinishInteroplation()
	{
		if (interpolating)
		{
			transform.position = interpPosition;
			interpolating = false;
		}
	}

	/**
	 * Call this to notify to not interpolate the next position change
	 */
	public void DisableNextInterp()
	{
		noInterp = true;
	}
}
