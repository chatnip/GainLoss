using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;

public class PlaceManager : Manager<PlaceManager>
{
    [Header("*Property")]
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] PhoneSoftware PhoneSoftware;
    [SerializeField] CheckGetAllDatas CheckGetAllDatas;

    [Header("*Player")]
    [SerializeField] SetInteractionObjects SetInteractionObjects;

    [Header("*Going Somewhere loading")]
    [SerializeField] CanvasGroup GoingSomewhereloadingCG;
    [SerializeField] TMP_Text CurrentPlaceTxt;

    /*[Space(10)]
    [SerializeField] PlacePadSpawner placeBtnSpawner;*/

    [Header("*Place")]
    [SerializeField] Button homeBtn;
    [SerializeField] List<IDBtn> placeBtnList = new();
    [Tooltip("Must make sure to get the order right FOR CSV")]
    [SerializeField] List<GameObject> placeGOList = new();

    /*[Header("*Behavior")]
    [SerializeField] GameObject behaviorPopup;
    [SerializeField] Button behaviorBackground;
    [SerializeField] TMP_Text behaviorHeaderText;

    [Header("*BehaviorConfirm")]
    [SerializeField] GameObject behaviorConfirmPopup;
    [SerializeField] TMP_Text behaviorConfirmText;
    [SerializeField] Button behaviorConfirmButton;
    [SerializeField] Button behaviorCancelButton;
    // ���� ��� �� ����� �׼� ��ư ���*/

    

    [HideInInspector] public List<IDBtn> enablePlaceBtnList = new();
    //[HideInInspector] public List<IDBtn> enableBehaviorActionBtnList = new();

    // ������ ��� �� ����� �׼� ���
    [SerializeField] public List<string> currentPlaceIDList = new();
    [SerializeField] public List<ButtonValue> currentPlaceList = new();
    /*[HideInInspector] private List<string> currentBehaviorActionIDList = new();
    [HideInInspector] public List<ButtonValue> currentBehaviorActionList = new();*/

    // ������ ��� �� ����� �׼�
    [HideInInspector] public ButtonValue currentPlace = null;
    //private ButtonValue currentBehaviorAction;

    #region Main

    protected override void Awake()
    {
        SetCurrent3DMap(currentPlace);

        //currentPlace = new ButtonValue("P00", DataManager.PlaceDatas[1]["P00"].ToString());

        //SetCurrent3DMap(currentPlace);
        //base.Awake();

        homeBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                // �� ��ư ��Ȱ��ȭ
                //PhoneSoftware.map_Btn.interactable = false;
                // �ڵ��� ����
                PhoneHardware.PhoneOff();
            });


        /*behaviorBackground.OnClickAsObservable()
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
            });*/
    }

    private void Start()
    {
        InitPlace();
        // ���߿� ���������� Ȱ��ȭ
    }

    #endregion

    #region Spawn3Dmap

    public void SetCurrent3DMap(ButtonValue buttonValue)
    {
        if(buttonValue.ID == null || buttonValue.ID == "" || buttonValue.ID == "P00")
        {
            foreach(GameObject go in placeGOList)
            {
                go.SetActive(false);
            }
            placeGOList[0].SetActive(true);
            CheckGetAllDatas.CurrentMap = placeGOList[0];

            SetInteractionObjects.OnInteractiveOB();
            CheckGetAllDatas.gameObject.SetActive(false);
            placeGOList[0].transform.position = Vector3.zero;

        }
        else 
        {
            int s = Convert.ToInt32(buttonValue.ID.Substring(1));
            foreach (GameObject go in placeGOList)
            {
                go.SetActive(false);
            }
            placeGOList[s].SetActive(true);
            CheckGetAllDatas.CurrentMap = placeGOList[s];
            placeGOList[s].transform.position = Vector3.zero;

            SetInteractionObjects.OnInteractiveOB();
            CheckGetAllDatas.gameObject.SetActive(true);
        }

        SetInteractionObjects.transform.parent.gameObject.transform.position = new Vector3(0, 0.2f, 0);

    }

    #endregion

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
                    Debug.Log("������ ���: " + currentPlace.ID + " / " + currentPlace.Name);
                    PhoneHardware.PhoneOff();
                    PhoneHardware.DoNotNeedBtns_ExceptionSituation = true;

                    StartGoingSomewhereLoading(1.5f);


                    /*string text = string.Format("<#D40047><b>{0}</b></color> ���� ������ �ұ�?", currentPlace.Name);
                    behaviorHeaderText.text = text;
                    behaviorPopup.SetActive(true);
                    InitBehaviorActionID(currentPlace.ID);
                    placeBtnSpawner.SpawnBehaviorActionBtn();*/
                });
        }
    }




    /*public void BehaviorActionBtnListSet()
    {
        foreach (IDBtn behaviorActionBtn in enableBehaviorActionBtnList)
        {
            behaviorActionBtn.button
                .OnClickAsObservable()
                .Select(action => behaviorActionBtn.buttonValue)
                .Subscribe(action =>
                {
                    currentBehaviorAction = action;
                    string text = string.Format("<#D40047><b>{0}</b></color> ���� <#D40047><b>{1}</b></color>", currentPlace.Name, currentBehaviorAction.Name);
                    behaviorConfirmText.text = text;
                    behaviorConfirmPopup.SetActive(true);
                });
        }
    }*/
    #endregion

    #region Going Somewhere

    public void StartGoingSomewhereLoading(float delay)
    {
        StartCoroutine(GoingSomewhereLoading(delay));
    }

    private IEnumerator GoingSomewhereLoading(float delay)
    {
        CurrentPlaceTxt.text = (string)DataManager.PlaceDatas[1][currentPlace.ID] + "(��)�� �̵��մϴ�.";
        GoingSomewhereloadingCG.alpha = 0f;
        GoingSomewhereloadingCG.DOFade(1, delay);
        GoingSomewhereloadingCG.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        SetInteractionObjects.OffInteractiveOB();
        SetInteractionObjects.ClearInteractiveOB();
        SetCurrent3DMap(currentPlace);

        yield return new WaitForSeconds(delay);

        GoingSomewhereloadingCG.DOFade(0, delay)
            .OnComplete(() =>
            {
                GoingSomewhereloadingCG.gameObject.SetActive(false);
            });

    }

    #endregion

    #region Init
    public void InitPlace()
    {
        currentPlaceList.Clear(); // �ʱ�ȭ
        foreach (IDBtn btn in placeBtnList)
        {
            btn.button.interactable = false;
        }

        foreach (string id in currentPlaceIDList) // ID ��ȸ
        {
            IDBtn btn = placeBtnList.Find(x => x.buttonValue.ID == id);
            btn.button.interactable = true;
            currentPlaceList.Add(btn.buttonValue);
        }
        PlaceBtnListSet();
    }

    /*private void InitBehaviorActionID(string id)
    {
        currentBehaviorActionIDList.Clear(); // �ʱ�ȭ
        foreach (var data in DataManager.ActionEventDatas[0]) // �׼� �̺�Ʈ ��ȸ
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
        currentBehaviorActionList.Clear(); // �ʱ�ȭ
        foreach (string id in currentBehaviorActionIDList) // ID ��ȸ
        {
            ButtonValue word = new(id, (string)DataManager.BehaviorActionDatas[1][id]);
            currentBehaviorActionList.Add(word);
        }
    }*/
    #endregion
}
