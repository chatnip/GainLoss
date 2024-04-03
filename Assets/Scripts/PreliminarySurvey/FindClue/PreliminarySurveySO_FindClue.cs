using UnityEngine;

[CreateAssetMenu(fileName = "PSSO_FC", menuName = "Scriptable Object/PSSO/Find Clue", order = int.MaxValue)]
[System.Serializable]
public class PreliminarySurveySO_FindClue : PreliminarySurveySO
{
    [SerializeField] public string answerNum;
    [SerializeField] public GameObject[] clues = new GameObject[8];
}
