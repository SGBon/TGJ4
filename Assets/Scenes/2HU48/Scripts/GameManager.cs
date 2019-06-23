using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public TileManager tileManager;

	private GameObject[] gameGrid;
	private static readonly int GRID_WIDTH = 4;
	private static readonly int GRID_SIZE = GRID_WIDTH * GRID_WIDTH;
	[Range(0,1)]
	public float TWO_CHANCE;
	public int winCondition; // Tile to reach to win

	private GameState gameState;

	public UIController ui;
	public float postWinTime = 5.0f; // time in seconds after winning before changing scene or after losing before resetting the game
	private float postWinTimer;

    // Start is called before the first frame update
    void Start()
    {
		gameGrid = new GameObject[GRID_SIZE];
		ResetGame();
	}

	enum GameState
	{
		PLAYING,
		WON,
		LOST
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
		if (gameState == GameState.PLAYING)
		{
			Direction direction = GetNextInput();
			if (direction != Direction.NONE)
			{
				if (SlideTiles(direction))
				{
					GenerateTile();
				}
				RepositionTiles();

				if (gameState != GameState.WON && !CheckValidMoves())
				{
					SetLost();
				}
			}
		}
		else if(gameState == GameState.WON)
		{
			if (postWinTimer > 0)
			{
				postWinTimer -= Time.deltaTime;
				if (postWinTimer <= 0)
				{
					SceneManager.LoadScene(2);
				}
			}
		}
		else if(gameState == GameState.LOST)
		{
			if (postWinTimer > 0)
			{
				postWinTimer -= Time.deltaTime;
				if (postWinTimer <= 0)
				{
					ResetGame();
				}
			}
		}
    }

	/**
	 * grabs the next input ie. what direction the user presses
	 */
	private Direction GetNextInput()
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
	private bool SlideTiles(Direction direction)
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

						if (firstTC.GetValue() == secondTC.GetValue())
						{
							firstTC.SetValue(firstTC.GetValue() * 2);
							tileManager.InternTile(second.tile);
							firstTC.Pulse();
							if (!anyslide)
							{
								anyslide = true;
							}

							//check win condition
							if(firstTC.GetValue() >= winCondition)
							{
								SetWon();
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

						if (firstTC.GetValue() == secondTC.GetValue())
						{
							firstTC.SetValue(firstTC.GetValue() * 2);
							firstTC.Pulse();
							tileManager.InternTile(second.tile);
							if (!anyslide)
							{
								anyslide = true;
							}

							//check win condition
							if (firstTC.GetValue() >= winCondition)
							{
								SetWon();
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
	private List<int> GetOpenSpaces()
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
	private void RepositionTiles()
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
				tc.FinishInteroplation();
				tc.SetPosition(new Vector3(x * interval, y * interval, tile.transform.position.z));
			}
		}
	}

	/**
	 * initialize starting values on a tile
	 */
	private void InitializeTile(GameObject tile)
	{
		int value = Random.Range(0.0f, 1.0f) < TWO_CHANCE ? 2 : 4;
		TileController t = tile.GetComponent<TileController>();
		t.SetValue(value);
		t.Shrink();
		t.Grow();
	}

	/**
	 * Sweeps the grid to check if there are any valid moves to make. If there are, then return true, false othewise.
	 * A possible valid move is one where two adjacent tiles share the same value (ie. can be merged) or there is a grid space that is empty
	 * This function should only need to be used when the grid is full but handles cases when it isn't full.
	 */
	private bool CheckValidMoves()
	{
		for (int y = 0; y < GRID_WIDTH; ++y)
		{
			for (int x = 0; x < GRID_WIDTH; ++x)
			{
				// if there's an empty space then there are still valid moves to make
				GameObject currentTile = gameGrid[y * GRID_WIDTH + x];
				if (currentTile == null)
				{
					return true;
				}
				else
				{
					TileController tc = currentTile.GetComponent<TileController>();
					// if there are grid spaces to the right of the current tile then check the tile to the right
					if(x < GRID_WIDTH - 1)
					{
						GameObject rightTile = gameGrid[y * GRID_WIDTH + (x + 1)];
						if(rightTile == null || rightTile.GetComponent<TileController>().GetValue() == tc.GetValue())
						{
							return true;
						}
					}

					// if there are grid spaces under the current tile then check the tile there
					if(y < GRID_WIDTH - 1)
					{
						GameObject downTile = gameGrid[(y + 1) * GRID_WIDTH + x];
						if(downTile == null || downTile.GetComponent<TileController>().GetValue() == tc.GetValue())
						{
							return true;
						}
					}
				}
			}
		}

		return false;
	}

	private void SetWon()
	{
		gameState = GameState.WON;
		ui.SetMessageEnabled(true);
		ui.SetMessage("Puzzle Solved!");
	}

	private void SetLost()
	{
		gameState = GameState.LOST;
		ui.SetMessageEnabled(true);
		ui.SetMessage("Puzzle Failed!");
	}

	/**
	 * Clear out the gamegrid and intern all the tiles.
	 * Generate 2 initial tiles
	 */
	private void ResetGame()
	{
		// create our game grid
		for (int i = 0; i < GRID_SIZE; ++i)
		{
			GameObject tile = gameGrid[i];
			if (tile != null)
			{
				gameGrid[i] = null;
				tileManager.InternTile(tile);
			}
		}

		// initialize first two tiles
		GenerateTile();
		GenerateTile();

		RepositionTiles();

		ui.SetMessageEnabled(false);
		gameState = GameState.PLAYING;
		postWinTimer = postWinTime;
	}

	private void GenerateTile()
	{
		GameObject tile = tileManager.RetrieveTile();
		tile.GetComponent<TileController>().DisableNextInterp();
		if (tile != null)
		{
			InitializeTile(tile);
			List<int> openSpaces = GetOpenSpaces();
			int index = Random.Range(0, openSpaces.Count);
			int space = openSpaces[index];
			gameGrid[space] = tile;
		}
	}
}
