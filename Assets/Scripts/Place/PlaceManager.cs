using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class PlaceManager : MonoBehaviour
{
    [Header("*Property")]
    [SerializeField] ObjectPooling ObjectPooling;

    [Space(10)]
    [SerializeField] PlaceSpawner placeBtnSpawner;

    [Header("*Place")]
    [SerializeField] List<IDBtn> placeBtnList = new();

    // ���� ��� �� ����� �׼� ��ư ���
    [HideInInspector] public List<IDBtn> enablePlaceBtnList = new();
    [HideInInspector] public List<IDBtn> enableBehaviorActionBtnList = new();

    // ������ ��� �� ����� �׼� ���
    [SerializeField] public List<string> currentPlaceIDList = new();
    [SerializeField] public List<ButtonValue> currentPlaceList = new();
    [HideInInspector] private List<string> currentBehaviorActionIDList = new();
    [HideInInspector] public List<ButtonValue> currentBehaviorActionList = new();

    // ������ ��� �� ����� �׼�
    private ButtonValue currentPlace;
    private ButtonValue currentBehaviorAction;

    private void Start()
    {
        InitPlace();
    }

    #region ButtonListSeting
    public void PlaceBtnListSet()
    {
        foreach (IDBtn placeBtn in enablePlaceBtnList)
        {
            placeBtn.button
                .OnClickAsObservable()
                .Select(place => placeBtn.buttonValue)
                .Subscribe(place =>
                {
                    currentPlace = place;
                    InitBehaviorActionID(currentPlace.ID);
                    // placeBtnSpawner.SpawnWordActionBtn();
                });
        }
    }

    public void BehaviorActionBtnListSet()
    {
        foreach (IDBtn behaviorActionBtn in enableBehaviorActionBtnList)
        {
            behaviorActionBtn.button
                .OnClickAsObservable()
                .Select(action => behaviorActionBtn.buttonValue)
                .Subscribe(action =>
                {
                    currentBehaviorAction = action;
                    // �ൿ �Ŵ��� �����
                    // StreamManager.currentStreamEventID = currentWord.ID + currentWordAction.ID;
                });
        }
    }
    #endregion

    #region Init
    public void InitPlace()
    {
        foreach(IDBtn btn in placeBtnList)
        {
            // 20231127 ���� �����־���
            /*
            if(currentPlaceIDList.Find(btn.buttonValue.ID))
            btn.buttonValue.ID
            DataManager.PlaceDatas[1][btn.buttonValue.ID]
            */
        }

        currentPlaceList.Clear(); // �ʱ�ȭ
        foreach (string id in currentPlaceIDList) // ID ��ȸ
        {
            ButtonValue place = new(id, (string)DataManager.PlaceDatas[1][id]);
            currentPlaceList.Add(place);
        }
    }
    private void InitBehaviorActionID(string id)
    {
        currentBehaviorActionIDList.Clear(); // �ʱ�ȭ
        foreach (var data in DataManager.StreamEventDatas[0]) // ��Ʈ�� �̺�Ʈ ��ȸ
        {
            if (data.Key.Contains(id))
            {
                string key = Regex.Replace(data.Key.ToString(), id, "");
                currentBehaviorActionIDList.Add(key);
            }
        }
        InitBehaviorAction();
    }

    private void InitBehaviorAction()
    {
        currentBehaviorActionList.Clear(); // �ʱ�ȭ
        foreach (string id in currentBehaviorActionIDList) // ID ��ȸ
        {
            ButtonValue word = new(id, (string)DataManager.WordActionDatas[0][id]);
            currentBehaviorActionList.Add(word);
        }
    }
    #endregion
}
