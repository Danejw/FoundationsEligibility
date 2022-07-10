using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public enum Piece
    {
        none,
        cross,
        circle
    }

    public enum Difficulty
    {
        easy,
        hard
    }
    public Difficulty difficulty = Difficulty.easy;

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
    [SerializeField] private AudioClip drawSound;
    [SerializeField] private AudioClip startSound;


    [SerializeField] private bool isPlayersTurn;
    private bool aiThnking = false;
    
    [Header("Picks")]
    public List<GameObject> allPossiblePicks;
    [SerializeField] List<GameObject> availablePicks;
    [SerializeField] List<GameObject> playerPicks = new List<GameObject>();
    [SerializeField] List<GameObject> aiPicks = new List<GameObject>();

    private List<GameObject> instantiatedPieces = new List<GameObject>();


    public delegate void GameEnded(Piece winner);
    public static event GameEnded onGameEnded;

    public delegate void GamePaused();
    public static event GamePaused onGamePaused;

    public delegate void GameStarted();
    public static event GameStarted onGameStarted;

    [Serializable]
    public class WinningSequence
    {
        public bool isSequenceOpen = true;
        public int playerConsecutive = 0;
        public int aiConsecutive = 0;
        public List<int> sequence;
    }
    [SerializeField] private WinningSequence[] winningSequences;

    [SerializeField] private bool gamePaused;

    private void Awake()
    {
        playAudio = GetComponent<AudioSource>();
    }


    void Start()
    {
        TicTacToeTrigger.onTriggerCLicked += playerClickedTrigger;

        onGamePaused?.Invoke();
        gamePaused = true;
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
       
        if (availablePicks.Count <= 0 && !gamePaused)
            StartCoroutine(EndGameSequence(Piece.none));
    }

    public void InitGame()
    {
        // set available pieces
        availablePicks.Clear();
        foreach (var piece in allPossiblePicks)
        {
            availablePicks.Add(piece);
            piece.GetComponent<TicTacToeTrigger>().canClick = true;
        }

        // reset sequences
        foreach (var seq in winningSequences)
        {
            seq.playerConsecutive = 0;
            seq.aiConsecutive = 0;
            seq.isSequenceOpen = true;
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

        gamePaused = false;

        onGameStarted?.Invoke();

        // play sound
        playAudio.clip = startSound;
        playAudio.Play();
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
        foreach (var seq in winningSequences)
        {
            if (seq.sequence.Contains(pieceObj.GetComponent<TicTacToeTrigger>().index))
            {
                seq.playerConsecutive++;
                if (seq.playerConsecutive >= 3)
                    StartCoroutine(EndGameSequence(playerPiece));
                if (seq.playerConsecutive + seq.aiConsecutive >= 3)
                    seq.isSequenceOpen = false;
            }
        }
    }

    private IEnumerator AiPick()
    {
        if (gamePaused) yield break;
        if (availablePicks.Count == 0) yield break;

        aiThnking = true;

        
        // Ai pick
        GameObject objPick = UpdateBestNextMoves();
        if (difficulty == Difficulty.hard)
        {
            if (!objPick)
            {
                // random
                int pick = UnityEngine.Random.Range(0, availablePicks.Count - 1);
                objPick = availablePicks[pick];
            }
        }
        // easy : Random
        else
        {
            int pick = UnityEngine.Random.Range(0, availablePicks.Count - 1);
            objPick = availablePicks[pick];
        }
        

        Debug.Log("Pick : " + objPick);

        objPick.GetComponent<TicTacToeTrigger>().canClick = false;

        yield return new WaitForSeconds(1);

        // place the right ai piece
        switch (aiPiece)
        {
            case Piece.cross:
                SetVisual(xPiece, objPick.transform.position);
                break;
            case Piece.circle:
                SetVisual(oPiece, objPick.transform.position);
                break;
        }

        // add pick to player's list of picks
        aiPicks.Add(objPick);

        // iterate to ai's turn
        isPlayersTurn = true;
        aiThnking = false;

        // play sound
        playAudio.clip = aiPlaySound;
        playAudio.Play();

        // play particle

        // Check if the ai has won
        foreach (var seq in winningSequences)
        {
            if (seq.sequence.Contains(objPick.GetComponent<TicTacToeTrigger>().index))
            {
                seq.aiConsecutive++;
                if (seq.aiConsecutive >= 3)
                    StartCoroutine(EndGameSequence(aiPiece));
                if (seq.playerConsecutive + seq.aiConsecutive >= 3)
                    seq.isSequenceOpen = false;
            }
        }

        // remove pick from availble picks
        availablePicks.Remove(objPick);
    }

    private void SetVisual(GameObject piece, Vector3 position)
    {
        instantiatedPieces.Add(Instantiate(piece, position, Quaternion.identity));
    }

    private IEnumerator EndGameSequence(Piece winner)
    {
        gamePaused = true;

        onGameEnded?.Invoke(winner);

        // play sound
        if (winner == playerPiece)
        {
            playAudio.clip = winningSound;
            playAudio.Play();
        }
        else if (winner == aiPiece)
        {
            playAudio.clip = losingSound;
            playAudio.Play();
        }
        else if (winner == Piece.none)
        {
            playAudio.clip = drawSound;
            playAudio.Play();
        }


        yield return new WaitForSeconds(5);

        foreach (var piece in instantiatedPieces)
            Destroy(piece.gameObject);
    }

    public void SetDifficultyHard(bool value)
    {
        if (value)
            difficulty = Difficulty.hard;

        if (!value)
            difficulty = Difficulty.easy;
    }


    [SerializeField] private List<GameObject> bestNextMoves = new List<GameObject>();
    private GameObject UpdateBestNextMoves()
    {
        bestNextMoves.Clear();

        // iterate through the sequences and order the indexs according to consecutive counts
        foreach(var seq in winningSequences)
        {
            if (seq.isSequenceOpen)
            {               
                if (seq.aiConsecutive == 2)
                {
                    foreach (var index in seq.sequence)
                    {
                        // put on top of list
                        foreach (var obj in availablePicks)
                        {
                            if (obj.GetComponent<TicTacToeTrigger>().index == index)
                            {
                                bestNextMoves.Insert(0, obj);
                            }
                        }
                    }
                }
                if (seq.playerConsecutive == 2)
                {
                    foreach (var index in seq.sequence)
                    {
                        // put on top of list
                        foreach (var obj in availablePicks)
                        {
                            if (obj.GetComponent<TicTacToeTrigger>().index == index)
                            {
                                bestNextMoves.Insert(0, obj);
                            }
                        }
                    }
                }
            }
        }

        if (bestNextMoves.Count > 0)
        {
            Debug.Log("Best Next Move : ", bestNextMoves[0]);
            return bestNextMoves[0];
        }
        else
        {
            return null;
        }
    }
}
