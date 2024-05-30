//Refactoring v1.0
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UniRx;
using System.Collections.Generic;
using System;
using UniRx.Triggers;
using System.Linq;

public class GameSystem : Singleton<GameSystem>
{
    #region Value

    [Header("=== Main UI")]
    [SerializeField] Button pauseBtn;
    [SerializeField] List<TMP_Text> abilityTxts;

    [Header("=== Obj Panel")]
    [SerializeField] public Button objPanelBtn;
    [SerializeField] TMP_Text objName;
    [SerializeField] TMP_Text objText;
    [SerializeField] Image objImg;
    [SerializeField] Animator anotherAnimator;
    [SerializeField] float textingSpeed = 20f;

    [Header("=== Check Things")]
    [SerializeField] List<string> objWriteTexts = new List<string>();
    [SerializeField] List<string> objNameTexts = new List<string>();
    [SerializeField] List<Sprite> objSprites = new List<Sprite>();
    [SerializeField] List<string> objAnimNames = new List<string>();

    bool IsNpc;
    InteractObject currentIO;

    Tween objTextingTween;
    int objWriteTexts_currentOrder = 0;

    

    List<IDisposable> iDisposables = new List<IDisposable>();

    [Header("=== Character Sprite")]
    [SerializeField] public List<SpriteModule> spriteModules;

    [Header("=== Choice UI")]
    [SerializeField] CanvasGroup chioceCG;
    [SerializeField] RectTransform btnsRT;
    [SerializeField] List<IDBtn> choiceBtnList = new List<IDBtn>();
    [SerializeField] TMP_Text needAbilityTxt;
    [SerializeField] Sprite btnSprite;

    [Header("=== Cutscene")]
    [SerializeField] public Image cutsceneImg;
    [SerializeField] public TMP_Text cutsceneTxt;

    [Header("=== Player")]
    public Transform playerPos;

    // Other Value
    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        // Player
        playerPos.position = new Vector3(0f, 0f, 0f);
        playerPos.rotation = Quaternion.identity;

