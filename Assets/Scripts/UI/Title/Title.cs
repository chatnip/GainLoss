using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.IO;
using System.Collections.Generic;
using Spine.Unity;

public class Title : MonoBehaviour
{
    #region Value

    [Header("*Property")]
    [SerializeField] public TitleInputController TitleInputController;
    [SerializeField] JsonManager JsonManager;
    [SerializeField] CanvasScaler TitleCanvasScaler;
    [SerializeField] Camera MainCamera;

    [Header("*TitleBtn")]
    [SerializeField] Button newGameBtn;
    [SerializeField] Button continueBtn;
    [SerializeField] public Button OptionBtn;
    [SerializeField] Button QuitBtn;
    List<List<Button>> btns;

    [Header("*Window")]
    [SerializeField] Image BlackScreenImg;
    [SerializeField] CanvasGroup warningTextCG;
    [SerializeField] CanvasGroup teamLogoCG;
    [SerializeField] Spine.Unity.SkeletonGraphic LogoSG;
    [SerializeField] AnimationReferenceAsset LogoARA;
    [SerializeField] GameObject OptionWindow;

    [Header("*Move Effect By Pointer RTs")]
    [SerializeField] RectTransform TitleLogoRT;
    [SerializeField] List<RectTransform> MoveEffectRTList;
    [Tooltip("Dont change this - only Check")][SerializeField] List<Vector3> OffsetRotationList;

    [Header("*BGM")]
    [SerializeField] AudioSource Bgm;

    #endregion

    #region Framework

    private void Awake()
    {
        btns = new List<List<Button>>()
        {
            new List<Button> { newGameBtn, OptionBtn },
            new List<Button> { continueBtn, QuitBtn }
        };

        OffsetRotationList = new List<Vector3>();
        for (int i = 0; i < MoveEffectRTList.Count; i++)
        {
            Vector3 v = MoveEffectRTList[i].rotation.eulerAngles;
            OffsetRotationList.Add(v);
        }


        newGameBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Bgm.DOFade(0, 0.5f).SetEase(Ease.InOutSine);
                BlackScreenImg.gameObject.SetActive(true);
                BlackScreenImg.DOFade(1.0f, 0.7f)
                    .SetEase(Ease.InOutBack)
                    .OnComplete(() =>
                    {
                        DOTween.KillAll();
                        SceneManager.LoadScene("Main");
                    });
            });
        continueBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                
            });
        OptionBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                
            });
        QuitBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Application.Quit();
            });

        StartTitle();
        SetEfectful_TitleLogo();
    }

    private void LateUpdate()
    {
        setEffectful_RotateByPointer(getMousePos());
    }


    #endregion

    #region Start
    private void StartTitle()
    {
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

        seq.Append(teamLogoCG.DOFade(1, 0.2f));

        seq.AppendInterval(3.2f);
        seq.Append(LogoSG.DOFade(0, 0.5f));
        seq.Append(teamLogoCG.DOFade(0, 0.5f)
            .OnComplete(() =>
            {
                teamLogoCG.gameObject.SetActive(false);
                TitleCanvasScaler.scaleFactor = 0.5f;
            }));

        seq.AppendInterval(0.25f);

        // 메인 타이틀 씬
        seq.Append(setEffectful_ComeinScreen());
        //seq.Append(DOTween.To(() => TitleCanvasScaler.scaleFactor, x => TitleCanvasScaler.scaleFactor = x, 1f, 0.5f));

        // +(동시) 블랙스크린 없애기
        BlackScreenImg.gameObject.SetActive(true);
        seq.Join(BlackScreenImg.DOFade(0.0f, 0.5f)
            .SetEase(Ease.InOutBack)
            .OnComplete(() =>
            {
                BlackScreenImg.gameObject.SetActive(false);
                Bgm.Play();
            }));
    }

    #endregion

    #region Data
    private void JsonSave(MainInfo mainDatas)
    {
        /*if (!Directory.Exists("Assets/Resources/Json/"))
        {
            Directory.CreateDirectory("Assets/Resources/Json/");
        }*/

        string saveJson = JsonUtility.ToJson(mainDatas, true);
        string saveFilePath = Application.persistentDataPath + "/mainInfoDatabase.json";

#if UNITY_EDITOR
        saveFilePath = "Assets/Resources/Json/mainInfoDatabase.json";
#endif

        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
    }

    public MainInfo JsonLoad_MI(string jsonPath, string jsonName)
    {
        /*string path = jsonPath + jsonName;
        TextAsset loadJson = Resources.Load<TextAsset>(path);
        MainInfo mainInfo = JsonUtility.FromJson<MainInfo>(loadJson.ToString());
*/
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/" + jsonName + ".json");
        MainInfo mainInfo = JsonUtility.FromJson<MainInfo>(jsonString);

#if UNITY_EDITOR
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/" + jsonPath + jsonName + ".json");
        mainInfo = JsonUtility.FromJson<MainInfo>(jsonString);
#endif

        return mainInfo;
    }

    #endregion

    #region Effectful

    // 마우스 좌표값 받아오기
    private Vector2 getMousePos()
    {
        Vector3 point = Input.mousePosition;
        point.z = MainCamera.farClipPlane;
        point = MainCamera.ScreenToWorldPoint(point);
        return (Vector2)point;
    }

    // 처음 시작 시 멀리서 날라오는 느낌
    private Sequence setEffectful_ComeinScreen()
    {
        for (int i = 0; i < MoveEffectRTList.Count; i++)
        {
            Vector3 RTPos = MoveEffectRTList[i].anchoredPosition3D;
            RTPos.z = 5000;
            MoveEffectRTList[i].anchoredPosition3D = RTPos;

            if (MoveEffectRTList[i].TryGetComponent(out CanvasGroup CG))
            { CG.DOFade(0f, 0f); }
            else if (MoveEffectRTList[i].TryGetComponent(out Image img))
            { img.DOFade(0f, 0f); }
        }

        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < MoveEffectRTList.Count; i++)
        {
            Vector3 RTPos = MoveEffectRTList[i].anchoredPosition3D;
            RTPos.z = 0;
            seq.Insert(i * 0.1f, MoveEffectRTList[i].DOAnchorPos3D(RTPos, 0.3f));

            if (MoveEffectRTList[i].TryGetComponent(out CanvasGroup CG))
            { seq.Join(CG.DOFade(1f, 0.3f)); }
            else if (MoveEffectRTList[i].TryGetComponent(out Image img))
            { seq.Join(img.DOFade(1f, 0.3f)); }
        }

        return seq;
    }

    // 좌표값에 따라 RT Rotation 값 자체 변경
    private void setEffectful_RotateByPointer(Vector2 MousePos)
    {
        for (int i = 0; i < MoveEffectRTList.Count; i++)
        {
            Vector3 mousePosForRot = new Vector3(MousePos.y * 0.1f, -MousePos.x * 0.1f, 0);
            Vector3 totalRot = OffsetRotationList[i] + mousePosForRot;
            MoveEffectRTList[i].rotation = Quaternion.Euler(totalRot);
        }

        
    }

    // 타이틀 로고 움직임
    private void SetEfectful_TitleLogo()
    {
        Sequence Seq = DOTween.Sequence();
        Seq.Append(TitleLogoRT.DOScale(Vector3.one * 1.1f, 1f).SetEase(Ease.InOutCubic));
        Seq.Append(TitleLogoRT.DOScale(Vector3.one, 1f).SetEase(Ease.InOutCubic));
        Seq.SetLoops(-1, LoopType.Restart);
    }

    #endregion
}
