//Refactoring v1.0
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PhoneOptionManager : Singleton<PhoneOptionManager>
{
    #region Value

    [Header("=== Phone App")]
    [SerializeField] List<IDBtn> mainOptionAppIDBtns;

    // Other Value
    [SerializeField] public IDBtn currentIdBtn;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Place Btn
        foreach (IDBtn mainOptionBtn in mainOptionAppIDBtns)
        {
            // Set Btn Setting
            mainOptionBtn.buttonText.text = DataManager.Instance.Get_PhoneOptionName(mainOptionBtn.buttonID);
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

    public void DoPhoneOption()
    {
        switch(currentIdBtn.buttonID)
        {
            case "000": // ���� �����ֱ�
                break;
            case "001": // é�� �����
                break;
            case "002": // Ÿ��Ʋ�� ����
                break;
            case "003": // �����ϱ�
                break;
        }
    }

    private void Information() { Debug.Log("Information"); }
    private void Restart() { Debug.Log("Restart"); }
    private void Title() { Debug.Log("Title"); }
    private void Quit() { Debug.Log("Quit"); }

    #endregion
}
