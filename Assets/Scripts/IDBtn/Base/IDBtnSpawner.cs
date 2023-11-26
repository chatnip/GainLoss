using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public abstract class IDBtnSpawner : MonoBehaviour
{
    [Header("*Property")]
    [SerializeField] protected ObjectPooling ObjectPooling;
 
    private void OnEnable()
    {
        SpawnIDBtn();
    }

    protected virtual void SpawnIDBtn()
    {

    }

    protected virtual IDBtn CreateIDBtn(ButtonValue buttonValue)
    {
        return null;
    }

    protected virtual void PickIDBtn()
    {

    }

    protected virtual void OnDisable()
    {
        PickIDBtn();
    }
}
