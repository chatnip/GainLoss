using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class StreamManager : Manager<StreamManager>
{
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
    [SerializeField] bool used;
    [SerializeField] int stressValue;
    [SerializeField] int angerValue;
    [SerializeField] int riskValue;
}

