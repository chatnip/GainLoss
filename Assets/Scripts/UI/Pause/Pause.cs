using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Pause : MonoBehaviour
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
        resumeBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                closePausePopup();
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


    public void OnEnable()
    {
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

        PlayerInputController.CanMove = false;

    }
    public void closePausePopup()
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
    }

    #endregion
}
