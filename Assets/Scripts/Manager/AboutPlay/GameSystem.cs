//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameSystem : Singleton<GameSystem>
{
    #region Value

    [Header("=== Main UI")]
    [SerializeField] Button pauseBtn;
    [SerializeField] List<TMP_Text> abilityTxts;

    [Header("=== Sprites")]
    [SerializeField] public List<SpriteModule> characterSprites;
    [SerializeField] public List<SpriteModule> objectSprites;
    [SerializeField] public List<SpriteModule> screenSprites;
    Dictionary<string, List<SpriteModule>> typeBySpriteDict = new Dictionary<string, List<SpriteModule>>();

    [Header("=== Cutscene")]
    [SerializeField] public Image cutsceneImg;
    [SerializeField] public TMP_Text cutsceneTxt;

    [Header("=== Player")]
    public Transform playerPos;

    [Header("== Epilogue")]
    [SerializeField] CanvasGroup epilogueCG;
    [SerializeField] RectTransform epilogueTxtRT;
    [SerializeField] TMP_Text epilogueTxt;
    [SerializeField] Button skipBtn;
    Sequence endSeq;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Player
        playerPos.position = new Vector3(0f, 0f, 0f);
        playerPos.rotation = Quaternion.identity;

        // Text
        SetAbilityUI();

        
        pauseBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canInput) { return; }

                StartCoroutine(PhoneHardware.Instance.Start_PhoneOn(PhoneHardware.e_phoneStateExtra.option));
            });

        skipBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (!GameManager.Instance.canSkipTalking) { return; }
                DOTween.Kill(endSeq);
                SceneManager.LoadScene("Title");
            });



        // Sprites
        string Path = "SpriteModule/";
        foreach (SpriteModule SM in characterSprites)
        {
            SM.nameID = SM.texture.name;
            SM.sprites = Resources.LoadAll<Sprite>(Path + SM.texture.name);
        }
        foreach (SpriteModule SM in objectSprites)
        {
            SM.nameID = SM.texture.name;
            SM.sprites = Resources.LoadAll<Sprite>(Path + SM.texture.name);
        }
        foreach (SpriteModule SM in screenSprites)
        {
            SM.nameID = SM.texture.name;
            SM.sprites = Resources.LoadAll<Sprite>(Path + SM.texture.name);
        }

        // Dict
        typeBySpriteDict = new Dictionary<string, List<SpriteModule>>
        {
            { "Character", characterSprites },
            { "Object", objectSprites },
            { "Screen", screenSprites }
        };


        Debug.Log("TestVer Epilogue");
        epilogueCG.alpha = 0f;
        epilogueCG.gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
    }


    #endregion

    #region SetAbilityUI

    public void SetAbilityUI()
    {
        abilityTxts[0].text = GameManager.Instance.mainInfo.observation.ToString();
        abilityTxts[1].text = GameManager.Instance.mainInfo.sociability.ToString();
        abilityTxts[2].text = GameManager.Instance.mainInfo.mentality.ToString();
    }

    #endregion

    #region Sprite

    private List<Sprite> GetAll_Illust(string type)
    {
        List<Sprite> allSprite = new List<Sprite>();
        foreach(SpriteModule SM in typeBySpriteDict[type])
        {
            foreach(Sprite sprite in SM.sprites)
            {
                allSprite.Add(sprite);
            }
        }
        return allSprite;
    }
    
    public Sprite Get_IllustToID(string type, string IllustID)
    {
        if (type == "" || IllustID == "") { return null; }

        List<Sprite> allIllust = GetAll_Illust(type);
        foreach(Sprite sprite in allIllust)
        {
            if(sprite.name == IllustID)
            {
                return sprite;
            }
        }
        return null;
    }



    #endregion

    #region T E S T ver

    public void ShowEpilogue()
    {
        GameManager.Instance.canInput = false;
        PlayerInputController.Instance.MoveStop();
        PlayerController.Instance.ResetAnime();
        GameManager.Instance.canInteractObject = false;

        endSeq = DOTween.Sequence();

        epilogueCG.gameObject.SetActive(true);

        endSeq.Append(epilogueCG.DOFade(1f, 3f)
            .OnComplete(() =>
            {
                GameManager.Instance.canInput = true;
            }));
        endSeq.Append(epilogueTxtRT.DOAnchorPosY(4120, 20f).SetEase(Ease.InOutSine));
        endSeq.Append(epilogueTxt.DOFade(0f, 3f));
        endSeq
            .OnComplete(() =>
            {
                SceneManager.LoadScene("Title");
            });
    }

    #endregion

}

[System.Serializable]
public class SpriteModule
{
    public string nameID;
    public Texture2D texture;
    public Sprite[] sprites;
}
