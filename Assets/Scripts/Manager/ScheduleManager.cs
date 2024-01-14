using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleManager : Manager<ScheduleManager>
{
    #region Value
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] SchedulePrograss SchedulePrograss;

    [HideInInspector] public List<string> currentHaveScheduleID = new List<string>();
    [HideInInspector] public List<string> currentSelectedScheduleID = new List<string>();

    [HideInInspector] public string currentPrograssScheduleID;

    #endregion


    public void PassNextSchedule()
    {
        if (currentPrograssScheduleID == "" || currentPrograssScheduleID == null)
        {
            SchedulePrograss.Set_InStartScheduleUI();
        }
        else
        {
            for (int i = 0; i < currentHaveScheduleID.Count; i++)
            {
                if ((currentPrograssScheduleID == currentSelectedScheduleID[i]) && (i == 0))
                {
                    currentPrograssScheduleID = currentSelectedScheduleID[1];
                    SchedulePrograss.Set_InPMScheduleUI();

                }
                else if ((currentPrograssScheduleID == currentSelectedScheduleID[i]) && (i == 0))
                {
                    currentPrograssScheduleID = "S99";
                    SchedulePrograss.Set_InEndScheduleUI();
                }

            }

        }

    }

}
