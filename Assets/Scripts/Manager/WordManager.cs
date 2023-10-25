using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UniRx;

public class WordManager : MonoBehaviour
{
    // public List<string> wordList = new List<string>();

    [SerializeField] public TMP_Text wordText;
    [SerializeField] private InputAction wordReset;
    [SerializeField] private InputAction gameExit;

    private void OnEnable()
    {
        wordReset.Enable();
        wordReset.started += _ => { WordReset(); };
        gameExit.Enable();
        gameExit.started += _ => { GameQuit(); };
    }

    private void OnDisable()
    {
        wordReset.Disable();
        wordReset.started -= _ => { WordReset(); };
        gameExit.started -= _ => { GameQuit(); };
    }

    public void WordReset()
    {
        // wordList.Clear();
        wordText.text = "";
    }

    private void GameQuit()
    {
        Application.Quit();
    }
}
