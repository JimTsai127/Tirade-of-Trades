using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayLevel : MonoBehaviour {

	// Game Objects
	public GameObject gainBlock;
	public GameObject lossBlock;
	public GameObject bankruptBlock;
	public GameObject goldbar;
	public GameObject moneybag;
	public GameObject wallet;
	public GameObject paperbill;
	public GameObject coin;
	public GameObject playAreaBackground;
	public GameObject gameOverPanel;
	public GameObject nextLevelPanel;
	public GameObject speedHackPanel;

	// UI Objects
	private Text profitText;
	private Text levelText;
	private Text movesLeftText;
	private Text goalText;
	private Text roiText;
	private Text newsText;

	// PlayArea details
	private int width = 9;
	private int height = 6;
	private int offsetX = -8;
	private int offsetY = -4;
	private float spacing = 1.2F;
	private bool screenEnabled = true;
	private bool matchEnabled = true;
	private bool isPaused = false;

	// Game Details
	private int currentLevel = 1;
	private float currentScore = 0F;
	private int currentMovesLeft = 0;
	private float profitGoal = 0F;
	private float stockROI = 0F;
	private string currentNews = "Stock market opened. Happy trading!";
	private int currentEvent = 0;

	// Array of blocks
	private GameObject[,] blocksArray2D;
	private GameObject[,] symbolsArray2D;

	// Array of score markers to be used when calculating scores per match
	private string[,] scoreSymbols2D;
	private string[,] scoreBlocks2D;

	// Spawn Chance Details
	private float goldbarChance = 0.1F;
	private float moneybagChance = 0.15F;
	private float walletChance = 0.2F;
	private float paperbillChance = 0.25F;
	private float coinChance = 0.3F;
	// Example: [ProfitBlockChance, LossBlockChance, BankruptBlockChance]
	// Add future block type chances to the end of the list
	private float[] blockChance = new float[] { 0F, 0F, 0F };

	// Previously and currently pressed block
	private GameObject previousBlock;
	private GameObject currentBlock;

	// Block Types
	private enum BlockType {
		UnplayableArea = 999,
		EmptyArea = 0,
		Profit = 1,
		Loss = -1,
		Bankrupt = -5
	}

	// Symbol Types
	private enum SymbolType {
		Coin = 1,
		Paperbill = 2,
		Wallet = 3,
		Moneybag = 4,
		Goldbar = 5
	}

	// Spawn the initial blocks for the level
	void InitialSpawn() {
		// Create the matrices of the play area
		blocksArray2D = new GameObject[height, width];
		symbolsArray2D = new GameObject[height, width];
		scoreSymbols2D = new string[symbolsArray2D.GetLength(0), symbolsArray2D.GetLength(1)];
		scoreBlocks2D = new string[blocksArray2D.GetLength(0), blocksArray2D.GetLength(1)];

		for (int c = 0; c < width; c++) {
			for (int r = 0; r < height; r++) {
				// Randomly generate and store the next blocks and symbols 
				BlockType nextBlockType = NextBlock();
				SymbolType nextSymbolType = NextSymbol();

				blocksArray2D[r, c] = StoreBlock(r, c, nextBlockType);
				symbolsArray2D[r, c] = StoreSymbol(r, c, nextSymbolType);
			}
		}
		
	}

	// Return the game object representing the block type, x is column, y is row
	GameObject StoreBlock(int y, int x, BlockType blockType) {
		GameObject currentGameObject;
		switch (blockType) {
			case BlockType.Profit:
				currentGameObject = Instantiate(gainBlock, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
			case BlockType.Loss:
				currentGameObject = Instantiate(lossBlock, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
			case BlockType.Bankrupt:
				currentGameObject = Instantiate(bankruptBlock, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
			default:
				currentGameObject = Instantiate(gainBlock, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
		}
		currentGameObject.name = y + "," + x;
		return currentGameObject;
	}

	// Return the game object representing the symbol type, x is column, y is row
	GameObject StoreSymbol(int y, int x, SymbolType symbolType) {
		GameObject currentGameObject;
		switch (symbolType) {
			case SymbolType.Coin:
				currentGameObject = Instantiate(coin, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
			case SymbolType.Paperbill:
				currentGameObject = Instantiate(paperbill, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
			case SymbolType.Wallet:
				currentGameObject = Instantiate(wallet, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
			case SymbolType.Moneybag:
				currentGameObject = Instantiate(moneybag, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
			case SymbolType.Goldbar:
				currentGameObject = Instantiate(goldbar, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
			default:
				currentGameObject = Instantiate(coin, new Vector2(x * spacing + offsetX, y * spacing + offsetY), Quaternion.identity);
				break;
		}

		currentGameObject.name = y + "," + x;
		return currentGameObject;
	}

	// Determine the next block type
	BlockType NextBlock() {
		float rand = Random.value;
		BlockType nextBlock = BlockType.Profit;

		if (rand < blockChance[0]) {
			nextBlock = BlockType.Profit;
		}
		else if (rand < blockChance[0] + blockChance[1]) {
			nextBlock = BlockType.Loss;
		}
		else if (rand <= blockChance[0] + blockChance[1] + blockChance[2]) {
			nextBlock = BlockType.Bankrupt;
		}

		return nextBlock;
	}

	// Determine the next symbol
	SymbolType NextSymbol() {
		float rand = Random.value;
		SymbolType nextSymbol = SymbolType.Coin;

		// Coin
		if (rand < coinChance) {
			nextSymbol = SymbolType.Coin;
		}
		// Paperbill
		else if (rand < coinChance + paperbillChance) {
			nextSymbol = SymbolType.Paperbill;
		}
		// Wallet
		else if (rand < coinChance + paperbillChance + walletChance) {
			nextSymbol = SymbolType.Wallet;
		}
		// Moneybag
		else if (rand < coinChance + paperbillChance + walletChance + moneybagChance) {
			nextSymbol = SymbolType.Moneybag;
		}
		// Goldbar
		else if (rand <= coinChance + paperbillChance + walletChance + moneybagChance + goldbarChance) {
			nextSymbol = SymbolType.Goldbar;
		}

		return nextSymbol;
	}

	// Set the block chance for the given level
	void SetBlockChance(int currentLevel) {
		// Probabilities of each block spawning (starting from lvl 1)
		// { GainBlock, LossBlock, BankruptBlock } <- Add more blocks to the end
		// Make sure the probabilities add up to 100%.
		float[][] blockChancesOfAllLevels = new float[][] {
			new float[] { 0.95F, 0.05F, 0.00F }, // 1 (level)
			new float[] { 0.90F, 0.10F, 0.00F }, // 2
			new float[] { 0.85F, 0.15F, 0.00F }, // 3
			new float[] { 0.80F, 0.20F, 0.00F }, // 4
			new float[] { 0.75F, 0.24F, 0.01F }, // 5
			new float[] { 0.70F, 0.27F, 0.03F }, // 6
			new float[] { 0.65F, 0.30F, 0.05F }, // 7
			new float[] { 0.60F, 0.33F, 0.07F }, // 8
			new float[] { 0.55F, 0.36F, 0.09F }, // 9
			new float[] { 0.50F, 0.40F, 0.10F } // 10 and beyond
		};

		if(currentLevel < blockChancesOfAllLevels.GetLength(0)) {
			blockChance = blockChancesOfAllLevels[currentLevel - 1];
		} else {
			blockChance = blockChancesOfAllLevels[blockChancesOfAllLevels.GetLength(0) - 1];
		}
		
	}

	// Determine if there are any matches in the given board state
	bool MatchFound(GameObject[,] symbolsState, GameObject[,] blocksState) {
		bool matchFound = false;

		// Find matches
		for (int c = 0; c < symbolsState.GetLength(1); c++) {
			for (int r = 0; r < symbolsState.GetLength(0); r++) {
				GameObject originSymbol = symbolsState[r, c]; // Origin symbol
				GameObject originBlock = blocksState[r, c]; // Origin block

				if (r < symbolsState.GetLength(0) - 2) {
					GameObject oneDownSymbol = symbolsState[r + 1, c]; // One square below origin symbol
					GameObject oneDownBlock = blocksState[r + 1, c]; // One square below origin block
					GameObject twoDownSymbol = symbolsState[r + 2, c]; // Two squares below origin symbol
					GameObject twoDownBlock = blocksState[r + 2, c]; // Two squares below origin block

					if(originBlock != null
						&& originSymbol != null
						&& oneDownSymbol != null
						&& oneDownBlock != null
						&& twoDownSymbol != null
						&& twoDownBlock != null) {
						// Vertical match found, set score board state
						if (originSymbol.tag == oneDownSymbol.tag && oneDownSymbol.tag == twoDownSymbol.tag) {
							matchFound = true;
							// Symbols score
							scoreSymbols2D[r, c] = originSymbol.tag;
							scoreSymbols2D[r + 1, c] = oneDownSymbol.tag;
							scoreSymbols2D[r + 2, c] = twoDownSymbol.tag;
							// Blocks score
							scoreBlocks2D[r, c] = originBlock.tag;
							scoreBlocks2D[r + 1, c] = oneDownBlock.tag;
							scoreBlocks2D[r + 2, c] = twoDownBlock.tag;
						}
					}
				}

				if (c < symbolsState.GetLength(1) - 2) {
					GameObject oneRightSymbol = symbolsState[r, c + 1]; // One square to right of origin symbol
					GameObject oneRightBlock = blocksState[r, c + 1]; // One square to right of origin block
					GameObject twoRightSymbol = symbolsState[r, c + 2]; // Two squares to right of origin symbol
					GameObject twoRightBlock = blocksState[r, c + 2]; // Two squares to right of origin block

					if(oneRightSymbol != null 
						&& oneRightBlock != null 
						&& twoRightSymbol != null 
						&& twoRightBlock != null
						&& originSymbol != null
						&& originBlock != null) {
						// Horizontal match found, set score board state
						if (originSymbol.tag == oneRightSymbol.tag && oneRightSymbol.tag == twoRightSymbol.tag) {
							matchFound = true;
							// Symbols score
							scoreSymbols2D[r, c] = originSymbol.tag;
							scoreSymbols2D[r, c + 1] = oneRightSymbol.tag;
							scoreSymbols2D[r, c + 2] = twoRightSymbol.tag;
							// Blocks score
							scoreBlocks2D[r, c] = originBlock.tag;
							scoreBlocks2D[r, c + 1] = oneRightBlock.tag;
							scoreBlocks2D[r, c + 2] = twoRightBlock.tag;
						}
					}
				}
			}
		}

		return matchFound;
	}

	// Calculate the net profit, given the symbol and block score states
	float CalculateScore(string[,] scoreSymbolsState, string[,] scoreBlocksState) {
		// Variables to keep track of profits of each type
		float netProfit = 0F;
		float coinProfit = 0F;
		float paperbillProfit = 0F;
		float walletProfit = 0F;
		float goldbarProfit = 0F;
		float moneybagProfit = 0F;

		float coinTotal = 0F;
		float paperbillTotal = 0F;
		float walletTotal = 0F;
		float goldbarTotal = 0F;
		float moneybagTotal = 0F;

		float coinMultiplier = 0F;
		float paperbillMultiplier = 0F;
		float walletMultiplier = 0F;
		float goldbarMultiplier = 0F;
		float moneybagMultiplier = 0F;

		float numOfCoins = 0F;
		float numOfPaperbills = 0F;
		float numOfWallets = 0F;
		float numOfGoldbars = 0F;
		float numOfMoneybags = 0F;

		// Calculate score based on each type
		for (int c = 0; c < scoreSymbolsState.GetLength(1); c++) {
			for (int r = 0; r < scoreSymbolsState.GetLength(0); r++) {
				if (scoreSymbolsState[r, c] != null && scoreBlocksState[r, c] != null) {
					switch(scoreSymbolsState[r, c]) {
						case "paperbill":
							paperbillTotal += (float) SymbolType.Paperbill;
							numOfPaperbills += 1F;
							paperbillMultiplier += CalculateMultiplier(scoreBlocksState[r, c]);
							break;
						case "goldbar":
							goldbarTotal += (float) SymbolType.Goldbar;
							numOfGoldbars += 1F;
							goldbarMultiplier += CalculateMultiplier(scoreBlocksState[r, c]);
							break;
						case "moneybag":
							moneybagTotal += (float) SymbolType.Moneybag;
							numOfMoneybags += 1F;
							moneybagMultiplier += CalculateMultiplier(scoreBlocksState[r, c]);
							break;
						case "wallet":
							walletTotal += (float) SymbolType.Wallet;
							numOfWallets += 1F;
							walletMultiplier += CalculateMultiplier(scoreBlocksState[r, c]);
							break;
						case "coin":
							coinTotal += (float) SymbolType.Coin;
							numOfCoins += 1F;
							coinMultiplier += CalculateMultiplier(scoreBlocksState[r, c]);
							break;
					}
				}
			}
		}

		// Calculation formula:
		// [Symbol value] * [Profit/Loss Multiplier] * [ROI of level]
		if (numOfCoins != 0F) {
			coinProfit = (float) SymbolType.Coin * coinMultiplier * stockROI;
		} else if (numOfPaperbills != 0F) {
			paperbillProfit = (float)SymbolType.Paperbill * paperbillMultiplier * stockROI;
		} else if (numOfWallets != 0F) {
			walletProfit = (float)SymbolType.Wallet * walletMultiplier * stockROI;
		} else if (numOfGoldbars != 0F) {
			goldbarProfit = (float)SymbolType.Goldbar * goldbarMultiplier * stockROI;
		} else if (numOfMoneybags != 0F) {
			moneybagProfit = (float)SymbolType.Moneybag * moneybagMultiplier * stockROI;
		}

		// Sum up all the profits and return the net profit
		netProfit = coinProfit + paperbillProfit + walletProfit + goldbarProfit + moneybagProfit;
		return netProfit;
	}

	float CalculateMultiplier(string blockName) {
		float finalMultiplier = 0F;

		switch (blockName) {
			case "bankruptBlock":
				finalMultiplier += (float)BlockType.Bankrupt;
				break;
			case "lossBlock":
				finalMultiplier += (float)BlockType.Loss;
				break;
			case "gainBlock":
				finalMultiplier += (float)BlockType.Profit;
				break;
		}
		
		return finalMultiplier;
	}

	// Swap two pieces and check for a match, otherwise unswap
	void SwapBlocks(GameObject previousBlock, GameObject currentBlock) {
		if(previousBlock != null && currentBlock != null) {
			// Break the "x,y" string apart
			string[] prevCoords = previousBlock.name.Split(',');
			string[] currCoords = currentBlock.name.Split(',');
			int prevX;
			int prevY;
			int currX;
			int currY;

			// Convert to numbers
			int.TryParse(prevCoords[0], out prevX);
			int.TryParse(prevCoords[1], out prevY);
			int.TryParse(currCoords[0], out currX);
			int.TryParse(currCoords[1], out currY);

			if(symbolsArray2D[currX, currY] != null && blocksArray2D[currX, currY] != null
				&& symbolsArray2D[prevX, prevY] != null && blocksArray2D[prevX, prevY] != null) {
				// Swap the names
				string tempSymbolName = symbolsArray2D[currX, currY].name;
				string tempBlockName = blocksArray2D[currX, currY].name;
				symbolsArray2D[currX, currY].name = symbolsArray2D[prevX, prevY].name;
				blocksArray2D[currX, currY].name = blocksArray2D[prevX, prevY].name;
				symbolsArray2D[prevX, prevY].name = tempSymbolName;
				blocksArray2D[prevX, prevY].name = tempBlockName;
				// Swap positions
				Vector2 tempSymbolPosition = symbolsArray2D[currX, currY].transform.position;
				Vector2 tempBlockPosition = blocksArray2D[currX, currY].transform.position;
				symbolsArray2D[currX, currY].transform.position = symbolsArray2D[prevX, prevY].transform.position;
				blocksArray2D[currX, currY].transform.position = blocksArray2D[prevX, prevY].transform.position;
				symbolsArray2D[prevX, prevY].transform.position = tempSymbolPosition;
				blocksArray2D[prevX, prevY].transform.position = tempBlockPosition;
				// Temporarily swap the blocks and check for matches
				GameObject tempSymbol = symbolsArray2D[currX, currY];
				GameObject tempBlock = blocksArray2D[currX, currY];
				symbolsArray2D[currX, currY] = symbolsArray2D[prevX, prevY];
				blocksArray2D[currX, currY] = blocksArray2D[prevX, prevY];
				symbolsArray2D[prevX, prevY] = tempSymbol;
				blocksArray2D[prevX, prevY] = tempBlock;

				// If a match is found, update score, play matching animation, replace and spawn new blocks
				if (MatchFound(symbolsArray2D, blocksArray2D)) {
					currentMovesLeft -= 1;
					movesLeftText.text = currentMovesLeft + "";
					StartCoroutine(UpdateBoards());

					// Every 3 moves, get a new event
					if(currentMovesLeft % 3 == 0) {
						System.Random ran = new System.Random();
						currentEvent = ran.Next(0, 10);
					}
					// Launch event
					LaunchEvent(currentEvent);
				}
				else {
					// Swap back the names
					symbolsArray2D[currX, currY].name = symbolsArray2D[prevX, prevY].name;
					blocksArray2D[currX, currY].name = blocksArray2D[prevX, prevY].name;
					symbolsArray2D[prevX, prevY].name = tempSymbolName;
					blocksArray2D[prevX, prevY].name = tempBlockName;

					// Swap back the positions
					symbolsArray2D[currX, currY].transform.position = symbolsArray2D[prevX, prevY].transform.position;
					blocksArray2D[currX, currY].transform.position = blocksArray2D[prevX, prevY].transform.position;
					symbolsArray2D[prevX, prevY].transform.position = tempSymbolPosition;
					blocksArray2D[prevX, prevY].transform.position = tempBlockPosition;

					// Swap back the blocks
					symbolsArray2D[currX, currY] = symbolsArray2D[prevX, prevY];
					blocksArray2D[currX, currY] = blocksArray2D[prevX, prevY];
					symbolsArray2D[prevX, prevY] = tempSymbol;
					blocksArray2D[prevX, prevY] = tempBlock;
				}
			}
		}
	}

	// Update the play board and scoreboard
	IEnumerator UpdateBoards() {
		matchEnabled = false;

		// Calculate and display score
		currentScore += CalculateScore(scoreSymbols2D, scoreBlocks2D);
		profitText.text = currentScore + "";

		// Replace the matched squares with random blocks and symbols
		for (int c = 0; c < scoreSymbols2D.GetLength(1); c++) {
			for (int r = 0; r < scoreSymbols2D.GetLength(0); r++) {
				if (scoreSymbols2D[r, c] != null) { // Matched square
					// Matching animation
					symbolsArray2D[r, c].GetComponent<Animator>().SetBool("isMatched", true);
					StartCoroutine(DestroyBlock(r, c));
				}
			}
		}

		yield return new WaitForSeconds(1.1F);
		matchEnabled = true;
	}

	// After a delay, destroy the symbol and block object and respawn another in their place
	// Also disable screen interaction during the delay
	IEnumerator DestroyBlock(int r, int c) {
		screenEnabled = false;

		// Destroy after animation is done
		yield return new WaitForSeconds(1F);
		if(symbolsArray2D[r, c] != null) {
			Destroy(symbolsArray2D[r, c]);
			symbolsArray2D[r, c] = null;
			scoreSymbols2D[r, c] = null; // Reset symbol score
		}
		if(blocksArray2D[r, c] != null) {
			Destroy(blocksArray2D[r, c]);
			blocksArray2D[r, c] = null;
			scoreBlocks2D[r, c] = null; // Reset block score
		}

		// Respawn with random blocks
		if (blocksArray2D[r, c] == null) {
			blocksArray2D[r, c] = StoreBlock(r, c, NextBlock());
		}
		if (symbolsArray2D[r, c] == null) {
			symbolsArray2D[r, c] = StoreSymbol(r, c, NextSymbol());
		}
		
		screenEnabled = true;
	}

	// Load the level
	void LevelLoad() {
		// Load the block chances for the given level
		SetBlockChance(currentLevel);

		// Load inital spawn of each level
		InitialSpawn();

		// Populate the UI text
		levelText.text = currentLevel + "";
		currentMovesLeft = currentLevel * 3 + 5;
		profitGoal = currentLevel * 50;
		movesLeftText.text = currentMovesLeft + "";
		goalText.text = profitGoal + "";
		currentScore = 0F;
		profitText.text = currentScore + "";
		stockROI = currentLevel;
		roiText.text = stockROI + "x";
		newsText.text = currentNews + "";
	}

	// Restart the level
	public void RestartGame() {
        // Hide the game over screen
        gameOverPanel.SetActive(false);

		//// Destroy any GameObjects remaining in play area
		//foreach(GameObject block in blocksArray2D) {
		//	if(block != null) {
		//		Destroy(block);
		//          }
		//      }
		//foreach (GameObject symbol in symbolsArray2D) {
		//	if (symbol != null) {
		//		Destroy(symbol);
		//	}
		//}

		//// Load the level
		//LevelLoad();

		SceneManager.LoadScene("StockMarketScreen");
	}

	// Go back to main menu
	public void MainMenu() {
		SceneManager.LoadScene("MainMenuScreen");
	}

	// Game over screen
	void GameOver() {
		// Show game over panel
		gameOverPanel.SetActive(true);
		isPaused = true;
    }

	// Go to the next level
	public void NextLevel() {
		nextLevelPanel.SetActive(false);
		currentLevel += 1; // Increment level
		SaveLevel(); // Save level
		RestartGame(); // Reboot the game
    }

	// Next level screen
	void NextLevelPanel() {
		gameOverPanel.SetActive(false); // In case the player wins because the matches are still happening
		nextLevelPanel.SetActive(true);
		isPaused = true;
	}

	// Save user level
	void SaveLevel() {
		PlayerPrefs.SetInt("PlayerLevel", currentLevel);
		PlayerPrefs.Save();
    }

	// Get user level
	void GetLevel() {
		int savedLevel = PlayerPrefs.GetInt("PlayerLevel");
		if(savedLevel > 0) {
			currentLevel = savedLevel;
		}
	}

	// Check if the game board bugged out
	bool IsBuggedOut() {
		for (int c = 0; c < symbolsArray2D.GetLength(1); c++) {
			for (int r = 0; r < symbolsArray2D.GetLength(0); r++) {
				if(symbolsArray2D[r, c] == null || blocksArray2D[r, c] == null) {
					return true;
                }
			}
		}
		return false;
    }

	// Speed hack screen
	void SpeedHackPanel() {
		speedHackPanel.SetActive(true);
		isPaused = true;
	}

	// Speed hack button resets game
	public void SpeedHack() {
		speedHackPanel.SetActive(false);
		RestartGame(); // Reboot the game
	}

	// Launches a new event which will shake up the play area with
	// new blocks, symbols, and/or headlines depending on which event
	void LaunchEvent(int currentEvent) {
		// Depending on the event, change the rate of spawn or blocks on field
		switch(currentEvent) {
			case 1: // Good news event (profit block spawn rate++)
				blockChance = new float[] { 1.00F, 0.00F, 0.00F };
				currentNews = "Stocks rally as the market heads towards a bull market.";
				break;
			case 2:	// Bad news event (higher red spawn rate)
				blockChance = new float[] { 0.00F, 1.00F, 0.00F };
				currentNews = "Stocks tank as the market heads towards a bear market.";
				break;
			case 3: // Short squeeze
				// Destroy all the blocks and replace with stripe pattern
				for(int r = 0; r < blocksArray2D.GetLength(0); r++) {
					for (int c = 0; c < blocksArray2D.GetLength(1); c++) {
						if (blocksArray2D[r, c] != null) {
							Destroy(blocksArray2D[r, c]);
							if(r % 2 == 0) {
								blocksArray2D[r, c] = StoreBlock(r, c, BlockType.Loss);
							} else {
								blocksArray2D[r, c] = StoreBlock(r, c, BlockType.Profit);
							}
						}
					}
				}
				currentNews = "Hedge funds have engaged in a short ladder attack against the market!";
                break;
			case 4: // Market crash
				// Destroy all the blocks and replace with loss blocks
				for (int r = 0; r < blocksArray2D.GetLength(0); r++) {
					for (int c = 0; c < blocksArray2D.GetLength(1); c++) {
						if (blocksArray2D[r, c] != null) {
							Destroy(blocksArray2D[r, c]);
							blocksArray2D[r, c] = StoreBlock(r, c, BlockType.Loss);
						}
					}
				}
				currentNews = "Hedge funds have artifically crashed the market!";
				break;
			case 5: // Gold crash
				// Replace all goldbars with coins
				for (int r = 0; r < symbolsArray2D.GetLength(0); r++) {
					for (int c = 0; c < symbolsArray2D.GetLength(1); c++) {
						if (symbolsArray2D[r, c] != null && symbolsArray2D[r, c].tag == "goldbar") {
							Destroy(symbolsArray2D[r, c]);
							symbolsArray2D[r, c] = StoreSymbol(r, c, SymbolType.Coin);
						}
					}
				}
				currentNews = "Market manipulators have started a gold crash!";
				break;
			case 6: // Delta hedge
				// Destroy all the blocks and replace with checkboard pattern
				for (int r = 0; r < blocksArray2D.GetLength(0); r++) {
					for (int c = 0; c < blocksArray2D.GetLength(1); c++) {
						if (blocksArray2D[r, c] != null) {
							Destroy(blocksArray2D[r, c]);
							if ((r + c) % 2 == 0) {
								blocksArray2D[r, c] = StoreBlock(r, c, BlockType.Loss);
							}
							else {
								blocksArray2D[r, c] = StoreBlock(r, c, BlockType.Profit);
							}
						}
					}
				}
				currentNews = "Market makers are currently delta hedging the market!";
				break;
			case 7: // Short squeeze
				// Line of red blocks on the sides only
				for (int r = 0; r < blocksArray2D.GetLength(0); r++) {
					for (int c = 0; c < blocksArray2D.GetLength(1); c++) {
						if (blocksArray2D[r, c] != null) {
							Destroy(blocksArray2D[r, c]);
							if (r == 0 || r == blocksArray2D.GetLength(0) - 1) {
								blocksArray2D[r, c] = StoreBlock(r, c, BlockType.Loss);
							} else {
								blocksArray2D[r, c] = StoreBlock(r, c, BlockType.Profit);
							}
						}
					}
				}
				currentNews = "The retail investors have banded together and started a short squeeze in the market!";
				break;
			case 8: // Stock market frenzy
				// Destroy all the blocks and replace with profit blocks
				for (int r = 0; r < blocksArray2D.GetLength(0); r++) {
					for (int c = 0; c < blocksArray2D.GetLength(1); c++) {
						if (blocksArray2D[r, c] != null) {
							Destroy(blocksArray2D[r, c]);
							blocksArray2D[r, c] = StoreBlock(r, c, BlockType.Profit);
						}
					}
				}
				currentNews = "The stock market frenzy has begun! All investors are piling in to buy buy buy!";
				break;
			case 9: // Gold fever
					// Replace all goldbars with coins
				for (int r = 0; r < symbolsArray2D.GetLength(0); r++) {
					for (int c = 0; c < symbolsArray2D.GetLength(1); c++) {
						if (symbolsArray2D[r, c] != null && symbolsArray2D[r, c].tag != "goldbar") {
							Destroy(symbolsArray2D[r, c]);
							symbolsArray2D[r, c] = StoreSymbol(r, c, SymbolType.Goldbar);
						}
					}
				}
				currentNews = "The market is now experiencing a gold fever!";
				break;
			default: // Default no event, reset everything back to normal
				SetBlockChance(currentLevel);
				currentNews = "The stock market is trading normally. Good luck!";
				break;
        }

		// Set news headline
		newsText.text = currentNews;
	}

	// Use this for initialization
	void Start () {
		GetLevel();

		// Load the background of play area
		Instantiate(playAreaBackground,
			new Vector2(width / 2.0F + offsetX + 0.25F, height / 2.0F + offsetY),
			Quaternion.identity);

		// Hook up the text
		profitText = GameObject.FindWithTag("profitText").GetComponent<Text>();
		levelText = GameObject.FindWithTag("levelText").GetComponent<Text>();
		movesLeftText = GameObject.FindWithTag("movesLeftText").GetComponent<Text>();
		goalText = GameObject.FindWithTag("goalText").GetComponent<Text>();
		roiText = GameObject.FindWithTag("roiText").GetComponent<Text>();
		newsText = GameObject.FindWithTag("newsText").GetComponent<Text>();

		// Load the level
		LevelLoad();
	}
	
	// Update is called once per frame
	void Update () {
		// Keep looping until no more matches on the board
		if (MatchFound(symbolsArray2D, blocksArray2D) && matchEnabled) {
			StartCoroutine(UpdateBoards());
		}

		// Get the current and previous selected blocks
		if (Input.GetMouseButtonUp(0) && screenEnabled && !isPaused) {
			// Get and animate the object that was clicked
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit.collider != null) {
				if (currentBlock != null) {
					currentBlock.GetComponent<Animator>().SetBool("isMatched", false);
					previousBlock = currentBlock;
                }

				currentBlock = hit.collider.gameObject;
				currentBlock.GetComponent<Animator>().SetBool("isMatched", true);

                // Check for a match if the current and previous block are swappable
                if (previousBlock != null) {
					// Stop existing animations
					if(previousBlock.GetComponent<Animator>() != null) {
						previousBlock.GetComponent<Animator>().SetBool("isMatched", false);
					}
					if (currentBlock.GetComponent<Animator>() != null) {
						currentBlock.GetComponent<Animator>().SetBool("isMatched", false);
					}

					SwapBlocks(previousBlock, currentBlock);
					previousBlock = null;
					currentBlock = null;
                }
			}
		}

		// Display game over or next level panel depending on the
		// moves left and whether goal was reached or not
		if(currentMovesLeft == 0) {
			if(currentScore < profitGoal) {
				GameOver();
			} else {
				NextLevelPanel();
            }
        } else if(currentScore >= profitGoal) {
			NextLevelPanel();
        }

		if (IsBuggedOut()) {
			SpeedHackPanel();
		}
	}
}
