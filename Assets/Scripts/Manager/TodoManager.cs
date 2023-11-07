using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TodoManager : MonoBehaviour
{
    [Header("*Property")]
    [SerializeField] GameManager GameManager;
    [SerializeField] WordBtnSpawner WordBtnSpawner;

    // �ൿ ���� ��ư
    List<Button> wordBtns = new();
    List<Button> wordActionBtns = new();

    // ������ �ܾ��� �̸�
    string currentWordName;

    // ������ �ܾ��� �׼� ���
    public List<WordActionData> currentWordActionData = new();


    private void Start()
    {
        foreach (Button wordBtn in wordBtns)
        {
            wordBtn
                .OnClickAsObservable()
                .Select(buttonNum => wordBtn.transform.GetSiblingIndex())
                .Subscribe(buttonNum =>
                {
                    if(WordBtnSpawner.enableWordBtnList.Count != 0)
                    {
                        currentWordName = WordBtnSpawner.enableWordBtnList[buttonNum].wordBtnTextStr;
                        currentWordActionData = FindWordActions(FindWord());
                    }
                });
        }
    }

    private void OnEnable()
    {
        GetButtonList();
    }

    private void GetButtonList()
    {
        wordBtns.Clear();

        for(int i = 0; i < WordBtnSpawner.enableWordBtnList.Count; i++)
        {
            wordBtns.Add(WordBtnSpawner.enableWordBtnList[i].button);
        }
    }

    // �� �Ʒ����ٰ� �ܾ� ���ý� �ൿ ������ �����Ǵ� �Լ� �����
    private WordData FindWord()
    {
        foreach (WordData data in GameManager.wordDatas)
        {
            if(currentWordName == data.wordName)
            {
                return data;
            }
        }
        return null;
    }

    private List<WordActionData> FindWordActions(WordData wordData)
    {
        List<WordActionData> datas = new();
        foreach (WordActionData data in wordData.wordActionDatas)
        {
            if (data.wordActionBool == true)
            {
                datas.Add(data);
            }
        }
        return datas;
    }

}
