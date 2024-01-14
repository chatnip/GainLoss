using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class CheckGetAllDatas : MonoBehaviour
{
    [SerializeField] public GameObject CurrentMap;
    [SerializeField] TMP_Text Info;
    [SerializeField] Button TerminateBtn;

    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] SetInteractionObjects SetInteractionObjects;

    private void Awake()
    {
        TerminateBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ScheduleManager.PassNextSchedule();
                PlaceManager.currentPlace = new ButtonValue("P00", (string)DataManager.PlaceDatas[1]["P00"]);
                PlaceManager.StartGoingSomewhereLoading(1.5f);
                
                this.gameObject.SetActive(false);
            });
    }

    private void OnEnable()
    {
        ApplyTerminateBtnAndText();
    }

    public void ApplyTerminateBtnAndText()
    {
        if (checkGetAllDatas(CurrentMap) <= 0)
        {
            Info.text = "Terminate this Part";
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
            if (child.TryGetComponent(out InteractObject interactObject))
            {
                if (interactObject.getWordID != "" || interactObject.getWordActionID != "")
                {
                    _remain++;
                    //Debug.Log("아직 존재");
                }
            }
        }

        return _remain;
    }

}