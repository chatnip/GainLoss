using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TodoSpawner : IDBtnSpawner, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] Desktop Desktop;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] WordManager WordManager;
    [SerializeField] StreamManager StreamManager;
    [SerializeField] DialogManager DialogManager;

    [Header("*WordParentObj")]
    [SerializeField] RectTransform wordParentObject;
    [SerializeField] RectTransform wordActionParentObject;

    #endregion

    #region Main
    protected override void OnEnable()
    {
        base.OnEnable();
        SetThisSectionBtns(wordParentObject);


    }
    protected override void OnDisable()
    {
        base.OnDisable();
        PickWordActionBtn();
    }

    private void SetThisSectionBtns(RectTransform parentRT)
    {
        Button[] allChildren = parentRT.GetComponentsInChildren<Button>();
        List<List<Button>> allBtnsList = new List<List<Button>>();
        for(int i = 0; i < allChildren.Length; i++)
        {
            allBtnsList.Add(new List<Button> { allChildren[i] });
        }
        List<List<Button>> btns = new List<List<Button>>(allBtnsList);
        PlayerInputController.SetSectionBtns(btns, this);
    }

    public void Interact()
    {
        if (PlayerInputController.SelectBtn.TryGetComponent(out IDBtn iDBtn))
        {
            if(iDBtn.buttonType == ButtonType.WordType)
            {
                WordManager.WordBtnApply(iDBtn.buttonValue);
                SetThisSectionBtns(wordActionParentObject);

                return;
            }
            else if (iDBtn.buttonType == ButtonType.WordActionType)
            {
                WordManager.WordActionBtnApply(iDBtn.buttonValue);
                PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { WordManager.resetBtn, Desktop.streamStartBtn } }, this);

                return;
            }
        }
        else if(PlayerInputController.SelectBtn == WordManager.resetBtn)
        {
            WordManager.TodoReset();
            WordManager.resetBtn.TryGetComponent(out UnityEngine.UI.Outline outilne);
            outilne.enabled = false;
            SetThisSectionBtns(wordParentObject);
        }
        else if (PlayerInputController.SelectBtn == Desktop.streamStartBtn)
        {
            Desktop.streamStartBtn.TryGetComponent(out UnityEngine.UI.Outline outilne);
            outilne.enabled = false;
            Desktop.StartStream();
            PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { DialogManager.dialogNextBtn } }, DialogManager);
        }

    }

    #endregion

    #region Create Btns

    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = true;
        wordBtn.buttonValue = word;
        //wordBtn.Rate = ;
        return wordBtn;
    }

    protected override void SpawnIDBtn()
    {
        SpawnWordBtn();
    }

    #region AIL Btns

    private void SpawnWordBtn()
    {
        WordManager.enableWordBtnList.Clear();
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            IDBtn wordBtn = CreateIDBtn(WordManager.currentWordList[i]); // 생성
            wordBtn.transform.SetParent(wordParentObject); // 부모 설정
            wordBtn.AddVisiableWordRate((string)DataManager.WordDatas[1][WordManager.currentWordList[i].ID]); // 등급 표시
            bool checkCanUseMalicious = CheckCanUseMalicious(
                (string)DataManager.WordDatas[1][WordManager.currentWordList[i].ID],
                WordManager.currentWordIDList[i]); // Malicious계급 ail파일을 사용했는지 판단
            if (!checkCanUseMalicious)
            { wordBtn.CannotUse(false, "Use up to the maximum(1)"); }
            else
            { wordBtn.CannotUse(true, ""); }
            wordBtn.buttonType = ButtonType.WordType; // 정렬
            WordManager.enableWordBtnList.Add(wordBtn); // 활성화 리스트에 삽입
            // WordManager.WordBtnListSet(); // 데이터 삽입
            wordBtn.gameObject.SetActive(true); // 활성화
        }
        WordManager.WordBtnListSet(); // 데이터 삽입
    }
    private bool CheckCanUseMalicious(string rate, string wordID)
    {
        if (rate == "Malicious")
        {
            List<string> streamEventIDs = new List<string>();
            for (int i = 0; i < (DataManager.WordActionDatas[0].Count - 1); i++)
            {
                if (i.ToString().Length == 1) { streamEventIDs.Add(wordID + "WA0" + (i + 1)); }
                else if (i.ToString().Length == 2) { streamEventIDs.Add(wordID + "WA" + (i + 1)); }
            }

            foreach (string s in streamEventIDs)
            {
                if (Convert.ToBoolean(StreamManager.currentStreamEventDatas[0][s]))
                { return false; }
            }
            return true;
        }
        else
        { return true; }
    }

    #endregion

    #region EXE Btns

    public void SpawnWordActionBtn()
    {
        PickWordActionBtn(); // 버튼 초기화
        WordManager.enableWordActionBtnList.Clear();

        for (int i = 0; i < WordManager.currentWordActionList.Count; i++)
        {
            IDBtn actionBtn = CreateIDBtn(WordManager.currentWordActionList[i]); // 생성
            actionBtn.transform.SetParent(wordActionParentObject); // 부모 설정
            if (Convert.ToInt32(StreamManager.currentStreamEventDatas[1][WordManager.currentWord.ID + WordManager.currentWordActionList[i].ID]) >= 3)
            { actionBtn.CannotUse(false, "Use up to the maximum(3)"); }
            else 
            { actionBtn.CannotUse(true, ""); }
            actionBtn.AddVisiableWordRate((string)DataManager.WordActionDatas[1][WordManager.currentWordActionList[i].ID]); // 등급 표시
            actionBtn.buttonType = ButtonType.WordActionType; // 정렬
            WordManager.enableWordActionBtnList.Add(actionBtn); // 활성화 리스트에 삽입
            // WordManager.WordActionBtnListSet(); // 데이터 삽입
            actionBtn.gameObject.SetActive(true); // 활성화

        }
        WordManager.WordActionBtnListSet(); // 데이터 삽입
    }

    #endregion

    #region Pick

    protected override void PickIDBtn()
    {
        PickWordBtn();
    }

    public void PickWordBtn()
    {
        if (WordManager.enableWordBtnList.Count != 0)
        {
            for (int i = WordManager.enableWordBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.WordObjectPick(WordManager.enableWordBtnList[i]);
            }
            WordManager.enableWordBtnList.Clear();
        }
    }

    public void PickWordActionBtn()
    {
        if (WordManager.enableWordActionBtnList.Count != 0)
        {
            for (int i = WordManager.enableWordActionBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.WordObjectPick(WordManager.enableWordActionBtnList[i]);
            }
            WordManager.enableWordActionBtnList.Clear();
        }
    }

    #endregion


    #endregion
}
