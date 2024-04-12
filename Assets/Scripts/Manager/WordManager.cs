using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UnityEditor;

public class WordManager : Manager<WordManager>
{
    #region Value

    [Header("*Property")]
    [SerializeField] StreamManager StreamManager;
    [SerializeField] ObjectPooling ObjectPooling;
    [SerializeField] DialogManager DialogManager;
    [SerializeField] Desktop Desktop;
    [SerializeField] PlayerInputController PlayerInputController;

    [Space(10)]
    [SerializeField] TodoSpawner todoWordBtnSpawner;
     
    [Header("*View")]
    [SerializeField] TMP_Text viewWordAction;
    [SerializeField] public Button resetBtn;
    [SerializeField] TMP_Text ResultPreview_NumberOfUsed;
    [SerializeField] TMP_Text ResultPreview_Gage;
    StringReactiveProperty currentWordActionStr = new();
    [HideInInspector] public StreamEvent currentStreamEvent = new StreamEvent();


    // 켜진 단어 및 단어의 액션 버튼 목록
    [SerializeField] public List<IDBtn> enableWordBtnList = new();
    [SerializeField] public List<IDBtn> enableWordActionBtnList = new();

    // 선택한 단어 및 단어의 액션 목록
    [HideInInspector] public List<string> currentWordIDList = new();
    [HideInInspector] public List<ButtonValue> currentWordList = new();
    [HideInInspector] public List<string> currentWordActionIDList = new();
    [HideInInspector] public List<ButtonValue> currentWordActionList = new();

    // 선택한 단어 및 단어의 액션
    [HideInInspector] public ButtonValue currentWord;
    private ButtonValue currentWordAction;

    #endregion

    #region Main

    private void Start()
    {
        TodoReset();
        InitWord();

        currentWordActionStr
            .Subscribe(x =>
            {
                viewWordAction.text = x;
            });

        resetBtn
            .OnClickAsObservable()
            .Subscribe(x =>
            {
                TodoReset();
                todoWordBtnSpawner.SetThisSectionBtns(todoWordBtnSpawner.wordParentObject);
                todoWordBtnSpawner.setOnLine();
            });
    }
    public void TodoReset()
    {
        Desktop.CanUseThisSentence = true;
        resetBtn.TryGetComponent(out Outline OL);
        OL.enabled = false;
        todoWordBtnSpawner.PickWordActionBtn();
        currentWord = null;
        currentWordAction = null;
        // currentWordIDList.Clear();  // 목록을 지우도록 수정
        currentWordList.Clear();
        // currentWordActionIDList.Clear();
        currentWordActionList.Clear();
        currentWordActionStr.Value = "아무것도 하지 않는다";
        todoWordBtnSpawner.SetThisSectionBtns(todoWordBtnSpawner.wordParentObject);

        ResultPreview_NumberOfUsed.text = "";
        ResultPreview_Gage.text = "";
    }

    #endregion

    #region ButtonListSetting

    #region AIL

    public void WordBtnListSet()
    {
        ResultPreview_NumberOfUsed.text = "";
        ResultPreview_Gage.text = "";

        Debug.Log(enableWordBtnList.Count);
        foreach (IDBtn wordBtn in enableWordBtnList)
        {
            Debug.Log(wordBtn.buttonValue.Name);
            wordBtn.button
                .OnClickAsObservable()
                .Select(word => wordBtn.buttonValue)
                .RepeatUntilDisable(wordBtn)
                .Subscribe(word =>
                {
                    if (wordBtn.CannotUseLabal.gameObject.activeSelf) { Desktop.CanUseThisSentence = false; }

                    WordBtnApply(word);

                    todoWordBtnSpawner.SetThisSectionBtns(todoWordBtnSpawner.wordActionParentObject);
                    todoWordBtnSpawner.setOnLine();

                });
        }
    }
    public void WordBtnApply(ButtonValue BV)
    {
        currentWord = BV;
        Debug.Log("currentWord : " + currentWord.Name);
        InitWordAction(currentWord.ID);
        todoWordBtnSpawner.SpawnWordActionBtn();
    }

    #endregion

    #region EXE

    public void WordActionBtnListSet()
    {
        Debug.Log(enableWordActionBtnList.Count);
        foreach (IDBtn wordActionBtn in enableWordActionBtnList)
        {
            Debug.Log(wordActionBtn.buttonValue.Name);
            wordActionBtn.button
                .OnClickAsObservable()
                .Select(action => wordActionBtn.buttonValue)
                .RepeatUntilDisable(wordActionBtn)
                .Subscribe(action =>
                {
                    if (wordActionBtn.CannotUseLabal.gameObject.activeSelf) { Desktop.CanUseThisSentence = false; }

                    WordActionBtnApply(action);

                    PlayerInputController.SetSectionBtns(new List<List<Button>> { new List<Button> { resetBtn, Desktop.streamStartBtn } }, todoWordBtnSpawner);
                    todoWordBtnSpawner.setOnLine();

                });
        }
    }
    public void WordActionBtnApply(ButtonValue BV)
    {
        currentWordAction = BV;
        Debug.Log("currentWordAction : " + currentWordAction.Name);
        string text = string.Format("<size=150%><#D40047><b>{0}</b></color></size>에 대한 <size=150%><#D40047><b>{1}</b></color></size>을(를) 한다.", currentWord.Name, currentWordAction.Name);
        currentWordActionStr.Value = text;
        StreamManager.currentStreamEventID = currentWord.ID + currentWordAction.ID;

        currentStreamEvent = SetStreamEvent(
            currentWord.ID,
            currentWordAction.ID,
            Convert.ToBoolean(StreamManager.currentStreamEventDatas[0][StreamManager.currentStreamEventID]),
            Convert.ToInt32(StreamManager.currentStreamEventDatas[1][StreamManager.currentStreamEventID]));

        SetResultPreview(
            currentStreamEvent,
            Convert.ToBoolean(StreamManager.currentStreamEventDatas[0][StreamManager.currentStreamEventID]),
            Convert.ToInt32(StreamManager.currentStreamEventDatas[1][StreamManager.currentStreamEventID]));
    }

