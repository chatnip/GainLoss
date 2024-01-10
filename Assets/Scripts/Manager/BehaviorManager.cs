using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviorManager : Manager<BehaviorManager>
{
    [HideInInspector] public List<string> currentHaveBehaviors = new List<string>();
    [HideInInspector] public List<string> currentSelectedBehaviors = new List<string>();
}
