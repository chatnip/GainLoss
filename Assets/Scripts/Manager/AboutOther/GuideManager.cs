//Refactoring v1.0
using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GuideManager : Singleton<GuideManager>
{
    #region Value

    [Header("=== Tutorial")]
    [SerializeField] List<TutorialController> TutorialControllers;
    [SerializeField] CanvasGroup panelTutorialCG;
    [SerializeField] Button tutorialWindowExitBtn;

    [Header("=== Guide")]
    [SerializeField] List<GuideSet> guideSets;

    #endregion

    #region Framework & Base

    public void Offset()
    {
        // Tutorial Controller (Each)
        foreach(TutorialController _controller in TutorialControllers)
        { _controller.Offset(); }

        // UI
        panelTutorialCG.alpha = 0f;
        panelTutorialCG.gameObject.SetActive(false);

        //Btn
        tutorialWindowExitBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ActiveOff();
            });
    }

    protected override void Awake()
    {
       base.Awake();
    }

    #endregion

    #region Check have Tutorial

    public void PlayTutorial_WhenHad()
    {
        string tutorialID = DataManager.Instance.GetID_HaveTutorial(
            GameManager.Instance.currentChapter,
            GameManager.Instance.mainInfo.Day.ToString(),
            Enum.GetName(typeof(GameManager.e_currentActPart), GameManager.Instance.currentActPart));

        // 챕터, 날짜, 상태의 조건이 충족 시
        if(tutorialID != "")
        { ActiveOn(tutorialID); }
    }

    #endregion

    #region Active ON/OFF Tutorial

    public void ActiveOn(string tutorialID)
    {
        GameManager.Instance.canInput = false;

        panelTutorialCG.gameObject.SetActive(true);

        foreach (TutorialController _controller in TutorialControllers)
        {
            if(_controller.thisID == tutorialID)
            { _controller.gameObject.SetActive(true); }
            else
            { _controller.gameObject.SetActive(false); }
        }

        if (DOTween.IsTweening(panelTutorialCG)) { DOTween.Kill(panelTutorialCG); }
        panelTutorialCG.DOFade(1f, 1f)
            .OnComplete(() =>
            {
                GameManager.Instance.canInput = true;
            });
    }
    public void ActiveOff()
    {
        GameManager.Instance.canInput = false;


        if (DOTween.IsTweening(panelTutorialCG)) { DOTween.Kill(panelTutorialCG); }
        panelTutorialCG.DOFade(0f, 1f)
            .OnComplete(() =>
            {
                panelTutorialCG.gameObject.SetActive(false); 
                
                foreach (TutorialController _controller in TutorialControllers)
                { _controller.gameObject.SetActive(false); }

                GameManager.Instance.canInput = true;
            });
    }

    #endregion

    #region GuideArrow

    public void SetGuideArrow()
    {
        foreach (GuideSet GS in guideSets)
        {
            if(GS.thisActPart == GameManager.Instance.currentActPart)
            {
                foreach(Transform tf in GS.guideTFs)
                {
                    tf.gameObject.SetActive(true); 
                    tf.localPosition = Vector3.zero;
                    tf.DOLocalMoveY(0.07f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
                }
            }
            else
            {
                foreach (Transform tf in GS.guideTFs)
                {
                    tf.gameObject.SetActive(false);
                    if (DOTween.IsTweening(tf)) 
                    { 
                        DOTween.Kill(tf); 
                        tf.localPosition = Vector3.zero; 
                    } 
                }
            }
        }
    }

    #endregion

    #region Set UI Each State

    public void SetActiveOnOffGOs()
    {
        foreach (GuideSet GS in guideSets)
        {
            if (GS.thisActPart == GameManager.Instance.currentActPart)
            {
                foreach (GameObject go in GS.activeOnGO_WhenThisTime) { go.gameObject.SetActive(true); }
                foreach (GameObject go in GS.activeOffGO_WhenThisTime) { go.gameObject.SetActive(false); }
            }
        }
    }

    #endregion

}

[System.Serializable]
public class GuideSet
{
    public GameManager.e_currentActPart thisActPart;
    public List<Transform> guideTFs;

    public List<GameObject> activeOnGO_WhenThisTime;
    public List<GameObject> activeOffGO_WhenThisTime;
}


