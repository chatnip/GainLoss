using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleManager : Manager<ScheduleManager>
{
    #region Value

    [HideInInspector] public List<string> currentHaveSchedule = new List<string>();
    [HideInInspector] public List<string> currentSelectedSchedule = new List<string>();

    [HideInInspector] public string currentPrograssSchedule;

    #endregion



}
