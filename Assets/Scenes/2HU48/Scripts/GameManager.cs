using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public TileManager tileManager;

	private GameObject[] gameGrid;
	private static readonly int GRID_WIDTH = 4;
	private static readonly int GRID_SIZE = GRID_WIDTH * GRID_WIDTH;
	[Range(0,1)]
	public float TWO_CHANCE;

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
		tile.GetComponent<TileController>().disableNextInterp();
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

		int yStart;
		int yEnd;
		int dy;
		int xStart;
		int xEnd;
		int dx;

		if (direction == Direction.RIGHT || direction == Direction.LEFT)
		{
			yStart = 0;
			yEnd = GRID_WIDTH;
			dy = 1;

			for (int y = yStart; y < yEnd; y += dy)
			{
				Stack<SlideInfo> mergeStack = new Stack<SlideInfo>();

				if (direction == Direction.LEFT)
				{
					xStart = GRID_WIDTH - 1;
					xEnd = -1;
					dx = -1;
				}
				else
				{
					xStart = 0;
					xEnd = GRID_WIDTH;
					dx = 1;
				}

				for (int x = xStart; x != xEnd; x += dx)
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
					int firstIndex;

					if (direction == Direction.RIGHT)
					{
						firstIndex = y * GRID_WIDTH + (GRID_WIDTH - 1);
						xStart = GRID_WIDTH - 2;
						dx = -1;
					}
					else
					{
						firstIndex = y * GRID_WIDTH;
						xStart = 1;
						dx = 1;
					}

					gameGrid[firstIndex] = first.tile;
					if (!anyslide && firstIndex != first.oldIndex)
					{
						anyslide = true;
					}


					for (int x = xStart; mergeStack.Count > 0; x += dx)
					{
						SlideInfo second = mergeStack.Pop();
						TileController secondTC = second.tile.GetComponent<TileController>();

						if (firstTC.getValue() == secondTC.getValue())
						{
							firstTC.setValue(firstTC.getValue() * 2);
							tileManager.internTile(second.tile);
							firstTC.pulse();
							if (!anyslide)
							{
								anyslide = true;
							}

							if (mergeStack.Count > 0)
							{
								first = mergeStack.Pop();
								firstTC = first.tile.GetComponent<TileController>();
								firstIndex = y * GRID_WIDTH + x;
								gameGrid[firstIndex] = first.tile;
							}
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
		else if (direction == Direction.UP || direction == Direction.DOWN)
		{
			xStart = 0;
			xEnd = GRID_WIDTH;
			dx = 1;

			for (int x = xStart; x != xEnd; x += dx)
			{
				Stack<SlideInfo> mergeStack = new Stack<SlideInfo>();

				if (direction == Direction.DOWN)
				{
					yStart = GRID_WIDTH - 1;
					yEnd = -1;
					dy = -1;
				}
				else
				{
					yStart = 0;
					yEnd = GRID_WIDTH;
					dy = 1;
				}

				for (int y = yStart; y != yEnd; y += dy)
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
					int firstIndex;

					if (direction == Direction.DOWN)
					{
						firstIndex = x;
						yStart = 1;
						dy = 1;
					}
					else
					{
						firstIndex = GRID_WIDTH * (GRID_WIDTH - 1) + x;
						yStart = GRID_WIDTH - 2;
						dy = -1;
					}

					gameGrid[firstIndex] = first.tile;
					if (!anyslide && firstIndex != first.oldIndex)
					{
						anyslide = true;
					}


					for (int y = yStart; mergeStack.Count > 0; y += dy)
					{
						SlideInfo second = mergeStack.Pop();
						TileController secondTC = second.tile.GetComponent<TileController>();

						if (firstTC.getValue() == secondTC.getValue())
						{
							firstTC.setValue(firstTC.getValue() * 2);
							firstTC.pulse();
							tileManager.internTile(second.tile);
							if (!anyslide)
							{
								anyslide = true;
							}
							if (mergeStack.Count > 0)
							{
								first = mergeStack.Pop();
								firstTC = first.tile.GetComponent<TileController>();
								firstIndex = y * GRID_WIDTH + x;
								gameGrid[firstIndex] = first.tile;
							}
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
				TileController tc = tile.GetComponent<TileController>();
				tc.finishInteroplation();
				tc.setPosition(new Vector3(x * interval, y * interval, tile.transform.position.z));
			}
		}
	}

	/**
	 * initialize starting values on a tile
	 */
	private void initializeTile(GameObject tile)
	{
		int value = Random.Range(0.0f, 1.0f) < TWO_CHANCE ? 2 : 4;
		TileController t = tile.GetComponent<TileController>();
		t.setValue(value);
		t.shrink();
		t.grow();
	}
}
