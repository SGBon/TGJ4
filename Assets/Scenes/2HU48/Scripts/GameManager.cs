using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public TileManager tileManager;

	private GameObject[] gameGrid;
	private static readonly int GRID_WIDTH = 4;
	private static readonly int GRID_SIZE = GRID_WIDTH * GRID_WIDTH;
	private static readonly float TWO_CHANCE = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
		// create our game grid
		gameGrid = new GameObject[GRID_SIZE];
		for(int i = 0; i < GRID_SIZE; ++i)
		{
			gameGrid[i] = null;
		}

		// initialize first two tiles
		generateTile();
		generateTile();

		repositionTiles();
	}

	private void generateTile()
	{
		GameObject tile = tileManager.retrieveTile();
		if (tile != null)
		{
			initializeTile(tile);
			List<int> openSpaces = getOpenSpaces();
			int index = Random.Range(0, openSpaces.Count);
			int space = openSpaces[index];
			gameGrid[space] = tile;
		}
	}

	enum Direction{
		UP,
		DOWN,
		LEFT,
		RIGHT,
		NONE
	}

    // Update is called once per frame
    void Update()
    {
		Direction direction = getNextInput();
		if(direction != Direction.NONE)
		{
			Debug.Log("direction: " + direction);
			if (slideTiles(direction))
			{
				generateTile();
			}
			repositionTiles();
		}
    }

	/**
	 * grabs the next input ie. what direction the user presses
	 */
	private Direction getNextInput()
	{
		if (Input.GetKeyUp(KeyCode.RightArrow))
		{
			return Direction.RIGHT;
		}
		else if (Input.GetKeyUp(KeyCode.LeftArrow))
		{
			return Direction.LEFT;
		}
		else if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			return Direction.UP;
		}
		else if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			return Direction.DOWN;
		}
		else
		{
			return Direction.NONE;
		}
	}

	private struct SlideInfo
	{
		public GameObject tile;
		public int oldIndex;

		public SlideInfo(GameObject tile, int oldIndex)
		{
			this.tile = tile;
			this.oldIndex = oldIndex;
		}
	}

	/**
	 * sweep over the grid translating tiles towards the input direction and merging tiles with the same value
	 * TODO: Refactor as there's obvious duplication going on in each if branch
	 */
	private bool slideTiles(Direction direction)
	{
		bool anyslide = false; // this flag will be set to true if any tiles were moved during the execution of this function

		if (direction == Direction.RIGHT)
		{
			for (int y = 0; y < GRID_WIDTH; ++y)
			{
				Stack<SlideInfo> mergeStack = new Stack<SlideInfo>();
				for (int x = 0; x < GRID_WIDTH; ++x)
				{
					int index = y * GRID_WIDTH + x;
					GameObject tile = gameGrid[index];
					if (tile != null)
					{
						mergeStack.Push(new SlideInfo(tile,index));
						gameGrid[index] = null; // temporarily remove the tile from the grid spaces
					}
				}

				if (mergeStack.Count > 0)
				{
					SlideInfo first = mergeStack.Pop();
					TileController firstTC = first.tile.GetComponent<TileController>();
					int firstIndex = y * GRID_WIDTH + (GRID_WIDTH - 1);
					gameGrid[firstIndex] = first.tile;
					if (firstIndex != first.oldIndex)
					{
						anyslide = true;
					}

					for(int x = GRID_WIDTH - 2; mergeStack.Count > 0; --x)
					{
						SlideInfo second = mergeStack.Pop();
						TileController secondTC = second.tile.GetComponent<TileController>();
						if (firstTC.getValue() == secondTC.getValue())
						{
							firstTC.setValue(firstTC.getValue() * 2);
							tileManager.internTile(second.tile);
						}
						else
						{
							int secondIndex = y * GRID_WIDTH + x;
							gameGrid[secondIndex] = second.tile;
							if (!anyslide && second.oldIndex != secondIndex)
							{
								anyslide = true;
							}
							first = second;
							firstTC = first.tile.GetComponent<TileController>();
						}
					}
				}
			}
		}
		else if (direction == Direction.LEFT)
		{
			for (int y = 0; y < GRID_WIDTH; ++y)
			{
				Stack<SlideInfo> mergeStack = new Stack<SlideInfo>();
				for (int x = GRID_WIDTH - 1; x >= 0; --x)
				{
					int index = y * GRID_WIDTH + x;
					GameObject tile = gameGrid[index];
					if (tile != null)
					{
						mergeStack.Push(new SlideInfo(tile, index));
						gameGrid[index] = null; // temporarily remove the tile from the grid spaces
					}
				}

				if (mergeStack.Count > 0)
				{
					SlideInfo first = mergeStack.Pop();
					TileController firstTC = first.tile.GetComponent<TileController>();
					int firstIndex = y * GRID_WIDTH;
					gameGrid[firstIndex] = first.tile;

					if (firstIndex != first.oldIndex)
					{
						anyslide = true;
					}

					for (int x = 1; mergeStack.Count > 0; ++x)
					{
						SlideInfo second = mergeStack.Pop();
						TileController secondTC = second.tile.GetComponent<TileController>();
						if (firstTC.getValue() == secondTC.getValue())
						{
							firstTC.setValue(firstTC.getValue() * 2);
							tileManager.internTile(second.tile);
						}
						else
						{
							int secondIndex = y * GRID_WIDTH + x;
							gameGrid[secondIndex] = second.tile;
							if (!anyslide && second.oldIndex != secondIndex)
							{
								anyslide = true;
							}
							first = second;
							firstTC = first.tile.GetComponent<TileController>();
						}
					}
				}
			}
		}
		else if (direction == Direction.UP)
		{

		}
		else if (direction == Direction.DOWN)
		{

		}

		return anyslide;
	}

	/**
	 * Find all the spaces that are unnoccupied in the grid
	 */
	private List<int> getOpenSpaces()
	{
		List<int> list = new List<int>();
		for(int i = 0; i < GRID_SIZE; ++i)
		{
			if(gameGrid[i] == null)
			{
				list.Add(i);
			}
		}
		return list;
	}

	/**
	 * update the visual position of all tiles on the
	 */
	private void repositionTiles()
	{
		float interval = tileManager.tileInterval;
		for(int i = 0; i < GRID_SIZE; ++i)
		{
			GameObject tile = gameGrid[i];

			if (tile != null)
			{
				int x = i % GRID_WIDTH;
				int y = i / GRID_WIDTH;
				tile.transform.position = new Vector3(x*interval,y*interval,tile.transform.position.z);
				Debug.Log(x + ":" + y + " " + tile.transform.position);
			}
		}
	}

	/**
	 * initialize starting values on a tile
	 */
	private void initializeTile(GameObject tile)
	{
		int value = Random.Range(0.0f, 1.0f) < TWO_CHANCE ? 2 : 4;
		tile.GetComponent<TileController>().setValue(value);
	}
}
