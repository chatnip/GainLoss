//Refactoring v1.0
using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region Value

    public static GameManager Instance;

    [Header("=== Other")]
    [SerializeField] public string currentChapter = "1";

    #endregion


    #region Framework & Base Set


    private void Awake()
    {
        if (null == Instance)
        {
            Debug.Log("�ν��Ͻ�ȭ");
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.Log("�����ϴ� ����");
            Destroy(this.gameObject);
        }
    }
    
    #endregion


}


