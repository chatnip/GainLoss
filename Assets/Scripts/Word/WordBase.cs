using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Word", menuName = "Prototype/Create New Word")]

public class WordBase : ScriptableObject
{
    [SerializeField] string wordName;
    [SerializeField] string wordPlace;
    [SerializeField] bool disposable;

    public string WordName
    {
        get { return wordName; }
    }
    public string WordPlace
    {
        get { return wordPlace ; }
    }
    public bool Disposable
    {
        get { return disposable; }
    }

}
