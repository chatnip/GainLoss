using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILpadSpawner : IDBtnSpawner
{

    [Header("*WordParentObj")]
    [SerializeField] RectTransform wordParentObject;


   
    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = false;
        wordBtn.buttonValue = word;       
        return wordBtn;
    }
}
