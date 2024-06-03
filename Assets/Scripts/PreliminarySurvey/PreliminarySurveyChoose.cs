using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PreliminarySurveyChoose : MonoBehaviour
{
    #region Value

    [Header("*Property")]
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] DesktopController Desktop;
    [SerializeField] PreliminarySurveyWindow_FindClue PSWindow_FindClue;
    [SerializeField] PreliminarySurveyWindow_Extract PSWindow_Extract;
    

    [Header("*Element")]
    [SerializeField] Button FindClueBtn;
    [SerializeField] Button ExtractBtn;
    [SerializeField] Button ExitBtn;

    #endregion

    #region Main

    private void Awake()
    {
        FindClueBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                this.gameObject.SetActive(false);
                PSWindow_FindClue.gameObject.SetActive(true);
            });

        ExtractBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                this.gameObject.SetActive(false);
                PSWindow_Extract.gameObject.SetActive(true);
            });


        ExitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                EffectfulWindow.DisappearEffectful(this.gameObject.GetComponent<RectTransform>(), Desktop.DisappearTime, Desktop.DisappearLastSize, Ease.Linear);
            });
        
    }

    private void OnEnable()
    {
        SetEachOnOffBtn();
        //PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { FindClueBtn, ExtractBtn } }, this);
    }

    private void SetEachOnOffBtn()
    {
        FindClueBtn.interactable = true;
        ExtractBtn.interactable= true;

    }

    public void Interact()
    {
        if(PlayerInputController.SelectBtn == FindClueBtn && FindClueBtn.interactable)
        {
            this.gameObject.SetActive(false);
            PSWindow_FindClue.gameObject.SetActive(true);
        }
        else if (PlayerInputController.SelectBtn == ExtractBtn && ExtractBtn.interactable)
        {
            this.gameObject.SetActive(false);
            PSWindow_Extract.gameObject.SetActive(true);
        }
    }

    #endregion
}
