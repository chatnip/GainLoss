using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleManager : Manager<ScheduleManager>
{
    #region Value

    [HideInInspector] public List<string> currentHaveScheduleID = new List<string>();
    [HideInInspector] public List<string> currentSelectedScheduleID = new List<string>();

    [HideInInspector] public string currentPrograssScheduleID;

    #endregion



}
