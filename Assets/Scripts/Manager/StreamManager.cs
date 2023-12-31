using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using DG.Tweening.Plugins.Core.PathCore;

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
        CSVWriter.SaveCSV("Assets/Resources/Sheet/", "SentenceSheet.csv", "Assets/Resources/Sheet/SaveDatas/", "SentenceSheet_Saved.csv");
        DataManager.StreamEventDatas = CSVWriter.SaveCSV_StreamEventDatas(id, "Assets/Resources/Sheet/SaveDatas/", "SentenceSheet_Saved.csv");

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

        GameManager.stressGage += currentStreamEvent.stressValue;
        GameManager.angerGage += currentStreamEvent.angerValue;
        GameManager.riskGage += currentStreamEvent.riskValue;
        GameManager.OverloadGage += currentStreamEvent.OverloadValue;

        if (GameManager.stressGage < 0) { GameManager.stressGage = 0; }
        if (GameManager.angerGage < 0) { GameManager.angerGage = 0; }
        if (GameManager.riskGage < 0) { GameManager.riskGage = 0; }
        if (GameManager.OverloadGage < 0) { GameManager.OverloadGage = 0; }
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

