using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceManager : MonoBehaviour
{
    [HideInInspector] public List<string> currentPlaceIDList = new();






    #region Init
    public void InitWord()
    {
        currentPlaceIDList.Clear(); // �ʱ�ȭ
        foreach (string id in currentPlaceIDList) // ID ��ȸ
        {
            // ButtonValue word = new(id, (string)DataManager.WordDatas[0][id]);
            // currentPlaceIDList.Add(word);
        }
    }
    #endregion
}
