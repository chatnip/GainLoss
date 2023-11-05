using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UniRx;

public class WordManager : Manager<WordManager>
{
    [SerializeField] ObjectPooling ObjectPooling;
    public List<WordBase> currentWordList = new List<WordBase>();

    private void Start()
    {
        
    }
}
