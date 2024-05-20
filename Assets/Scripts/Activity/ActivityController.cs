using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActivityController : MonoBehaviour, IInteract
{
    #region Value

    [Header("--- Property")]
    [SerializeField] GameManager GameManager;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;

    #region Gage

    [Header("--- Gage")]
    [SerializeField] public RectTransform activityGageWindowRT;
    [SerializeField] Image gageImg;
    [SerializeField] RectTransform markImg;
    [SerializeField] TMP_Text amountNumTxt;

    [Header("--- Question Window")]
    [SerializeField] public RectTransform activityQuestionWindowRT;
    [SerializeField] TMP_Text questionContentTxt;
    [SerializeField] TMP_Text kindOfGageByActivityTxt;
    [SerializeField] TMP_Text AmountNumInWindowTxt;
    [SerializeField] Image gageInWindowImg;
    [SerializeField] Button noBtn;
    [SerializeField] Button yesBtn;

    #endregion

    #region Question Window



    #endregion

    #endregion

    #region Main

    private void Awake()
    {
        ft_setGageUI(
            GameManager.currentMainInfo.currentActivity, 
            GameManager.currentMainInfo.maxActivity,
            4f);

        activityQuestionWindowRT.anchoredPosition = new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y);
        noBtn.interactable = false;
        yesBtn.interactable = false;
    }

    private void Update()
    {
        if (!GameManager.CanInput) { return; }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ft_setOnQuestionWindow(
                "TextContent", 
                "KindOf",
                GameManager.currentMainInfo.currentActivity,
                GameManager.currentMainInfo.maxActivity,
                0.25f);
        }
        else if (Input.GetKeyDown(KeyCode.R)) 
        {
            ft_setOffQuestionWindow(0.25f);
        }
    }
    public void Interact()
    {

    }

    #endregion

    #region Func for Gage

    private void ft_setGageUI(int current, int max, float rotateTime)
    {
        // Set Fill Gage
        float value = (float)current / max;
        gageImg.fillAmount = value;

        // Set RTs Pos
        float RT_X = activityGageWindowRT.rect.width;
        RT_X /= max;
        if (markImg.TryGetComponent(out RectTransform markRT)) { markRT.anchoredPosition = new Vector2(RT_X * current, 0); }
        if (amountNumTxt.TryGetComponent(out RectTransform amountNumRT)) { amountNumRT.anchoredPosition = new Vector2(RT_X * current, 0); }

        // Mark Anime
        markImg.DORotate(new Vector3(0, 0, 360), rotateTime, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetId("ActivityMarkRotate");

        // Set Num
        amountNumTxt.text = current.ToString();

    }

    #endregion

    #region Func for Question Window

    private void ft_setOnQuestionWindow(string questionContent, string kindOfGageByActivity, int currentActivity, int maxActivity, float time)
    {
        if (!GameManager.CanInput) { return; }
        GameManager.CanInput = false;

        // Exp
        if (ObjectInteractionButtonGenerator.SectionIsThis)
        { ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn(); }
        if (PhoneHardware.sectionIsThis)
        { PhoneHardware.SetOnOffPhoneBtn(); }
        PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { noBtn, yesBtn } }, this);
        // set Txt
        questionContentTxt.text = questionContent;
        kindOfGageByActivityTxt.text = kindOfGageByActivity;
        AmountNumInWindowTxt.text = currentActivity.ToString() + "/" + maxActivity.ToString();

        // Set Fill Gage
        float value = (float)currentActivity / maxActivity;
        gageInWindowImg.fillAmount = value;

        activityQuestionWindowRT.DOAnchorPos(new Vector2(720, activityQuestionWindowRT.anchoredPosition.y), time)
            .OnComplete(() =>
            {
                yesBtn.interactable = true;
                noBtn.interactable = true; 
                GameManager.CanInput = true;
            });
    }
    private void ft_setOffQuestionWindow(float time)
    {
        if (!GameManager.CanInput) { return; }
        GameManager.CanInput = false;

        yesBtn.interactable = false;
        noBtn.interactable = false;

        activityQuestionWindowRT.DOAnchorPos(new Vector2(1200, activityQuestionWindowRT.anchoredPosition.y), time)
            .OnComplete(() => { GameManager.CanInput = true; });
    }

   

    #endregion
}
