using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;

public class GetDataWithID : MonoBehaviour
{
    [Header("*Manager")]
    [SerializeField] WordJson WordJson;
    [SerializeField] WordManager WordManager;

    [Header("*Btn")]
    [SerializeField] Button CloseBtn;

    [Header("*TMP_Text")]
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Extension;
    [HideInInspector] public string ID;

    private void Awake()
    {
        CloseBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                EffectfulWindow.DisappearEffectful(this.GetComponent<RectTransform>(), 0.3f, 0.7f, DG.Tweening.Ease.Linear);
                if(Extension.text == ".AIL")
                {
                    InputWordData();
                }
                else if(Extension.text == ".EXE")
                {
                    InputWordActionData();
                }
            });
    }

    public void SetInfo(string id, string extension)
    {
        this.gameObject.SetActive(true);
        ID = id;
        Name.text = (string)DataManager.WordDatas[5][id];
        Extension.text = extension;
    }

    private void InputWordData()
    {
        bool alreadyHad = false;
        foreach(string id in WordManager.currentWordIDList)
        {
            if(id == ID)
            {
                alreadyHad = true;
            }
        }
        if (!alreadyHad)
        {
            WordManager.currentWordIDList.Add(ID);
        }
        else
        {
            Debug.Log("이미 가지고 있는 W");
        }
        
        WordJson.JsonSaveTest(WordJson.json_filePath, WordJson.json_wordFileName, WordManager.currentWordIDList);

        clearCurrentGetID();
    }

    private void InputWordActionData()
    {
        bool alreadyHad = false;
        foreach (string id in WordManager.currentWordActionIDList)
        {
            if (id == ID)
            {
                alreadyHad = true;
            }
        }
        if (!alreadyHad)
        {
            WordManager.currentWordActionIDList.Add(ID);
        }
        else
        {
            Debug.Log("이미 가지고 있는 WA");
        }

        WordJson.JsonSaveTest(WordJson.json_filePath, WordJson.json_wordActionFileName, WordManager.currentWordActionIDList);

        clearCurrentGetID();
    }

    private void clearCurrentGetID()
    {
        ID = null;
        Name.text = null;
        Extension.text = null;
    }
}
