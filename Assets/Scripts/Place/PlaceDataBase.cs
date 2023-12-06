using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "PlaceData", menuName = "GainLoss/PlaceData")]
[System.Serializable]
public class PlaceDataBase : ScriptableObject
{
    public string placeID;
    public GameObject place;
    public Vector3 spawnPos;
    public List<PlaceObject> placeObjectList = new();
}
