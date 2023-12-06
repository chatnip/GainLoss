using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class PlaceManager : Manager<PlaceManager>
{
    [Header("*Property")]
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] PhoneSoftware PhoneSoftware;

    [Space(10)]
    [SerializeField] PlaceSpawner placeBtnSpawner;

    [Header("*Place")]
    [SerializeField] Button homeBtn;
    [SerializeField] List<IDBtn> placeBtnList = new();

    [Header("*Behavior")]
    [SerializeField] GameObject behaviorPopup;
    [SerializeField] Button behaviorBackground;
    [SerializeField] TMP_Text behaviorHeaderText;

    [Header("*BehaviorConfirm")]
    [SerializeField] GameObject behaviorConfirmPopup;
    [SerializeField] TMP_Text behaviorConfirmText;
    [SerializeField] Button behaviorConfirmButton;
    [SerializeField] Button behaviorCancelButton;
    

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

    protected override void Awake()
    {
        base.Awake();

        homeBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                // 맵 버튼 비활성화
                PhoneSoftware.mapBtn.interactable = false;
                // 핸드폰 끄기
                PhoneHardware.PhoneOff();
            });


        behaviorBackground.OnClickAsObservable()
            .Subscribe(btn =>
            {
                behaviorPopup.SetActive(false);
            });

        behaviorCancelButton.OnClickAsObservable()
            .Subscribe(btn =>
            {
                behaviorConfirmPopup.SetActive(false);
            });

        behaviorConfirmButton.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ActionEventManager.currentActionEventID = currentPlace.ID + currentBehaviorAction.ID;
                ActionEventManager.StartCoroutine(ActionEventManager.PlaceSetting());
                PhoneHardware.PhoneOff();
            });
    }

    private void Start()
    {
        InitPlace();
        // 나중에 켜질때마다 활성화
    }

    #region ButtonListSeting
    public void PlaceBtnListSet()
    {
        foreach (IDBtn placeBtn in placeBtnList)
        {
            placeBtn.button
                .OnClickAsObservable()
                .Select(place => placeBtn.buttonValue)
                .Subscribe(place =>
                {
                    currentPlace = place;
                    string text = string.Format("<#D40047><b>{0}</b></color> 에서 무엇을 할까?", currentPlace.Name);
                    behaviorHeaderText.text = text;
                    behaviorPopup.SetActive(true);
                    InitBehaviorActionID(currentPlace.ID);
                    placeBtnSpawner.SpawnBehaviorActionBtn();
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
                    string text = string.Format("<#D40047><b>{0}</b></color> 에서 <#D40047><b>{1}</b></color>", currentPlace.Name, currentBehaviorAction.Name);
                    behaviorConfirmText.text = text;
                    behaviorConfirmPopup.SetActive(true);
                });
        }
    }
    #endregion

    #region Init
    public void InitPlace()
    {
        currentPlaceList.Clear(); // 초기화
        foreach (IDBtn btn in placeBtnList)
        {
            btn.button.interactable = false;
        }

        foreach (string id in currentPlaceIDList) // ID 순회
        {
            IDBtn btn = placeBtnList.Find(x => x.buttonValue.ID == id);
            btn.button.interactable = true;
            currentPlaceList.Add(btn.buttonValue);
        }
        PlaceBtnListSet();
    }

    private void InitBehaviorActionID(string id)
    {
        currentBehaviorActionIDList.Clear(); // 초기화
        foreach (var data in DataManager.ActionEventDatas[0]) // 액션 이벤트 순회
        {
            if (data.Key.Contains(id))
            {
                string key = data.Key.ToString().Substring(3, 4);
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
            ButtonValue word = new(id, (string)DataManager.BehaviorActionDatas[1][id]);
            currentBehaviorActionList.Add(word);
        }
    }
    #endregion
}
