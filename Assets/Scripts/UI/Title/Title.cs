using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.IO;
using System.Collections.Generic;

public class Title : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] TitleInputController TitleInputController;
    [SerializeField] CanvasScaler TitleCanvasScaler;

    [Header("*TitleBtn")]
    [SerializeField] Button newGameBtn;
    [SerializeField] Button continueBtn;
    [SerializeField] GameObject cannotUseContinue;
    [SerializeField] Button OptionBtn;
    [SerializeField] Button QuitBtn;
    List<Button> btns;

    [Header("*Window")]
    [SerializeField] Image BlackScreenImg;
    [SerializeField] CanvasGroup warningTextCG;
    [SerializeField] CanvasGroup teamLogoCG;
    [SerializeField] GameObject OptionWindow;

    #endregion

    #region Main

    private void Awake()
    {
        btns = new List<Button>()
        {
            newGameBtn, continueBtn, OptionBtn, QuitBtn
        };

        MainInfo SavedMainInfo = JsonLoad_MI();
        if(SavedMainInfo.day == 1 )
        {
            continueBtn.interactable = false;
            cannotUseContinue.SetActive(true);
        }


        newGameBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                BlackScreenImg.gameObject.SetActive(true);
                BlackScreenImg.DOFade(1.0f, 0.7f)
                    .SetEase(Ease.InOutBack)
                    .OnComplete(() =>
                    {
                        MainInfo newMainInfo = new MainInfo();
                        newMainInfo.NewGame = true;
                        JsonSave(newMainInfo);
                        DOTween.KillAll();
                        SceneManager.LoadScene("Main");
                    });
            });
        continueBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                BlackScreenImg.gameObject.SetActive(true);
                BlackScreenImg.DOFade(1.0f, 0.7f)
                    .SetEase(Ease.InOutBack)
                    .OnComplete(() =>
                    {
                        DOTween.KillAll();
                        SceneManager.LoadScene("Main");
                    });
            });
        OptionBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                EffectfulWindow.AppearEffectful(OptionWindow.GetComponent<RectTransform>(), 0.2f, 0.0f, Ease.InOutBack);
            });
        QuitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Application.Quit();
            });

        

    }
    private void OnEnable()
    {
        ft_StartTitle();
    }
    
    public void Interact()
    {
        if(TitleInputController.SelectBtn == newGameBtn)
        {
            BlackScreenImg.gameObject.SetActive(true);
            BlackScreenImg.DOFade(1.0f, 0.7f)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    MainInfo newMainInfo = new MainInfo();
                    newMainInfo.NewGame = true;
                    JsonSave(newMainInfo);
                    DOTween.KillAll();
                    SceneManager.LoadScene("Main");
                });
        }
        if (TitleInputController.SelectBtn == continueBtn)
        {
            BlackScreenImg.gameObject.SetActive(true);
            BlackScreenImg.DOFade(1.0f, 0.7f)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    DOTween.KillAll();
                    SceneManager.LoadScene("Main");
                });
        }
        if (TitleInputController.SelectBtn == OptionBtn)
        {
            EffectfulWindow.AppearEffectful(OptionWindow.GetComponent<RectTransform>(), 0.2f, 0.0f, Ease.InOutBack);
        }
        if (TitleInputController.SelectBtn == QuitBtn)
        {
            Application.Quit();
        }

    }

    #endregion

    #region Start


    private void ft_StartTitle()
    {
        TitleInputController.SetSectionBtns(btns, this);

        Sequence seq = DOTween.Sequence();


        // 경고문
        warningTextCG.alpha = 0.0f;
        warningTextCG.gameObject.SetActive(true);

        seq.Append(warningTextCG.DOFade(1, 0.5f));
        seq.AppendInterval(1f);
        seq.Append(warningTextCG.DOFade(0, 0.5f)
            .OnComplete(() =>
            {
                warningTextCG.gameObject.SetActive(false);
            }));

        // 팀 로고
        teamLogoCG.alpha = 0.0f;
        teamLogoCG.gameObject.SetActive(true);

        seq.Append(teamLogoCG.DOFade(1, 0.5f));
        seq.AppendInterval(1f);
        seq.Append(teamLogoCG.DOFade(0, 0.5f)
            .OnComplete(() =>
            {
                teamLogoCG.gameObject.SetActive(false);

                TitleCanvasScaler.scaleFactor = 0.5f;
            }));

        seq.AppendInterval(0.25f);

        // 메인 타이틀 씬
        seq.Append(DOTween.To(() => TitleCanvasScaler.scaleFactor, x => TitleCanvasScaler.scaleFactor = x, 1f, 0.5f));

        
        // +(동시) 블랙스크린 없애기
        BlackScreenImg.gameObject.SetActive(true);
        seq.Join(BlackScreenImg.DOFade(0.0f, 0.5f)
            .SetEase(Ease.InOutBack)
            .OnComplete(() =>
            {
                BlackScreenImg.gameObject.SetActive(false);
            }));
    }

    #endregion

    #region Data
    private void JsonSave(MainInfo mainDatas)
    {
        if (!Directory.Exists("Assets/Resources/Json/"))
        {
            Directory.CreateDirectory("Assets/Resources/Json/");
        }

        string saveJson = JsonUtility.ToJson(mainDatas, true);

        string saveFilePath = "Assets/Resources/Json/" + "mainInfoDatabase.json";
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }

    private MainInfo JsonLoad_MI()
    {
        if (!File.Exists("Assets/Resources/Json/" + "mainInfoDatabase.json"))
        {
            Debug.LogError("None File this Path");
            return null;
        }

        string path = "Assets/Resources/Json/" + "mainInfoDatabase.json";
        string loadJson = File.ReadAllText(path);
        MainInfo mainInfo = JsonUtility.FromJson<MainInfo>(loadJson);

        return mainInfo;
    }

    #endregion
}
