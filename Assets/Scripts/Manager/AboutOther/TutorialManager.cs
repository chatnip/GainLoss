//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : Singleton<TutorialManager>
{
    #region Value

    [Header("=== Property")]
    [SerializeField] List<TutorialController> TutorialControllers;

    [Header("=== UI")]
    [SerializeField] CanvasGroup panelTutorialCG;
    [SerializeField] Button tutorialWindowExitBtn;

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

    #region Active ON/OFF

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
}


