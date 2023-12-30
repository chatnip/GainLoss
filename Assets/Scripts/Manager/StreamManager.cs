using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StreamManager : Manager<StreamManager>
{
    [SerializeField] GameManager GameManager;
    [SerializeField] DialogManager DialogManager;
    [SerializeField] DataManager DataManager;

    [HideInInspector] public string currentStreamEventID;
    [HideInInspector] public StreamEvent currentStreamEvent;





    public void StartDialog(string id)
    {
        DialogManager.ScenarioBase.Value = InitStreamEventID(id);
    }

    public ScenarioBase InitStreamEventID(string id) //currentStreamEventID
    {
        ApplyGage();
        SaveCSV(id, "Assets/SaveCSV/", $"{DataManager.StreamEventsFileName}.csv");

        List<Fragment> fragments = new();
        List<KeyValuePair<string, object>> basicDatas = new();

        foreach (var data in DataManager.BasicDialogDatas[0]) //// 베이직 다이얼로그 순회
        {
            if (data.Key.Contains(id.Substring(4, 4)))
            {
                basicDatas.Add(data);
            }
        }

        // 방송 제목 삽입
        int rand = UnityEngine.Random.Range(1, 3);
        string addID = "T0" + rand.ToString();
        Debug.Log(addID);
        DialogManager.streamTitleText.text = (string)DataManager.TitleDatas[1][id.Substring(4, 4) + addID];

        foreach (var data in basicDatas) // 베이직 T 데이터 순회
        {
            if (data.Key.Contains(addID))
            {
                string key = data.Key; // 동일한 WA키가 담긴 모든 값 가져오기                    
                string animeId = (string)DataManager.BasicDialogDatas[0][key];
                string script = (string)DataManager.BasicDialogDatas[2][key]; // 언어 교체 추가해야함 
                Fragment fragment = new(animeId, script);
                fragments.Add(fragment);

            }
        }

        foreach (var data in DataManager.DialogDatas[0]) // 다이얼로그 순회
        {
            if (data.Key.Contains(id))
            {
                string key = data.Key;
                string animeId = (string)DataManager.DialogDatas[0][key];
                string script = (string)DataManager.DialogDatas[2][key]; // 언어 교체 추가해야함
                Fragment fragment = new(animeId, script);
                fragments.Add(fragment);
            }
        }
        ScenarioBase scenario = new(fragments);

        return scenario;
    }

    void ApplyGage()
    {
        DialogManager.currentStreamEvent = this.currentStreamEvent;

        GameManager.stressGage += currentStreamEvent.stressValue;
        GameManager.angerGage += currentStreamEvent.angerValue;
        GameManager.riskGage += currentStreamEvent.riskValue;
        GameManager.OverloadGage += currentStreamEvent.OverloadValue;

        if (GameManager.stressGage < 0) { GameManager.stressGage = 0; }
        if (GameManager.angerGage < 0) { GameManager.angerGage = 0; }
        if (GameManager.riskGage < 0) { GameManager.riskGage = 0; }
        if (GameManager.OverloadGage < 0) { GameManager.OverloadGage = 0; }
    }
    void SaveCSV(string id, string filePath, string fileName)
    {
        //CSV데이터 가져와 string값으로 저장
        string path = filePath + fileName;
        string s_sentenceSheetTemp = File.ReadAllText(path);
        string[] s_sentenceSheetCell = s_sentenceSheetTemp.Split(',', '\n');

        //각각의 줄을 저장
        List<string> s_id = new List<string>();
        List<string> s_isCreated = new List<string>();
        List<string> s_value = new List<string>();

        for (int i = 0; i < (s_sentenceSheetCell.Length - 1) / 3; i++)
        {
            s_id.Add(s_sentenceSheetCell[i]);
            s_isCreated.Add(s_sentenceSheetCell[i + (s_sentenceSheetCell.Length - 1) / 3]);
            s_value.Add(s_sentenceSheetCell[i + (s_sentenceSheetCell.Length - 1) * 2 / 3]);
        }
        int changeValueNum = 0;
        for (int i = 1; i < (s_sentenceSheetCell.Length - 1) / 3; i++)
        {
            if (s_id[i] == id) { changeValueNum = i; }
        }
        s_isCreated[changeValueNum] = "TRUE";
        int temp = Convert.ToInt32(s_value[changeValueNum]) + 1;
        s_value[changeValueNum] = temp.ToString();
        s_sentenceSheetTemp = "";

        InputCSV(s_sentenceSheetCell, s_id);
        InputCSV(s_sentenceSheetCell, s_isCreated);
        InputCSV(s_sentenceSheetCell, s_value);

        void InputCSV(string[] cell, List<string> value)
        {
            for (int i = 0; i < (cell.Length - 1) / 3; i++)
            {
                s_sentenceSheetTemp += value[i];
                if (i == ((s_sentenceSheetCell.Length - 1) / 3) - 1)
                { s_sentenceSheetTemp += "\n"; }
                else
                { s_sentenceSheetTemp += ","; }
            }
        }
        //Debug.Log(s_sentenceSheetTemp);

        DataManager.SetStreamEventDatas_reload(s_sentenceSheetTemp);


        /*StringBuilder sb = new StringBuilder();
        sb.Append(s_sentenceSheetTemp);
        StreamWriter outStream = File.CreateText(path);
        outStream.Write(sb);
        outStream.Close();
        */

        File.WriteAllText(path, s_sentenceSheetTemp);
    
    }
}
[System.Serializable]
public class StreamEvent
{
    [HideInInspector] public int numberOfUses;
    [HideInInspector] public int stressValue;
    [HideInInspector] public int angerValue;
    [HideInInspector] public int riskValue;
    [HideInInspector] public int OverloadValue;
}

