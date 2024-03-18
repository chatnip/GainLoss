using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamManager : Manager<StreamManager>
{
    [SerializeField] GameManager GameManager;
    [SerializeField] DialogManager DialogManager;
    [SerializeField] DataManager DataManager;
    [SerializeField] ScheduleManager ScheduleManager;

    [HideInInspector] public string currentStreamEventID;
    [HideInInspector] public StreamEvent currentStreamEvent;

    [HideInInspector] public List<Dictionary<string, object>> currentStreamEventDatas = new();

    public void StartDialog(string id)
    {
        DialogManager.ScenarioBase.Value = InitStreamEventID(id);

    }

    public ScenarioBase InitStreamEventID(string id) //currentStreamEventID
    {
        ApplyGage();
        if (GameManager.currentMainInfo.overloadGage >= 30 && ScheduleManager.currentHaveScheduleID.Contains("S01")) // ������ ������ ���� ��ġ �̻� �� �������� �߰�
        { ScheduleManager.currentHaveScheduleID.Add("S01"); Debug.Log("���� ���� ȹ��!"); }

        SetNumberOfUses(id);


        //CSVWriter.SaveCSV("Assets/Resources/Sheet/", "SentenceSheet.csv", "Assets/Resources/Sheet/SaveDatas/", "SentenceSave.txt");
        //DataManager.StreamEventDatas = CSVWriter.SaveCSV_StreamEventDatas(id, "Assets/Resources/Sheet/SaveDatas/", "SentenceSave.txt");

        List<Fragment> fragments = new();
        List<KeyValuePair<string, object>> basicDatas = new();

        foreach (var data in DataManager.BasicDialogDatas[0]) //// ������ ���̾�α� ��ȸ
        {
            if (data.Key.Contains(id.Substring(4, 4)))
            {
                basicDatas.Add(data);
            }
        }

        // ��� ���� ����
        int rand = UnityEngine.Random.Range(1, 3);
        string addID = "T0" + rand.ToString();
        Debug.Log(addID);
        DialogManager.streamTitleText.text = (string)DataManager.TitleDatas[1][id.Substring(4, 4) + addID];

        foreach (var data in basicDatas) // ������ T ������ ��ȸ
        {
            if (data.Key.Contains(addID))
            {
                string key = data.Key; // ������ WAŰ�� ��� ��� �� ��������                    
                string animeId = (string)DataManager.BasicDialogDatas[0][key];
                string script = (string)DataManager.BasicDialogDatas[2][key]; // ��� ��ü �߰��ؾ��� 
                Fragment fragment = new(animeId, script);
                fragments.Add(fragment);

            }
        }

        foreach (var data in DataManager.DialogDatas[0]) // ���̾�α� ��ȸ
        {
            if (data.Key.Contains(id))
            {
                string key = data.Key;
                string animeId = (string)DataManager.DialogDatas[0][key];
                string script = (string)DataManager.DialogDatas[2][key]; // ��� ��ü �߰��ؾ���
                Fragment fragment = new(animeId, script);
                fragments.Add(fragment);
            }
        }
        ScenarioBase scenario = new(fragments);

        return scenario;
    }

    private void ApplyGage()
    {
        DialogManager.currentStreamEvent = this.currentStreamEvent;

        GameManager.currentMainInfo.stressGage += currentStreamEvent.stressValue;
        GameManager.currentMainInfo.angerGage += currentStreamEvent.angerValue;
        GameManager.currentMainInfo.riskGage += currentStreamEvent.riskValue;
        GameManager.currentMainInfo.overloadGage += currentStreamEvent.OverloadValue;

        if (GameManager.currentMainInfo.stressGage < 0) { GameManager.currentMainInfo.stressGage = 0; }
        if (GameManager.currentMainInfo.angerGage < 0) { GameManager.currentMainInfo.angerGage = 0; }
        if (GameManager.currentMainInfo.riskGage < 0) { GameManager.currentMainInfo.riskGage = 0; }
        if (GameManager.currentMainInfo.overloadGage < 0) { GameManager.currentMainInfo.overloadGage = 0; }
    }

    private void SetNumberOfUses(string id)
    {
        string GetKey = "";
        foreach (string Key in currentStreamEventDatas[0].Keys)
        {
            if(id == Key)
            {
                GetKey = Key;
            }
        }

        currentStreamEventDatas[0][GetKey] = "TRUE";
        int currentValue = Convert.ToInt32(currentStreamEventDatas[1][GetKey]);
        currentStreamEventDatas[1][GetKey] = (currentValue + 1).ToString();

        Debug.Log(GetKey + "/" + currentStreamEventDatas[0][GetKey] + "/" + currentStreamEventDatas[1][GetKey]);
       
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

