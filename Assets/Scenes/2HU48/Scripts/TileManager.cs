using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	public float tileInterval;
	public GameObject tilePrefab;

	private Stack<GameObject> tilePool = new Stack<GameObject>(POOL_SIZE);
	private readonly static byte POOL_SIZE = 16;
	private byte total_instantiated_tiles = 0;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	/**
	 * Retrieve a new tile from the pool. If the pool contains no tiles and we have not yet instantiated POOL_SIZE tiles
	 * then instantiate a new tile. If we already have POOL_SIZE tiles in use then return null.
	 */
	public GameObject RetrieveTile()
	{
		if (tilePool.Count > 0)
		{
			GameObject tile = tilePool.Pop();
			tile.SetActive(true);
			return tile;
		}
		else
		{
			if (total_instantiated_tiles < POOL_SIZE)
			{
				GameObject newTile = Instantiate(tilePrefab, transform);
				total_instantiated_tiles++;
				return newTile;
			}
			else
			{
				return null;
			}
		}
	}

	/**
	 *  add a tile to the pool
	 */
	public void InternTile(GameObject tile)
	{
		tile.SetActive(false);
		tilePool.Push(tile);
	}
}
