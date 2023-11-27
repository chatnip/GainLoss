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

    public ScenarioBase InitStreamEventID(string id)
    {
        // ��� ���� ����
        DialogManager.streamTitleText.text = (string)DataManager.StreamEventDatas[5][id];

        List<Fragment> fragments = new();
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
}

[System.Serializable]
public class StreamEvent
{
    [SerializeField] bool used;
    [SerializeField] int stressValue;
    [SerializeField] int angerValue;
    [SerializeField] int riskValue;
}

