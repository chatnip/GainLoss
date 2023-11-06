using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TodoManager : MonoBehaviour
{
    [SerializeField] GameManager GameManager;
    [SerializeField] WordSpawner WordSpawner;
    List<Button> wordBtns = new List<Button>();
    WordBase currentWord = new WordBase();
    WordData tempWordData;

    private void Start()
    {
        foreach (Button wordBtn in wordBtns)
        {
            wordBtn
                .OnClickAsObservable()
                .Select(buttonNum => wordBtn.transform.GetSiblingIndex())
                .Subscribe(buttonNum =>
                {
                    if(WordSpawner.enableWordList.Count != 0)
                    {
                        currentWord = WordSpawner.enableWordList[buttonNum].wordBase;
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

        for(int i = 0; i < WordSpawner.enableWordList.Count; i++)
        {
            wordBtns.Add(WordSpawner.enableWordList[i].wordBtn);
        }
    }

    // �� �Ʒ����ٰ� �ܾ� ���ý� �ൿ ������ �����Ǵ� �Լ� �����
    private void SpawnWordAction()
    {
        foreach (WordData data in GameManager.wordDatas)
        {
            if(currentWord.wordName == data.wordName)
            {
                tempWordData = data;
            }
        }
        
        foreach(WordActionData actionData in tempWordData.wordActionDatas)
        {

        }
    }

}
