//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class TaskBar : MonoBehaviour
{
    #region Value

    [Header("=== UI")]
    [SerializeField] Button powerBtn;
    [SerializeField] TMP_Text CurrentDayText;
    [SerializeField] TMP_Text CurrentDayOfWeekText;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        CurrentDayText.text = "DAY " + GameManager.Instance.mainInfo.Day;
        CurrentDayOfWeekText.text = GameManager.Instance.mainInfo.TodayOfTheWeek;
    }

    private void Awake()
    {   
        powerBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Desktop.Instance.TurnOff();
            });
    }

    #endregion
}
