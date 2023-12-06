using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [SerializeField] PhoneHardware PhoneHardware;

    private void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        PhoneHardware.PhoneOn();
    }
}
