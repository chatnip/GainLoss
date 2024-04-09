using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PartTimeJobManager : Manager<PartTimeJobManager>, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] GameManager GameManager;
    [SerializeField] GameSystem GameSystem;

    [Header("*PartTimeJob")]
    [SerializeField] CanvasGroup partTimeJob_LoadingCG;
    [SerializeField] Button partTimeJob_StartBtn;
    [SerializeField] Button partTimeJob_EndBtn;

    [Header("*Cutscenc")]
    [SerializeField] List<cutsceneSO> allCutSceneSOs;

    [Header("*Other")]
    [SerializeField] TMP_Text moneyTMP;
    #endregion

    #region Main

    protected override void Awake()
    {
        ResetPartTimeJobUI();

        partTimeJob_StartBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                StartCoroutine(StartPartTimeJob(5.0f, selectCSSO()));
            });
        partTimeJob_EndBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                EndPartTimeJob();
            });
    }

    public void Interact()
    {
        if(PlayerInputController.SelectBtn == partTimeJob_EndBtn && partTimeJob_EndBtn.interactable)
        {
            EndPartTimeJob();
        }
    }

    #endregion

    #region Act

    public void distinctionPartTimeJob()
    {
        if (ScheduleManager.currentPrograssScheduleID == "S04")
        {
            Debug.Log("Part-time job");
            partTimeJob_StartBtn.gameObject.SetActive(true);
        }
            
    }

    public cutsceneSO selectCSSO()
    {
        return allCutSceneSOs[0];
    }
    public IEnumerator StartPartTimeJob(float time, cutsceneSO selectCSSO)
    {
        PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { partTimeJob_EndBtn } }, this);
        partTimeJob_LoadingCG.TryGetComponent(out Image image);
        partTimeJob_StartBtn.gameObject.SetActive(false);
        partTimeJob_LoadingCG.gameObject.SetActive(true);
        partTimeJob_LoadingCG.DOFade(1.0f, 0.5f);
        Sequence seq = cutsceneSO.justImgCutscene(image, selectCSSO.cutsceneSprites, time);
        PlayerInputController.StopMove();
        seq.OnComplete(() =>
        {
            partTimeJob_EndBtn.interactable = true;

        });
        yield return new WaitForSeconds(0.5f);
    }

    public void EndPartTimeJob()
    {
        partTimeJob_LoadingCG.DOFade(0.0f, 0.5f)
            .OnStart(() =>
            {
                partTimeJob_EndBtn.interactable = false;
                ScheduleManager.PassNextSchedule();
                GetMoney(100);
            })
            .OnComplete(() =>
            {
                PlayerInputController.SetSectionBtns(null, null);
                ResetPartTimeJobUI();
                PlayerInputController.CanMove = true;
            });
    }
    private void GetMoney(int money)
    {
        GameManager.currentMainInfo.money += money;
        moneyTMP.text = GameManager.currentMainInfo.money.ToString();
    }

    #endregion

    #region Reset

    private void ResetPartTimeJobUI() 
    {
        partTimeJob_EndBtn.interactable = false;

        partTimeJob_LoadingCG.alpha = 0;
        partTimeJob_LoadingCG.gameObject.SetActive(false);
        partTimeJob_StartBtn.gameObject.SetActive(false);   
    }

    #endregion
}
