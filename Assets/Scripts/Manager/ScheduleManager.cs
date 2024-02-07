using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleManager : Manager<ScheduleManager>
{
    #region Value
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] ActionEventManager ActionEventManager;

    [HideInInspector] public List<string> currentHaveScheduleID = new List<string>();
    [HideInInspector] public List<string> currentSelectedScheduleID = new List<string>();

    [HideInInspector] public string currentPrograssScheduleID;

    [Header("*Btn")]
    [SerializeField] public Button EndDayBtn;

    #endregion

    protected override void Awake()
    {
        EndDayBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ActionEventManager.TurnOnLoading();
            });
    }


    public void PassNextSchedule()
    {
        if (currentPrograssScheduleID == "" || currentPrograssScheduleID == null || currentPrograssScheduleID == "S00")
        {
            if (currentSelectedScheduleID.Count == 0)
            {
                SchedulePrograss.Set_InStartScheduleUI();
                return;
            }
            
        }
        else
        {
            Debug.Log(currentPrograssScheduleID);
            
            if (currentPrograssScheduleID == currentSelectedScheduleID[0])
            {
                currentPrograssScheduleID = currentSelectedScheduleID[1];
                SchedulePrograss.Set_InPMScheduleUI();
                return;
            }
            else if (currentPrograssScheduleID == currentSelectedScheduleID[1])
            {
                currentPrograssScheduleID = "S99";
                SchedulePrograss.Set_InEndScheduleUI();

                EndDayBtn.gameObject.SetActive(true);
                return;
            }

            /*for (int i = 0; i < currentSelectedScheduleID.Count; i++)
            {
                if ((currentPrograssScheduleID == currentSelectedScheduleID[i]) && (i == 0))
                {
                    SchedulePrograss.Set_InAMScheduleUI();
                    return;
                }
                else if ((currentPrograssScheduleID == currentSelectedScheduleID[i]) && (i == 1))
                {
                    currentPrograssScheduleID = "S99";
                    SchedulePrograss.Set_InEndScheduleUI();
                    EndDayBtn.gameObject.SetActive(true);
                    return;
                }

            }*/

        }

    }

    public void ResetDay()
    {
        currentSelectedScheduleID.Clear();
        currentPrograssScheduleID = "S00";
        EndDayBtn.gameObject.SetActive (false);
        PassNextSchedule();
    }

}
