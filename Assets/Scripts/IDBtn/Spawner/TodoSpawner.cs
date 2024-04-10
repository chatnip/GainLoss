using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public RectTransform wordParentObject;
    [SerializeField] public RectTransform wordActionParentObject;
    [SerializeField] public List<Button> finalBtns;

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

    public void SetThisSectionBtns(RectTransform parentRT)
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

    // �� AIL ���ϰ� ���� ����� �� �ִ� EXE �Ǻ� �� ��ȯ
    private List<string> GetThisAILCanCombine(string AIL_ID)
    {
        List<string> AILCanCombineExe = new List<string>();

        foreach (string all_id in DataManager.DialogDatas[0].Keys)
        {
            if (all_id.Substring(0, 4) == AIL_ID)
            {
                AILCanCombineExe.Add(all_id.Substring(4, 4));
            }
        }
        AILCanCombineExe = AILCanCombineExe.Distinct().ToList();
        foreach (string id in AILCanCombineExe) { Debug.Log(id); }

        return AILCanCombineExe;
    }

    #region AIL Btns

    private void SpawnWordBtn()
    {
        Debug.Log("Spawn AIL Btn");
        WordManager.enableWordBtnList.Clear();
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            IDBtn wordBtn = CreateIDBtn(WordManager.currentWordList[i]); // ����
            wordBtn.transform.SetParent(wordParentObject); // �θ� ����
            wordBtn.AddVisiableWordRate((string)DataManager.WordDatas[1][WordManager.currentWordList[i].ID]); // ��� ǥ��
            bool checkCanUseMalicious = CheckCanUseMalicious(
                (string)DataManager.WordDatas[1][WordManager.currentWordList[i].ID],
                WordManager.currentWordIDList[i]); // Malicious��� ail������ ����ߴ��� �Ǵ�
            if (!checkCanUseMalicious)
            { wordBtn.CannotUse(false, "�ִ� ��� Ƚ��(1) �ʰ�"); }
            else
            { wordBtn.CannotUse(true, ""); }
            wordBtn.buttonType = ButtonType.WordType; // ����
            WordManager.enableWordBtnList.Add(wordBtn); // Ȱ��ȭ ����Ʈ�� ����
            // WordManager.WordBtnListSet(); // ������ ����
            wordBtn.gameObject.SetActive(true); // Ȱ��ȭ

            List<string> canCombineEXEs = GetThisAILCanCombine(wordBtn.buttonValue.ID);
            bool canMakeSentence = false;
            foreach (string canCombineEXE in canCombineEXEs)
            {
                foreach(string currentWordActionID in WordManager.currentWordActionIDList)
                {
                    if(canCombineEXE == currentWordActionID)
                    {
                        canMakeSentence = true;
                    }
                }
            }
            if(!canMakeSentence)
            {
                wordBtn.button.interactable = false;
                wordBtn.CannotUse(false, "�ڿ������� ���� ���� �Ұ�");
            }
        }
        WordManager.WordBtnListSet(); // ������ ����
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
        Debug.Log("Spawn EXE Btn");
        PickWordActionBtn(); // ��ư �ʱ�ȭ
        WordManager.enableWordActionBtnList.Clear();

        // �� AIL ���ϰ� ���� ����� �� �ִ� EXE
        List<string> canCombineEXE = GetThisAILCanCombine(WordManager.currentWord.ID);

        for (int i = 0; i < WordManager.currentWordActionList.Count; i++)
        {
            // �� AIL ���ϰ� ���� ����� �� �ִ� EXE �Ǻ�
            if (canCombineEXE.Contains(WordManager.currentWordActionList[i].ID))
            {
                IDBtn actionBtn = CreateIDBtn(WordManager.currentWordActionList[i]); // ����
                actionBtn.transform.SetParent(wordActionParentObject); // �θ� ����
                if (Convert.ToInt32(StreamManager.currentStreamEventDatas[1][WordManager.currentWord.ID + WordManager.currentWordActionList[i].ID]) >= 3)
                { actionBtn.CannotUse(false, "�ִ� ��� Ƚ��(3) �ʰ�"); }
                else
                { actionBtn.CannotUse(true, ""); }
                actionBtn.AddVisiableWordRate((string)DataManager.WordActionDatas[1][WordManager.currentWordActionList[i].ID]); // ��� ǥ��
                actionBtn.buttonType = ButtonType.WordActionType; // ����
                WordManager.enableWordActionBtnList.Add(actionBtn); // Ȱ��ȭ ����Ʈ�� ����
                                                                    // WordManager.WordActionBtnListSet(); // ������ ����
                actionBtn.gameObject.SetActive(true); // Ȱ��ȭ
            }
        }
        WordManager.WordActionBtnListSet(); // ������ ����
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

    public void setOnLine()
    {
        // ��� ��ư
        List<Button> allBtns = new List<Button>();
        foreach (Transform Ob in wordParentObject)
        { Ob.TryGetComponent(out Button btn); allBtns.Add(btn); }
        foreach (Transform Ob in wordActionParentObject)
        { Ob.TryGetComponent(out Button btn); allBtns.Add(btn); }
        allBtns.AddRange(finalBtns);

        // Ȱ��ȭ ��ư ã��
        List<Button> OnBtnsList = new List<Button>();
        foreach(List<Button> OnBtns in PlayerInputController.SectionBtns) 
        { OnBtnsList.AddRange(OnBtns); }

        foreach (Button OffBtn in allBtns)
        {
            OffBtn.interactable = false;
        }
        foreach (Button OnBtn in OnBtnsList)
        {
            OnBtn.interactable = true;
        }
    }

    #endregion


    #endregion
}
