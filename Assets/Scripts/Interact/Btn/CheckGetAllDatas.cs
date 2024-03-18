using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class CheckGetAllDatas : MonoBehaviour
{
    #region Value

    [Header("*Map")]
    [SerializeField] public GameObject CurrentMap;

    [Header("*Component")]
    [SerializeField] TMP_Text Info;
    [SerializeField] public Button TerminateBtn;

    [Header("*Property")]
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] SetInteractionObjects SetInteractionObjects;
    [SerializeField] PhoneHardware PhoneHardware;

    #endregion

    #region Main

    private void Awake()
    {
        TerminateBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                TerminatePlaceAndGoHome();
            });
    }
    
    public void TerminatePlaceAndGoHome()
    {
        ScheduleManager.PassNextSchedule();
        PlaceManager.currentPlace = new ButtonValue("P00", (string)DataManager.PlaceDatas[1]["P00"]);
        PlaceManager.StartGoingSomewhereLoading(1.5f);
        PhoneHardware.DoNotNeedBtns_ExceptionSituation = false;
        TerminateBtn.gameObject.SetActive(false);   
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ApplyTerminateBtnAndText();
    }

    #endregion

    #region

    public void ApplyTerminateBtnAndText()
    {
        if (checkGetAllDatas(CurrentMap) <= 0)
        {
            Info.text = "Complete";
            TerminateBtn.gameObject.SetActive(true);
        }
        else
        {
            Info.text = checkGetAllDatas(CurrentMap) + " Remain(s)";
            TerminateBtn.gameObject.SetActive(false);
        }
    }

    private int checkGetAllDatas(GameObject OBparentMap)
    {
        int _remain = 0;

        Transform[] allChildren = OBparentMap.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.TryGetComponent(out InteractObject interactObject) && child.gameObject.activeSelf)
            {
                if (interactObject.getWordID != "" || interactObject.getWordActionID != "" || interactObject.getPlaceID != "")
                {
                    _remain++;
                    //Debug.Log("아직 존재");
                }
            }
        }

        return _remain;
    }

    #endregion
}