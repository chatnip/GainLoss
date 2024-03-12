using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Pause : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Input")]
    [SerializeField] PlayerInputController PlayerInputController;

    [Header("*EnableCamera")]
    [SerializeField] GameObject QuarterViewCamera;
    bool QuarterViewCameraSet = true;
    [SerializeField] List<GameObject> anotherCamerasCanvas;
    List<bool> anotherCamerasCanvasSet;

    [Header("*Btn")]
    [SerializeField] Button resumeBtn;
    [SerializeField] Button backToTitleBtn;
    [SerializeField] Button exitGameBtn;
    List<List<Button>> pauseBtns;

    [Header("*Window")]
    [SerializeField] public GameObject reconfirmWindow;

    [Header("reconfirmBtn")]
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;
    [SerializeField] TMP_Text chooseActionText;

    [HideInInspector] public Button chooseBtn;

    [Header("*WindowFrame")]
    [SerializeField] float AppearTime = 0.3f;
    [SerializeField] float AppearStartSize = 0.5f;

    [SerializeField] float DisappearTime = 0.1f;
    [SerializeField] float DisappearLastSize = 0.75f;

    #endregion

    #region Main

    private void Awake()
    {
        pauseBtns = new List<List<Button>>
        { 
            new List<Button> { resumeBtn },
            new List<Button> { backToTitleBtn },
            new List<Button> { exitGameBtn } 
        };

        resumeBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                ft_closePausePopup();
            });
        backToTitleBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                chooseActionText.text = "[ Back To Title ]";
                chooseBtn = backToTitleBtn;

                EffectfulWindow.AppearEffectful(reconfirmWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
            });
        exitGameBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                chooseActionText.text = "[ Exit Game ]";
                chooseBtn = exitGameBtn;

                EffectfulWindow.AppearEffectful(reconfirmWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);
            });
        noBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                EffectfulWindow.DisappearEffectful(reconfirmWindow.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
            });
        yesBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if(chooseBtn == backToTitleBtn)
                {
                    DOTween.KillAll();
                    SceneManager.LoadScene("Title");
                }
                else if (chooseBtn == exitGameBtn)
                {
                    Application.Quit(); // 어플리케이션 종료
                }
            });
    }


    private void OnEnable()
    {
        PlayerInputController.isPause = true;
        PlayerInputController.StopMove();


        chooseBtn = null;
        anotherCamerasCanvasSet = new List<bool>();
        for(int i = 0; i < anotherCamerasCanvas.Count; i++)
        {
            anotherCamerasCanvasSet.Add(anotherCamerasCanvas[i].activeSelf);
            anotherCamerasCanvas[i].SetActive(false);
        }
        QuarterViewCameraSet = QuarterViewCamera.activeSelf;
        QuarterViewCamera.SetActive(true);

        EffectfulWindow.AppearEffectful(this.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);


    }
    private void OnDisable()
    {
        PlayerInputController.isPause = false;
    }

    public void Interact()
    {
        if (PlayerInputController.SelectBtn == resumeBtn)
        { ft_closePausePopup(); return; }
        else if (PlayerInputController.SelectBtn == backToTitleBtn)
        {
            PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { yesBtn, noBtn } }, this);

            chooseActionText.text = "[ Back To Title ]";
            chooseBtn = backToTitleBtn;

            EffectfulWindow.AppearEffectful(reconfirmWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);


            return;
        }
        else if (PlayerInputController.SelectBtn == exitGameBtn)
        {
            PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { yesBtn, noBtn } }, this);

            chooseActionText.text = "[ Exit Game ]";
            chooseBtn = exitGameBtn;

            EffectfulWindow.AppearEffectful(reconfirmWindow.GetComponent<RectTransform>(), AppearTime, AppearStartSize, Ease.Linear);


            return;
        }

        if(PlayerInputController.SelectBtn == yesBtn) 
        {
            if (chooseBtn == backToTitleBtn)
            {
                DOTween.KillAll();
                SceneManager.LoadScene("Title");
            }
            else if (chooseBtn == exitGameBtn)
            {
                Application.Quit(); // 어플리케이션 종료
            }
            return; 
        }
        else if (PlayerInputController.SelectBtn == noBtn) 
        {
            EffectfulWindow.DisappearEffectful(reconfirmWindow.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
            return; 
        }
    }

    #endregion

    #region OnOff Func

    public void ft_openPausePopup()
    {
        this.gameObject.SetActive(true);
        PlayerInputController.SetSectionBtns(pauseBtns, this);

    }
    public void ft_closePausePopup()
    {
        chooseBtn = null;
        for (int i = 0; i < anotherCamerasCanvas.Count; i++)
        {
            anotherCamerasCanvas[i].SetActive(anotherCamerasCanvasSet[i]);
        }
        anotherCamerasCanvasSet.Clear();
        QuarterViewCamera.SetActive(QuarterViewCameraSet);

        EffectfulWindow.DisappearEffectful(this.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);
        EffectfulWindow.DisappearEffectful(reconfirmWindow.GetComponent<RectTransform>(), DisappearTime, DisappearLastSize, Ease.Linear);

        PlayerInputController.CanMove = true;

        PlayerInputController.ClearSeletedBtns();
    }

    #endregion
}
