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

    // 켜진 장소 및 장소의 액션 버튼 목록
    [HideInInspector] public List<IDBtn> enablePlaceBtnList = new();
    [HideInInspector] public List<IDBtn> enableBehaviorActionBtnList = new();

    // 선택한 장소 및 장소의 액션 목록
    [SerializeField] public List<string> currentPlaceIDList = new();
    [SerializeField] public List<ButtonValue> currentPlaceList = new();
    [HideInInspector] private List<string> currentBehaviorActionIDList = new();
    [HideInInspector] public List<ButtonValue> currentBehaviorActionList = new();

    // 선택한 장소 및 장소의 액션
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
                    // 행동 매니저 만들기
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
            // 20231127 여기 쓰고있었음
            /*
            if(currentPlaceIDList.Find(btn.buttonValue.ID))
            btn.buttonValue.ID
            DataManager.PlaceDatas[1][btn.buttonValue.ID]
            */
        }

        currentPlaceList.Clear(); // 초기화
        foreach (string id in currentPlaceIDList) // ID 순회
        {
            ButtonValue place = new(id, (string)DataManager.PlaceDatas[1][id]);
            currentPlaceList.Add(place);
        }
    }
    private void InitBehaviorActionID(string id)
    {
        currentBehaviorActionIDList.Clear(); // 초기화
        foreach (var data in DataManager.StreamEventDatas[0]) // 스트림 이벤트 순회
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
        currentBehaviorActionList.Clear(); // 초기화
        foreach (string id in currentBehaviorActionIDList) // ID 순회
        {
            ButtonValue word = new(id, (string)DataManager.WordActionDatas[0][id]);
            currentBehaviorActionList.Add(word);
        }
    }
    #endregion
}