        // Text
        SetAbilityUI();
        if (needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
        { CG.alpha = 0f; }
        List<TMP_Text> systemTmpT = new List<TMP_Text> { objName, objText, needAbilityTxt, cutsceneTxt };
        LanguageManager.Instance.SetLanguageTxts(systemTmpT);


        // Choice
        chioceCG.gameObject.SetActive(false);
        chioceCG.alpha = 0f;

        // Btns
        pauseBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (!GameManager.Instance.canSkipTalking) { return; }

                PlayerInputController.Instance.OnOffPause();
            });
        objPanelBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                if (!GameManager.Instance.canSkipTalking) { return; }

                ObjDescSkip();
            });

        // Sprites
        string Path = "talkingCharacter/";
        foreach (SpriteModule SM in spriteModules)
        {
            SM.nameID = SM.texture.name;
            SM.sprites = Resources.LoadAll<Sprite>(Path + SM.texture.name);
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && cutsceneImg.gameObject.activeSelf)
        {
            cutsceneSO.skipOrCompleteSeq(cutsceneImg, cutsceneTxt);
            return;
        }
    }


    #endregion

    #region Obj Panel

    private Tween SetWrite(int i)
    {
        return objText.DOText(objWriteTexts[i], objWriteTexts[i].Length / textingSpeed)
                     .SetEase(Ease.Linear)
                     .OnStart(() =>
                     {
                         if (IsNpc)
                         {
                             objName.text = objNameTexts[i];
                             objImg.sprite = objSprites[i];
                             if (objAnimNames[i].Substring(0, 3) == "KAA")
                             {
                                 PlayerController.Instance._animator.Play(objAnimNames[i]);
                             }
                             else
                             {
                                 anotherAnimator.Play(objAnimNames[i]);
                             }
                         }
                     })
                     .OnComplete(() =>
                     {
                         objWriteTexts_currentOrder++;
                     });
    }

    public void ObjDescOn(InteractObject IO, bool isNpc, string answerID)
    {
        // Set Base Data
        IsNpc = isNpc;
        if(IsNpc && IO.TryGetComponent(out Animator animator)) 
        { anotherAnimator = animator; }
        currentIO = IO;
        GameManager.Instance.canInput = false;
        GameManager.Instance.canInteractObject = false;
        GameManager.Instance.canSkipTalking = true;
        PlayerInputController.Instance.StopMove();
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(false);

        // Set Text Data
        objText.text = "";
        objWriteTexts = null;
        objNameTexts = null;
        objSprites = new List<Sprite>();

        void SetData(string text, string name, string sprite, string anim)
        {
            // Text
            objWriteTexts = text.Split('/').ToList();
            if (IsNpc)
            {
                // Name
                objNameTexts = name.Split('/').ToList();

                // Sprite
                List<string> objSpriteIDs = sprite.Split('/').ToList();
                objSprites = GetCollectSprite(objSpriteIDs);

                // Anim
                objAnimNames = anim.Split("/").ToList();
            }
        }

        if (answerID != null) // 선택에 따른 답변
        {
            SetData(
                DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount + LanguageManager.Instance.languageNum][answerID].ToString(),
                DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + LanguageManager.Instance.languageNum][answerID].ToString(), 
                DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + 4][answerID].ToString(),
                DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + 5][answerID].ToString()
                );
        }
        else if (IO.IsInteracted) // 기본
        {
            SetData(
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount + LanguageManager.Instance.languageNum][IO.ID].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 2 + LanguageManager.Instance.languageNum][IO.ID].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 5 + 0][IO.ID].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 5 + 2][IO.ID].ToString()
                );
        }
        else // 상호작용 처음 시 (선택지 주어지는 상호작용)
        {
            SetData(
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + LanguageManager.Instance.languageNum][IO.ID].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 4 + LanguageManager.Instance.languageNum][IO.ID].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 5 + 1][IO.ID].ToString(),
                DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageTypeAmount * 5 + 3][IO.ID].ToString()
                );
        }


        // Check Npc
        objImg.gameObject.SetActive(IsNpc);
        objName.gameObject.SetActive(IsNpc);
        if (IsNpc) { if (objText.TryGetComponent(out RectTransform rt)) { rt.sizeDelta = new Vector2(1100f, 180f); } }
        else { if (objText.TryGetComponent(out RectTransform rt)) { rt.sizeDelta = new Vector2(1400f, 250f); } }

        // Set Writting
        objPanelBtn.gameObject.SetActive(true);
        objWriteTexts_currentOrder = 0;
        objTextingTween = SetWrite(objWriteTexts_currentOrder);

        

    }

    public void ObjDescSkip()
    {
        // 글인 라이팅 중이면, 바로 완료
        if (DOTween.IsTweening(objText))
        {
            Debug.Log("라이팅 중");
            DOTween.Complete(objText);
        }
        else
        {
            // 마지막 라이팅까지 완료가 안되어있다면, 새로운 라이팅 시작
            if (objWriteTexts.Count > objWriteTexts_currentOrder)
            {
                Debug.Log("넘기기");
                objText.text = "";
                SetWrite(objWriteTexts_currentOrder);
            }
            // 마지막 라이팅까지 완료가 되어있다면
            else
            {
                // 선택지를 주는 라이팅 이었다면
                if (!currentIO.IsInteracted)
                {
                    Debug.Log("선택지");
                    ShowChioceWindow(0.25f);
                }
                else // 아니라면(기본 라이팅)
                {
                    Debug.Log("종료");
                    ObjDescOff();
                }
            }
        }
    }

    public void ObjDescOff()
    {
        currentIO.IsInteracted = true;

        GameManager.Instance.canInput = true;
        GameManager.Instance.canInteractObject = true;
        PlayerInputController.Instance.CanMove = true;
        PlayerController.Instance._animator.SetTrigger("Return");
        ObjectInteractionButtonGenerator.Instance.SetOnOffAllBtns(true);

        DOTween.Complete(objTextingTween);
        objPanelBtn.gameObject.SetActive(false);
        objWriteTexts_currentOrder = 0;
        objText.text = null;
        currentIO = null;
    }

    #endregion

    #region Choice

    private void ShowChioceWindow(float time)
    {
        // time Pause
        Time.timeScale = 0f;

        // Set On
        chioceCG.gameObject.SetActive(true);
        chioceCG.DOFade(1f, time)
            .OnStart(() => { GameManager.Instance.canSkipTalking = false; })
            .OnComplete(() => { GameManager.Instance.canSkipTalking = true; })
            .SetUpdate(true);

        // Set Btn By ID
        List<string> IDs = new List<string>();
        foreach (KeyValuePair<string, object> dictDatas in DataManager.Instance.ObjectChoiceCSVDatas[0])
        {
            if (dictDatas.Key != "ID" && dictDatas.Key.Substring(0, 4) == currentIO.ID)
            { IDs.Add(dictDatas.Key); }
        }

        // Set UI
        for (int i = 0; i < IDs.Count; i++)
        {
            IDBtn Choice_IDBtn = ObjectPooling.Instance.GetIDBtn();
            Choice_IDBtn.buttonType = ButtonType.ChoiceType;
            Choice_IDBtn.transform.SetParent(btnsRT);
            Choice_IDBtn.buttonID = IDs[i];
            Choice_IDBtn.basicImage = btnSprite;
            Choice_IDBtn.sizeDelta = new Vector2(1000f, 100f);
            LanguageManager.Instance.SetLanguageTxt(Choice_IDBtn.buttonText);

            Choice_IDBtn.anchorPos = new Vector3(0f, i * -150f, 0f);
            if (IDs.Count > 1) { Choice_IDBtn.anchorPos += Vector3.up * 75f * (IDs.Count - 1); }

            Choice_IDBtn.gameObject.SetActive(true);
            choiceBtnList.Add(Choice_IDBtn);
        }

        // Set Btn Subscribe
        foreach (IDBtn idBtn in choiceBtnList)
        {
            // Pointer Enter
            IDisposable iDisEnter = idBtn.button.OnPointerEnterAsObservable()
                .Subscribe(_ =>
                {
                    string _id = idBtn.buttonID;
                    if (_id.Substring(4, 3) == "C99") { return; }
                    needAbilityTxt.transform.parent.gameObject.SetActive(true);
                    if (needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
                    {
                        DOTween.Kill(CG);
                        CG.DOFade(1f, 0.25f).SetUpdate(true);
                    }
                    string Anno = "";
                    for (int i = 0; i < DataManager.Instance.AbilityCSVDatas.Count; i++)
                    {
                        int ability = Convert.ToInt32(DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + i][_id]);
                        if (ability > 0)
                        {
                            Anno += " " + DataManager.Instance.AbilityCSVDatas[LanguageManager.Instance.languageNum]["A0" + i.ToString()].ToString() + " " + ability + " ";
                        }
                        
                    }
                    needAbilityTxt.text = Anno;
                    needAbilityTxt.TryGetComponent(out RectTransform rt);
                    if (DOTween.IsTweening(rt)) { DOTween.Kill(rt); }
                    if (DOTween.IsTweening(needAbilityTxt)) { DOTween.Kill(needAbilityTxt); }
                    rt.localScale = Vector3.one;
                    needAbilityTxt.color = Color.white;
                });
            // Pointer Exit
            IDisposable iDisExit = idBtn.button.OnPointerExitAsObservable()
                .Subscribe(_ =>
                {
                    if (needAbilityTxt.transform.parent.TryGetComponent(out CanvasGroup CG))
                    {
                        DOTween.Kill(CG);
                        CG.DOFade(0f, 0.25f)
                            .OnComplete(() => { needAbilityTxt.transform.parent.gameObject.SetActive(false); })
                            .SetUpdate(true);
                    }
                });
            //Pointer Click 
            IDisposable iDisClick = idBtn.button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    // 능력치 충분한지 체크
                    string _id = idBtn.buttonID;
                    int need_obse = Convert.ToInt32(DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + 0][_id]);
                    int need_pers = Convert.ToInt32(DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + 1][_id]);
                    int need_ment = Convert.ToInt32(DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + 2][_id]);

                    if (GameManager.Instance.mainInfo.IsEnoughAbility(need_obse, need_pers, need_ment))
                    {
                        currentIO.IsInteracted = true;
                        chioceCG.DOFade(0f, time)
                            .OnStart(() => 
                            {
                                GameManager.Instance.canSkipTalking = false;
                                Time.timeScale = 1f;
                            })
                            .OnComplete(() =>
                            {
                                GameManager.Instance.canSkipTalking = true;
                                ChoiceTab(_id);
                            })
                            .SetUpdate(true);
                    }
                    else
                    {
                        Debug.Log("불충분");
                        needAbilityTxt.TryGetComponent(out RectTransform rt);
                        rt.DOScale(1.2f, 0.2f).SetUpdate(true);
                        needAbilityTxt.DOColor(Color.red, 0.2f).SetUpdate(true);
                    }
                });
            iDisposables.Add(iDisEnter); iDisposables.Add(iDisExit); iDisposables.Add(iDisClick);
        }

    }

    private void ChoiceTab(string _id)
    {
        chioceCG.gameObject.SetActive(false);

        // set Inevitable
        PlaceManager.Instance.Exclude_InevitableIO(currentIO);

        // Sub Cancel
        foreach (IDisposable iDis in iDisposables) { iDis.Dispose(); }

        // Get Reasoning Contents
        GameManager.Instance.mainInfo.ReasoningContentsID.Add(
            DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + 3][_id].ToString());

        // Get Stream Quarter
        DialogManager.Instance.streamQuarterID.Add(
            DataManager.Instance.ObjectChoiceCSVDatas[LanguageManager.Instance.languageTypeAmount * 3 + 6][_id].ToString());

        // Obj Description Play
        if (_id.Substring(0, 1) == "O")
        {
            ObjDescOn(currentIO, false, _id);
        }
        else if (_id.Substring(0, 1) == "N")
        {
            ObjDescOn(currentIO, true, _id);
        }


        // Object Pooling
        foreach (IDBtn idBtn in choiceBtnList)
        { ObjectPooling.Instance.GetBackIDBtn(idBtn); }
        choiceBtnList.Clear();
    }

    #endregion

    #region SetAbilityUI

    public void SetAbilityUI()
    {
        abilityTxts[0].text = GameManager.Instance.mainInfo.ObservationalAbility.ToString();
        abilityTxts[1].text = GameManager.Instance.mainInfo.PersuasiveAbility.ToString();
        abilityTxts[2].text = GameManager.Instance.mainInfo.MentalStrengthAbility.ToString();
    }

    #endregion

    #region Sprite

    private List<Sprite> GetAllCharacterSprite()
    {
        List<Sprite> allSprite = new List<Sprite>();
        foreach(SpriteModule SM in spriteModules)
        {
            foreach(Sprite sprite in SM.sprites)
            {
                allSprite.Add(sprite);
            }
        }
        return allSprite;
    }
    private List<Sprite> GetCollectSprite(List<string> spriteIDs)
    {
        List<Sprite> collects = new List<Sprite>();
        foreach (string spriteID in spriteIDs)
        {
            foreach (Sprite sprite in GetAllCharacterSprite())
            {
                if (spriteID == sprite.name)
                {
                    collects.Add(sprite);
                }
            }
        }
        return collects;
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