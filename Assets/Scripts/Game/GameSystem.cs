using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [SerializeField] GameObject phone;

    private void Start()
    {
        GameStart();
    }

    private void GameStart()
    {
        phone.SetActive(true);
    }
}
