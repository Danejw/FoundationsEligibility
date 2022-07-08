using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
	
}

public class TicTacToeAI : MonoBehaviour
{

	int _aiLevel;

	TicTacToeState[,] boardState;

	[SerializeField]
	private bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;
	
	TicTacToeState playerState = TicTacToeState.cross;
	TicTacToeState aiState = TicTacToeState.circle;

	[SerializeField]
	private GameObject _xPrefab;

	[SerializeField]
	private GameObject _oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _availableTriggers;
	ClickTrigger[,] _playerTriggers;
	ClickTrigger[,] _aiTriggers;
	
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
	}

    private void Update()
    {
        if (!_isPlayerTurn)
        {
			StartCoroutine(AiPick());
        }
    }

    public void StartAI(int AILevel){
		_aiLevel = AILevel;
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_availableTriggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame()
	{
		_availableTriggers = new ClickTrigger[3,3];
		onGameStarted.Invoke();
	}

	public void PlayerSelects(int coordX, int coordY)
	{
		//if (!_triggers[coordX, coordY].canClick) return;
		if (!_isPlayerTurn) return;

		SetVisual(coordX, coordY, playerState);

		_availableTriggers[coordX, coordY].canClick = false;

		_isPlayerTurn = false;
	}

	// Im gonna call this method when the ai finds out what coords to select amongst the available coords
	public void AiSelects(int coordX, int coordY)
	{
		if (!_availableTriggers[coordX, coordY].canClick) return;
		if (_isPlayerTurn) return;

		SetVisual(coordX, coordY, aiState);

		_availableTriggers[coordX, coordY].canClick = false;

		_isPlayerTurn = true;
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_availableTriggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}


	private IEnumerator AiPick()
    {
		// if all triggers are taken, break
		foreach (var trigger in _availableTriggers)
        {
			if (trigger.canClick)
            {
				continue;
            }
            else
            {
				break;
            }
        }

		yield return new WaitForSeconds(1);

		// place a random piece in available spot
        
		int x = UnityEngine.Random.Range(0,3);
		int y = UnityEngine.Random.Range(0,3);

		while (_availableTriggers[x, y].canClick == false)
        {
			x = UnityEngine.Random.Range(0, 3);
			y = UnityEngine.Random.Range(0, 3);
		}

		AiSelects(x, y);


		// if player has two in a row, block it

		// if  ai has two in a row, pick the third

		// place a piece next to another
	}
}
