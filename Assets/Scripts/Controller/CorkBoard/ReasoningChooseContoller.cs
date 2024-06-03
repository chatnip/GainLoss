//Refactoring v1.0
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ReasoningChooseContoller : Singleton<ReasoningChooseContoller>
{
    #region Value

    [Header("=== ChooseBtn")]
    [SerializeField] public CanvasGroup chooseBtn_CG;
    [SerializeField] RectTransform chooseBtn_RT;

    [Header("=== Confirm Popup")]
    [SerializeField] CanvasGroup confirmPopup_CG;
    [SerializeField] Button confirmYesBtn;
    [SerializeField] Button confirmNoBtn;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Set Component
        chooseBtn_CG.gameObject.SetActive(false);
        chooseBtn_CG.alpha = 0f;
        chooseBtn_RT.anchoredPosition = new Vector2(-chooseBtn_RT.sizeDelta.x, chooseBtn_RT.anchoredPosition.y);

        confirmPopup_CG.gameObject.SetActive(false);
        confirmPopup_CG.alpha = 0f;

        // Btn
        confirmYesBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ReasoningController.Instance.GetChapter();
            });
        confirmNoBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ActiveOff_ConfirmPopup(0.2f);
            });
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Panel Choose Btn

    public void ActiveOn_ChooseBtn(float time)
    {
        chooseBtn_CG.gameObject.SetActive(true);
        chooseBtn_CG.DOFade(1f, time);
        chooseBtn_RT.DOAnchorPos(new Vector2(0f, chooseBtn_RT.anchoredPosition.y), time);
    }

    public void ActiveOff_ChooseBtn(float time)
    {
        chooseBtn_CG.DOFade(0f, time);
        chooseBtn_RT.DOAnchorPos(new Vector2(-chooseBtn_RT.sizeDelta.x, chooseBtn_RT.anchoredPosition.y), time)
            .OnComplete(() =>
            {
                chooseBtn_CG.gameObject.SetActive(false);
            });
    }

    #endregion

    #region Panel Confirm Popup

    public void ActiveOn_ConfirmPopup(float time)
    {
        confirmPopup_CG.gameObject.SetActive(true);
        confirmPopup_CG.DOFade(1f, time);
    }
    public void ActiveOff_ConfirmPopup(float time)
    {
        confirmPopup_CG.DOFade(0f, time)
            .OnComplete(() => { confirmPopup_CG.gameObject.SetActive(false); });
    }

    #endregion
}
