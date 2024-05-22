using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class EditorTestManager : Singleton<EditorTestManager>
{
    [Header("*Input")]
    [SerializeField] InputAction TestInput;

    [Header("*Test")]
    [SerializeField] WordManager WordManager;

#if UNITY_EDITOR
    protected override void Awake()
    {
        
    }

    private void OnEnable()
    {
        TestInput.Enable();
        TestInput.started += _ =>
        {
            Test_Function();
        };
    }

    private void OnDisable()
    {
        TestInput.Disable();
        TestInput.started -= _ =>
        {
            Test_Function();
        };
    }

    private void Test_Function()
    {
        foreach (string s in WordManager.currentWordIDList)
        {
            Debug.Log(DataManager.WordDatas[0][s]);
        }

    }

#endif
}
