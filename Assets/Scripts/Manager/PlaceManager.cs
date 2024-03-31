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
    [SerializeField] Camera MainCamera;

    [Header("*Player")]
    [SerializeField] SetInteractionObjects SetInteractionObjects;

    [Header("*Going Somewhere loading")]
    [SerializeField] CanvasGroup GoingSomewhereloadingCG;
    [SerializeField] TMP_Text CurrentPlaceTxt;
    [SerializeField] TMP_Text HUD_currentPlactTxt;

    [Header("*Place")]
    [SerializeField] Button homeBtn;
    [SerializeField] public List<IDBtn> placeBtnList = new();
    [Tooltip("Must make sure to get the order right FOR CSV")]
    [SerializeField] List<GameObject> placeGOList = new();
    [SerializeField] List<Color> placeColorList = new();

    [HideInInspector] public List<IDBtn> enablePlaceBtnList = new();

    // ������ ��� �� ����� �׼� ���
    [SerializeField] public Dictionary<string, int> currentPlaceID_Dict = new();
    [SerializeField] public List<ButtonValue> currentPlaceList = new();

    // ������ ��� �� ����� �׼�
    public ButtonValue currentPlace = null;

    #region Main

    protected override void Awake()
    {
        SetCurrent3DMap(currentPlace);

        homeBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                PhoneHardware.PhoneOff();
            });
    }

    private void Start()
    {
        InitPlace();
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
                go.transform.position =
                    new Vector3(100, 0, 0);
            }
            placeGOList[0].SetActive(true);
            placeGOList[0].transform.position = new Vector3(PlayerController.transform.position.x, 0, PlayerController.transform.position.z);
            MainCamera.backgroundColor = placeColorList[0];
            

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
            MainCamera.backgroundColor = placeColorList[s];
            placeGOList[s].transform.position = new Vector3(PlayerController.transform.position.x, 0, PlayerController.transform.position.z);

            int visitAmount = 0; // �湮�� Ƚ�� ���ϱ�
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
    //��Ʈ�� ���� (�� �� �湮�ߴ³Ŀ� ����) ������Ʈ ��ġ
    private void SetObjectByVisitAmount(GameObject MapGO, string ID, int visitAmount)
    {
        // ���, �湮 Ƚ���� �Ѿ��� ������Ʈ�� ID ��������
        List<string> setOnObjectIDs = new List<string>();
        foreach(string DataID in new List<string>(DataManager.ObjectEventData[0].Keys.ToList()))
        {
            string[] ID_Values = DataID.Split('P', 'V', 'O');
            if (ID_Values[0] == ID && Convert.ToInt32(ID_Values[1]) == visitAmount) { setOnObjectIDs.Add(ID_Values[2]); }
        }

        // �� ���� IDs�� ������Ʈ Ȱ��ȭ �� ��Ȱ��ȭ
        foreach (Transform O in MapGO.transform)
        {
            if(O.TryGetComponent(out InteractObject IO) && IO.objectID != "")
            {
                if (setOnObjectIDs.Contains(IO.objectID)) { O.gameObject.SetActive(true); }
                else { O.gameObject.SetActive(false); }
            }
        }

        // �湮 Ƚ�� �ڵ嵥���� +1 ����
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
                });
        }
    }

    public void SetPlaceBtnSet(ButtonValue BV)
    {
        currentPlace = BV;
        Debug.Log("������ ���: " + currentPlace.ID + " / " + currentPlace.Name);
        PhoneHardware.PhoneOff();
        PhoneHardware.DoNotNeedBtns_ExceptionSituation = true;

        StartGoingSomewhereLoading(1.5f);
    }

    #endregion

    #region Going Somewhere

    public void StartGoingSomewhereLoading(float delay)
    {
        StartCoroutine(GoingSomewhereLoading(delay));
        PlayerInputController.StopMove();
    }

    private IEnumerator GoingSomewhereLoading(float delay)
    {
        CurrentPlaceTxt.text = (string)DataManager.PlaceDatas[1][currentPlace.ID] + "(��)�� �̵��մϴ�.";
        GoingSomewhereloadingCG.alpha = 0f;
        GoingSomewhereloadingCG.DOFade(1, delay);
        GoingSomewhereloadingCG.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        SetInteractionObjects.OffInteractiveOB();
        SetCurrent3DMap(currentPlace);
        HUD_currentPlactTxt.text = (string)DataManager.PlaceDatas[1][currentPlace.ID];

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
        currentPlaceList.Clear(); // �ʱ�ȭ
        foreach (IDBtn btn in placeBtnList)
        {
            btn.button.interactable = false;
        }
        foreach (KeyValuePair<string, int> keyValuePair in currentPlaceID_Dict) // ID ��ȸ
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

    
    #endregion
}
