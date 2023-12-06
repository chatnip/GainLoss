using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using System;

public class StreamManager : Manager<StreamManager>
{
    [SerializeField] GameManager GameManager;
    [SerializeField] DialogManager DialogManager;
    
    [HideInInspector] public string currentStreamEventID;

    public void StartDialog(string id)
    {
        DialogManager.ScenarioBase.Value = InitStreamEventID(id);
    }

    public ScenarioBase InitStreamEventID(string id) //currentStreamEventID
    {
        List<Fragment> fragments = new();
        List<KeyValuePair<string, object>> basicDatas = new();

        foreach (var data in DataManager.BasicDialogDatas[0]) // ������ ���̾�α� ��ȸ
        {
            if (data.Key.Contains(id.Substring(4, 4)))
            {
                basicDatas.Add(data);
            }
        }
        
        // ��� ���� ����
        int rand = UnityEngine.Random.Range(1, 4);
        string addID = "T0" + rand.ToString();
        DialogManager.streamTitleText.text = (string)DataManager.TitleDatas[1][id.Substring(4, 4) + addID];

        // ��ġ ��ȭ
        // ���� �� Set
        StreamEvent streamEvent = new StreamEvent();
        streamEvent.used = Convert.ToBoolean(DataManager.StreamEventDatas[0][id]);
        streamEvent.stressValue = Convert.ToInt32(DataManager.StreamEventDatas[1][id]);
        streamEvent.angerValue = Convert.ToInt32(DataManager.StreamEventDatas[2][id]);
        streamEvent.riskValue = Convert.ToInt32(DataManager.StreamEventDatas[3][id]);
        DialogManager.streamEvent = streamEvent;

        // ��ȭ �� Apply
        float decMultiple = 1.0f;
        if (streamEvent.used) { decMultiple = 0.5f; }
        GameManager.stressGage += (int)(streamEvent.stressValue * decMultiple);
        GameManager.angerGage += (int)(streamEvent.angerValue * decMultiple);
        GameManager.riskGage += (int)(streamEvent.riskValue * decMultiple);

        

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

        Debug.Log(Convert.ToBoolean(DataManager.StreamEventDatas[0][id]));

        return scenario;
    }
}

[System.Serializable]
public class StreamEvent
{
    [HideInInspector] public bool used;
    [HideInInspector] public int stressValue;
    [HideInInspector] public int angerValue;
    [HideInInspector] public int riskValue;
}

