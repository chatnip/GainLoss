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
    [Header("*Property")]
    [SerializeField] TitleInputController TitleInputController;

    [Header("*TitleBtn")]
    [SerializeField] Button newGameBtn;
    [SerializeField] Button continueBtn;
    [SerializeField] GameObject cannotUseContinue;
    [SerializeField] Button OptionBtn;
    [SerializeField] Button QuitBtn;
    List<Button> btns;

    [Header("*Window")]
    [SerializeField] Image BlackScreenImg;
    [SerializeField] GameObject OptionWindow;


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
        TitleInputController.SetSectionBtns(btns, this);
        BlackScreenImg.gameObject.SetActive(true);
        EffectfulWindow.AppearEffectful(this.GetComponent<RectTransform>(), 1f, 0.5f, Ease.InOutBack);
        BlackScreenImg.DOFade(0.0f, 0.5f)
            .SetEase(Ease.InOutBack)
            .OnComplete(() =>
            {
                BlackScreenImg.gameObject.SetActive(false);
            });
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

}
