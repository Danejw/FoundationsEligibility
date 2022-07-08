using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public enum Piece
    {
        cross,
        circle
    }

    [Header("Pieces")]
    public Piece playerPiece;
    [SerializeField] private Piece aiPiece;
    [SerializeField] private GameObject xPiece;
    [SerializeField] private GameObject oPiece;


    // sound fx
    AudioSource playAudio;

    [Header("Sound FX")]
    [SerializeField] private AudioClip playerPlaySound;
    [SerializeField] private AudioClip aiPlaySound;
    [SerializeField] private AudioClip winningSound;
    [SerializeField] private AudioClip losingSound;


    [SerializeField] private bool isPlayersTurn;
    private bool aiThnking = false;
    
    [Header("Picks")]
    public List<GameObject> allPossiblePicks;
    [SerializeField] List<GameObject> availablePicks;
    [SerializeField] List<GameObject> playerPicks = new List<GameObject>();
    [SerializeField] List<GameObject> aiPicks = new List<GameObject>();

    

    private List<GameObject> instantiatedPieces = new List<GameObject>();


    [SerializeField] private List<List<int>> winningPicks;
    public class WinningSequence
    {
        public int consecutive;
        public List<int> sequence = new List<int>();
    }



    private void Awake()
    {
        playAudio = GetComponent<AudioSource>();
    }


    void Start()
    {
        TicTacToeTrigger.onTriggerCLicked += playerClickedTrigger;

        InitGame();
    }

    private void OnDestroy()
    {
       TicTacToeTrigger.onTriggerCLicked -= playerClickedTrigger;
    }


    private void Update()
    {
        if (!isPlayersTurn && !aiThnking)
        {
            // do ai stuff
            StartCoroutine(AiPick());
        }

        /*
        // check winning numbers
        foreach (var pick in playerPicks)
        {
            foreach (winning)
            if ( pick.GetComponent<TicTacToeTrigger>().index)
        }
        */
       
        if (availablePicks.Count <= 0)
            EndGame();
    }



    private void InitGame()
    {
        SetWinningPicks();

        // set available pieces
        foreach (var piece in allPossiblePicks)
        {
            availablePicks.Add(piece);
            piece.GetComponent<TicTacToeTrigger>().canClick = true;
        }

        // sets pieces
        if (playerPiece == Piece.circle) aiPiece = Piece.cross;
        else
            aiPiece = Piece.circle;

        // randomizes who plays first move
        isPlayersTurn = UnityEngine.Random.value > 0.5f;

        // clear past lists
        playerPicks.Clear();
        aiPicks.Clear();
    }

    private void SetWinningPicks()
    {
        // set winning picks
        winningPicks = new List<List<int>>();

        List<int> seq0 = new List<int> { 0, 1, 2 };
        winningPicks.Add(seq0);

        List<int> seq1 = new List<int> { 3, 4, 5 };
        winningPicks.Add(seq0);

        List<int> seq2 = new List<int> { 6, 7, 8 };
        winningPicks.Add(seq0);

        List<int> seq3 = new List<int> { 0, 3, 6 };
        winningPicks.Add(seq0);

        List<int> seq4 = new List<int> { 1, 4, 7 };
        winningPicks.Add(seq0);

        List<int> seq5 = new List<int> { 2, 5, 8 };
        winningPicks.Add(seq0);

        List<int> seq6 = new List<int> { 0, 4, 8 };
        winningPicks.Add(seq0);

        List<int> seq7 = new List<int> { 6, 4, 2 };
        winningPicks.Add(seq0);
    }

    private void playerClickedTrigger(GameObject pieceObj)
    {
        if (availablePicks.Count == 0) return;
        if (!availablePicks.Contains(pieceObj)) return;

        // check to see if its the player's turn
        if (!isPlayersTurn) return;

        pieceObj.GetComponent<TicTacToeTrigger>().canClick = false;

        // place the right player piece
        switch (playerPiece)
        {
            case Piece.cross:
                SetVisual(xPiece, pieceObj.transform.position);
                break;
            case Piece.circle:
                SetVisual(oPiece, pieceObj.transform.position);
                break;
        }

        // add pick to player's list of picks
        playerPicks.Add(pieceObj);

        // remove pick from availble picks
        availablePicks.Remove(pieceObj);

        // iterate to other player's turn
        isPlayersTurn = false;

        // play sound
        playAudio.clip = playerPlaySound;
        playAudio.Play();

        // play particle

        // Check if the player won
        //CheckForWinner();

        
    }


    private IEnumerator AiPick()
    {
        if (availablePicks.Count == 0) yield break;

        aiThnking = true;

        // pick
        var pick = UnityEngine.Random.Range(0, availablePicks.Count - 1);

        availablePicks[pick].GetComponent<TicTacToeTrigger>().canClick = false;

        yield return new WaitForSeconds(1);


        // place the right ai piece
        switch (aiPiece)
        {
            case Piece.cross:
                SetVisual(xPiece, availablePicks[pick].transform.position);
                break;
            case Piece.circle:
                SetVisual(oPiece, availablePicks[pick].transform.position);
                break;
        }

        // add pick to player's list of picks
        aiPicks.Add(availablePicks[pick]);

        // remove pick from availble picks
        availablePicks.Remove(availablePicks[pick]);

        // iterate to ai's turn
        isPlayersTurn = true;
        aiThnking = false;

        // play sound
        playAudio.clip = aiPlaySound;
        playAudio.Play();

        // play particle

        // Check if the ai has won
        //CheckForWinner();
    }

    private void SetVisual(GameObject piece, Vector3 position)
    {
        instantiatedPieces.Add(Instantiate(piece, position, Quaternion.identity));
    }


    [SerializeField] private int consecutive = 0;
    private void CheckForWinner()
    {
       
        // Go through each set of winngin picks
        foreach (List<int> list in winningPicks)
        {        
            // Go through each set of player picks
            foreach (GameObject pick in playerPicks)
            {            
                // Check to see if each player pick is in the winning pick list
                if (list.Contains(pick.GetComponent<TicTacToeTrigger>().index))
                {
                    consecutive++;
                    Debug.Log("Found Pick " + pick);
                }
                else
                {
                    consecutive = 0;
                }
            }
        }
    }

    private void EndGame()
    {
        foreach (var piece in instantiatedPieces)
            Destroy(piece.gameObject);

        InitGame();
    }

}
