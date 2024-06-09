//Refactoring v1.0
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MainOptionManager : Singleton<MainOptionManager>
{
    #region Value

    [Header("=== Phone App")]
    [SerializeField] List<IDBtn> mainOptionAppIDBtns;
    List<eachPhoneApp> mainOptionAppPlays;
    public Dictionary<IDBtn, eachPhoneApp> mainOptionAppPlaysDict;

    // Other Value
    IDBtn currentIdBtn;
    public delegate void eachPhoneApp();

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Dict
        mainOptionAppPlays = new List<eachPhoneApp>
        {
            new eachPhoneApp(Information),
            new eachPhoneApp(Restart),
            new eachPhoneApp(Title),
            new eachPhoneApp(Quit)
        };
        mainOptionAppPlaysDict = new Dictionary<IDBtn, eachPhoneApp>();
        for(int i = 0; i < mainOptionAppIDBtns.Count; i++)
        { mainOptionAppPlaysDict.Add(mainOptionAppIDBtns[i], mainOptionAppPlays[i]); }


        // Set Place Btn
        foreach (IDBtn mainOptionBtn in mainOptionAppIDBtns)
        {
            // Set Btn Setting
            //mainOptionBtn.buttonText.text = DataManager.Instance.PhoneOptionAppCSVDatas[LanguageManager.Instance.languageNum][mainOptionBtn.buttonID].ToString();

            mainOptionBtn.button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (!GameManager.Instance.canInput) { return; }

                    currentIdBtn = mainOptionBtn;
                    PhoneSoftware.Instance.OpenPopup(currentIdBtn, 0.5f);
                });
        }

    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Each Button

    private void Information() { Debug.Log("Information"); }
    private void Restart() { Debug.Log("Restart"); }
    private void Title() { Debug.Log("Title"); }
    private void Quit() { Debug.Log("Quit"); }

    #endregion
}
