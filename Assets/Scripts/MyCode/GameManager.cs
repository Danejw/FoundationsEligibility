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
    public List<GameObject> allPicks;
    [SerializeField] List<GameObject> availablePicks;
    [SerializeField] List<GameObject> playerPicks = new List<GameObject>();
    [SerializeField] List<GameObject> aiPicks = new List<GameObject>();

    private List<int>[] winningPicks = new List<int>[8];

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
    }

    private void InitGame()
    {
        SetWinningPicks();

        // sets pieces
        if (playerPiece == Piece.circle) aiPiece = Piece.cross;
        else
            aiPiece = Piece.circle;

        // randomizes who plays first move
        isPlayersTurn = UnityEngine.Random.value > 0.5f;

        // set available pieces
        availablePicks = allPicks;

        // clear past lists
        playerPicks.Clear();
        aiPicks.Clear();
    }

    private void SetWinningPicks()
    {
        // set winning picks
        winningPicks[0] = new List<int>();
        winningPicks[0].Add(0);
        winningPicks[0].Add(1);
        winningPicks[0].Add(2);

        winningPicks[1] = new List<int>();
        winningPicks[1].Add(3);
        winningPicks[1].Add(4);
        winningPicks[1].Add(5);

        winningPicks[2] = new List<int>();
        winningPicks[2].Add(6);
        winningPicks[2].Add(7);
        winningPicks[2].Add(8);

        winningPicks[3] = new List<int>();
        winningPicks[3].Add(0);
        winningPicks[3].Add(3);
        winningPicks[3].Add(6);

        winningPicks[4] = new List<int>();
        winningPicks[4].Add(1);
        winningPicks[4].Add(4);
        winningPicks[4].Add(7);

        winningPicks[5] = new List<int>();
        winningPicks[5].Add(2);
        winningPicks[5].Add(5);
        winningPicks[5].Add(8);

        winningPicks[6] = new List<int>();
        winningPicks[6].Add(0);
        winningPicks[6].Add(4);
        winningPicks[6].Add(8);

        winningPicks[7] = new List<int>();
        winningPicks[7].Add(2);
        winningPicks[7].Add(4);
        winningPicks[7].Add(6);
    }

    private void playerClickedTrigger(GameObject pieceObj)
    {
        if (availablePicks.Count == 0) return;
        if (!availablePicks.Contains(pieceObj)) return;

        // check to see if its the player's turn
        if (!isPlayersTurn) return;

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
    }


    private IEnumerator AiPick()
    {
        if (availablePicks.Count == 0) yield break;

        aiThnking = true;

        // pick
        var pick = UnityEngine.Random.Range(0, availablePicks.Count - 1);

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

        // iterate to other player's turn
        isPlayersTurn = true;
        aiThnking = false;

        // play sound
        playAudio.clip = aiPlaySound;
        playAudio.Play();

        // play particle
    }

    private void SetVisual(GameObject piece, Vector3 position)
    {
        Instantiate(piece, position, Quaternion.identity);
    }



}