    #endregion

    #region Set

    private StreamEvent SetStreamEvent(string wordID, string wordActionID, bool isCreated, int NumberOfUsed)
    {
        StreamEvent streamEvent = new StreamEvent();
        string wordRate = DataManager.WordDatas[1][wordID].ToString();
        string wordActionRate = DataManager.WordActionDatas[1][wordActionID].ToString();

        streamEvent.stressValue = SetGage((int)DataManager.WordDatas[2][wordID],isCreated, NumberOfUsed, wordRate, wordActionRate);
        streamEvent.angerValue = SetGage((int)DataManager.WordDatas[3][wordID],isCreated, NumberOfUsed, wordRate, wordActionRate);
        streamEvent.riskValue = SetGage((int)DataManager.WordDatas[4][wordID],isCreated, NumberOfUsed, wordRate, wordActionRate);
        streamEvent.OverloadValue = SetOverloadGage((int)DataManager.WordActionDatas[2][wordActionID], isCreated, NumberOfUsed);
        if (streamEvent.OverloadValue < 1) { streamEvent.OverloadValue = 1; }
        Debug.Log(streamEvent.stressValue + " / " +
            streamEvent.angerValue + " / " +
            streamEvent.riskValue + " / " +
            streamEvent.OverloadValue);
        return streamEvent;
    }

    private int SetGage(int gage, bool isCreated, int NumberOfUsed, string wordRate, string wordActionRate) // 수치 조절 함수
    {
        int finalGage = gage;

        if (wordRate == "Normal")
        {
            if (wordActionRate == "Normal") 
            { 
                //finalGage += 0;
                if (isCreated) { finalGage -= 2; }
            }
            else if (wordActionRate == "Malicious") 
            {
                finalGage += 2;
                if (isCreated) { finalGage -= 1; }
            }
            
        }
        else if (wordRate == "Malicious")
        {
            if (wordActionRate == "Normal") 
            { 
                finalGage += 2;
                if (isCreated) { finalGage -= 1; }
            }
            else if (wordActionRate == "Malicious") 
            { 
                finalGage += 4;
                //if (isCreated) { finalGage -= 0; }
            }
        }

        return finalGage;
    }
    private int SetOverloadGage(int gage, bool isCreated, int NumberOfUsed) // (과부하) 수치 조절 함수
    {
        int finalGage = gage;
        if(isCreated)
        {
            finalGage = 0;
        }

        return finalGage;
    }
    private void SetResultPreview(StreamEvent streamEvent, bool isCreated, int NumberOfUsed)
    {
        if (!Desktop.CanUseThisSentence)
        {
            Desktop.streamStartBtn.interactable = false;
            ResultPreview_NumberOfUsed.text = "사용 불가";
            ResultPreview_Gage.text = "";
            return;
        }

        Desktop.streamStartBtn.interactable = true;
        string[] info = new string[2];
        if(isCreated)
        {
            info[0] = String.Format($"사용량 [ <#320C0C>{NumberOfUsed} / 3</color> ]");
            info[1] = "";
            caculGage(streamEvent.OverloadValue, "Overload");
            caculGage(streamEvent.stressValue, "Stress");
            caculGage(streamEvent.angerValue, "Anger");
            caculGage(streamEvent.riskValue, "Risk");

            void caculGage(int value, string gageType)
            {
                if (value != 0)
                {
                    if (value > 0) { info[1] += String.Format($"{gageType} [ <#7F0000><size=125%><b>+{value}</b></size></color> ]\n"); }
                    else { info[1] += String.Format($"{gageType} [ <#00007F><size=125%><b>{value}</b></size></color> ]\n"); }
                }
            }
        }
        else
        {
            info[0] = "<#320C0C>최초 시도</color>";
            info[1] = String.Format($"Overload [ <#7F0000><size=125%><b>+{streamEvent.OverloadValue}</b></size></color> ]");
        }

        ResultPreview_NumberOfUsed.text = info[0];
        ResultPreview_Gage.text = info[1];
    }

#endregion

#endregion

    #region Init

    // JsonLoadTest() == InitWordID()
    // 나중에 하루 지날때마다 실행되도록 해야함

    public void InitWord()
    {
        currentWordList.Clear(); // 초기화
        foreach (string id in currentWordIDList) // ID 순회
        {
            ButtonValue word = new(id, (string)DataManager.WordDatas[5][id]);
            currentWordList.Add(word);
        }
    }
    public void InitWordAction(string wordID)
    {
        currentWordActionList.Clear(); // 초기화
        foreach (string WordActionID in currentWordActionIDList) // ID 순회
        {
            string wordRate = (string)DataManager.WordDatas[1][wordID];
            string wordActionRate = (string)DataManager.WordActionDatas[1][WordActionID];
            if (!(wordRate == "Positive" && wordActionRate == "Malicious"))
            {
                ButtonValue word = new(WordActionID, (string)DataManager.WordActionDatas[3][WordActionID]);
                currentWordActionList.Add(word);
            }
        }
    }

    #endregion
}


[System.Serializable]
public class ButtonValue
{
    public string ID;
    public string Name;

    public ButtonValue(string id, string name)
    {
        ID = id;
        Name = name;
    }
}