using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
     [SerializeField] private GameObject panel;

    public GameObject crossText;
    public GameObject circleText;
    public GameObject drawText;


    private void Start()
    {
        GameManager.onGameStarted += GameStarted;
        GameManager.onGamePaused += GamePaused;
        GameManager.onGameEnded += GameEnded;

        if (drawText) drawText.SetActive(false);
        if (circleText) circleText.SetActive(false);
        if (crossText) crossText.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.onGameStarted -= GameStarted;
        GameManager.onGamePaused -= GamePaused;
        GameManager.onGameEnded -= GameEnded;
    }

    private void GamePaused()
    {
        if (panel) panel.SetActive(true);
    }

    private void GameStarted()
    {
        if (panel) panel.SetActive(false);
    }

    private void GameEnded(GameManager.Piece winner)
    {
        StartCoroutine(EndGameRoutine(winner));
    }

    private IEnumerator EndGameRoutine(GameManager.Piece winner)
    {
        switch (winner)
        {
            case GameManager.Piece.none:
                if (drawText) drawText.SetActive(true);
                break;
            case GameManager.Piece.circle:
                if (circleText) circleText.SetActive(true);
                break;
            case GameManager.Piece.cross:
                if (crossText) crossText.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(5);

        if (drawText) drawText.SetActive(false);
        if (circleText) circleText.SetActive(false);
        if (crossText) crossText.SetActive(false);

        if (panel) panel.SetActive(true);
    }
}
