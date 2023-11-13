using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public abstract class WordBtnSpawner : MonoBehaviour
{
    [Header("*Property")]
    [SerializeField] protected ObjectPooling ObjectPooling;
    [SerializeField] protected WordManager WordManager;

    [Header("*WordParentObj")]
    [SerializeField] protected RectTransform wordParentObject;
 
    private void OnEnable()
    {
        SpawnWordBtn();
    }

    public virtual void SpawnWordBtn()
    {
    }

    protected virtual WordBtn CreateWordBtn(string btnText)
    {
        return null;
    }

    public void PickWord()
    {
        if (WordManager.enableWordBtnList.Count != 0)
        {
            for (int i = WordManager.enableWordBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.ObjectPick(WordManager.enableWordBtnList[i]);
            }
            WordManager.enableWordBtnList.Clear();
        }
    }

    public virtual void PickWordAction()
    {
    }

    private void OnDisable()
    {
        PickWord();
        PickWordAction();
    }
}
