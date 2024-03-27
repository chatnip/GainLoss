using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;
using System.Linq;

public class PlaceManager : Manager<PlaceManager>
{
    [Header("*Property")]
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] PlayerController PlayerController;
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] ActionEventManager ActionEventManager;
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] PhoneSoftware PhoneSoftware;
    [SerializeField] CheckGetAllDatas CheckGetAllDatas;
    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;

    [Header("*Player")]
    [SerializeField] SetInteractionObjects SetInteractionObjects;

    [Header("*Going Somewhere loading")]
    [SerializeField] CanvasGroup GoingSomewhereloadingCG;
    [SerializeField] TMP_Text CurrentPlaceTxt;

    /*[Space(10)]
    [SerializeField] PlacePadSpawner placeBtnSpawner;*/

    [Header("*Place")]
    [SerializeField] Button homeBtn;
    [SerializeField] public List<IDBtn> placeBtnList = new();
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
    // 켜진 장소 및 장소의 액션 버튼 목록*/

    

    [HideInInspector] public List<IDBtn> enablePlaceBtnList = new();
    //[HideInInspector] public List<IDBtn> enableBehaviorActionBtnList = new();

    // 선택한 장소 및 장소의 액션 목록
    [SerializeField] public Dictionary<string, int> currentPlaceID_Dict = new();
    [SerializeField] public List<ButtonValue> currentPlaceList = new();
    /*[HideInInspector] private List<string> currentBehaviorActionIDList = new();
    [HideInInspector] public List<ButtonValue> currentBehaviorActionList = new();*/

    // 선택한 장소 및 장소의 액션
    public ButtonValue currentPlace = null;
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
                // 맵 버튼 비활성화
                //PhoneSoftware.map_Btn.interactable = false;
                // 핸드폰 끄기
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
        // 나중에 켜질때마다 활성화
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
                go.transform.position = new Vector3(100, 0, 0);
            }
            placeGOList[0].SetActive(true);

            placeGOList[0].transform.position = new Vector3(PlayerController.transform.position.x, 0, PlayerController.transform.position.z);

            SetInteractionObjects.OnInteractiveOB();

            foreach (Transform child in placeGOList[0].transform)
            { if (child.TryGetComponent(out InteractObject IO)) { IO.ft_setOnNOP(); } }


            CheckGetAllDatas.CurrentMap = placeGOList[0];
            CheckGetAllDatas.gameObject.SetActive(false);
        }
        else 
        {
            int s = Convert.ToInt32(buttonValue.ID.Substring(1));
            foreach (GameObject go in placeGOList)
            {
                go.SetActive(false);
                go.transform.position = new Vector3(100, 0, 0);
            }
            placeGOList[s].SetActive(true);

            placeGOList[s].transform.position = new Vector3(PlayerController.transform.position.x, 0, PlayerController.transform.position.z);

            int visitAmount = 0; // 방문한 횟수 구하기
            foreach(string ID in currentPlaceID_Dict.Keys)
            {
                if (ID == buttonValue.ID) { visitAmount = Convert.ToInt32(currentPlaceID_Dict[ID]); }
            }

            SetObjectByVisitAmount(placeGOList[s], buttonValue.ID, visitAmount);

            SetInteractionObjects.OnInteractiveOB();

            foreach (Transform child in placeGOList[s].transform)
            { if (child.TryGetComponent(out InteractObject IO) && child.gameObject.activeSelf) { IO.ft_setOnNOP(); } }


            CheckGetAllDatas.CurrentMap = placeGOList[s];
            CheckGetAllDatas.gameObject.SetActive(true);
        }

        //PlayerController.ResetPlayerSpot();
    }
    //시트에 따라 (몇 번 방문했는냐에 따른) 오브젝트 배치
    private void SetObjectByVisitAmount(GameObject MapGO, string ID, int visitAmount)
    {
        // 장소, 방문 횟수로 켜야할 오브젝트의 ID 가져오기
        List<string> setOnObjectIDs = new List<string>();
        foreach(string DataID in new List<string>(DataManager.ObjectEventData[0].Keys.ToList()))
        {
            string[] ID_Values = DataID.Split('P', 'V', 'O');
            if (ID_Values[0] == ID && Convert.ToInt32(ID_Values[1]) == visitAmount) { setOnObjectIDs.Add(ID_Values[2]); }
        }

        // 위 구한 IDs로 오브젝트 활성화 및 비활성화
        foreach (Transform O in MapGO.transform)
        {
            if(O.TryGetComponent(out InteractObject IO) && IO.objectID != "")
            {
                if (setOnObjectIDs.Contains(IO.objectID)) { O.gameObject.SetActive(true); }
                else { O.gameObject.SetActive(false); }
            }
        }

        // 방문 횟수 코드데이터 +1 삽입
        foreach(string placeID in currentPlaceID_Dict.Keys)
        {
            if (placeID == ID) 
            {
                int currentVisitAmout = Convert.ToInt32(currentPlaceID_Dict[placeID]);
                currentVisitAmout++;
                currentPlaceID_Dict[placeID] = currentVisitAmout;
                return;
            }
        }
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
                .Subscribe(placeBV =>
                {
                    SetPlaceBtnSet(placeBV);

                    /*string text = string.Format("<#D40047><b>{0}</b></color> 에서 무엇을 할까?", currentPlace.Name);
                    behaviorHeaderText.text = text;
                    behaviorPopup.SetActive(true);
                    InitBehaviorActionID(currentPlace.ID);
                    placeBtnSpawner.SpawnBehaviorActionBtn();*/
                });
        }
    }

    public void SetPlaceBtnSet(ButtonValue BV)
    {
        currentPlace = BV;
        Debug.Log("정해진 장소: " + currentPlace.ID + " / " + currentPlace.Name);
        PhoneHardware.PhoneOff();
        PhoneHardware.DoNotNeedBtns_ExceptionSituation = true;

        StartGoingSomewhereLoading(1.5f);
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
                    string text = string.Format("<#D40047><b>{0}</b></color> 에서 <#D40047><b>{1}</b></color>", currentPlace.Name, currentBehaviorAction.Name);
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
        PlayerInputController.StopMove();
    }

    private IEnumerator GoingSomewhereLoading(float delay)
    {
        CurrentPlaceTxt.text = (string)DataManager.PlaceDatas[1][currentPlace.ID] + "(으)로 이동합니다.";
        GoingSomewhereloadingCG.alpha = 0f;
        GoingSomewhereloadingCG.DOFade(1, delay);
        GoingSomewhereloadingCG.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        SetInteractionObjects.OffInteractiveOB();
        SetCurrent3DMap(currentPlace);


        yield return new WaitForSeconds(delay);

        GoingSomewhereloadingCG.DOFade(0, delay)
            .OnComplete(() =>
            {
                GoingSomewhereloadingCG.gameObject.SetActive(false);
                PlayerInputController.CanMove = true;
            });

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
        foreach (KeyValuePair<string, int> keyValuePair in currentPlaceID_Dict) // ID 순회
        {
            string id = keyValuePair.Key;
            foreach(IDBtn idBtn in placeBtnList)
            {
                if(idBtn.buttonValue.ID == id)
                {
                    idBtn.button.interactable = true;
                    currentPlaceList.Add(idBtn.buttonValue);
                }
            }
        }
        PlaceBtnListSet();
    }

    /*private void InitBehaviorActionID(string id)
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
    }*/
    #endregion
}
