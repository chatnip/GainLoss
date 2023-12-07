using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

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
        
        // ��ȭ �� Apply
        if (streamEvent.used) 
        {
            streamEvent.stressValue = Convert.ToInt32(DataManager.StreamEventDatas[1][id]) / 2;
            streamEvent.angerValue = Convert.ToInt32(DataManager.StreamEventDatas[2][id]) / 2;
            streamEvent.riskValue = Convert.ToInt32(DataManager.StreamEventDatas[3][id]) / 2;
        }
        else
        {
            streamEvent.stressValue = Convert.ToInt32(DataManager.StreamEventDatas[1][id]);
            streamEvent.angerValue = Convert.ToInt32(DataManager.StreamEventDatas[2][id]);
            streamEvent.riskValue = Convert.ToInt32(DataManager.StreamEventDatas[3][id]);
        }

        GameManager.stressGage += (int)(streamEvent.stressValue);
        GameManager.angerGage += (int)(streamEvent.angerValue);
        GameManager.riskGage += (int)(streamEvent.riskValue);

        DialogManager.streamEvent = streamEvent;

        if (Convert.ToBoolean(DataManager.StreamEventDatas[0][id]) == false)
        {
            DataManager.StreamEventDatas[0][id] = true.ToString();
        }

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

