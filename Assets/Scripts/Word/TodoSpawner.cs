using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TodoSpawner : IDBtnSpawner
{
    [SerializeField] RectTransform wordActionParentObject;

    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = true;
        wordBtn.word = word;
        return wordBtn;
    }

    protected override void SpawnIDBtn()
    {
        SpawnWordBtn();
    }

    private void SpawnWordBtn()
    {
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            IDBtn wordBtn = CreateIDBtn(WordManager.currentWordList[i]); // 생성
            wordBtn.transform.SetParent(wordParentObject); // 부모 설정
            WordManager.enableWordBtnList.Add(wordBtn); // 활성화 리스트에 삽입
            WordManager.WordBtnListSet(); // 데이터 삽입
            wordBtn.gameObject.SetActive(true); // 활성화
        }
    }

    public void SpawnWordActionBtn()
    {
        PickWordAction(); // 버튼 초기화

        for (int i = 0; i < WordManager.currentWordActionList.Count; i++)
        {
            IDBtn wordBtn = CreateIDBtn(WordManager.currentWordActionList[i]); // 생성
            wordBtn.transform.SetParent(wordActionParentObject); // 부모 설정
            WordManager.enableWordActionBtnList.Add(wordBtn); // 활성화 리스트에 삽입
            WordManager.WordActionBtnListSet(); // 데이터 삽입
            wordBtn.gameObject.SetActive(true); // 활성화
        }
    }
    protected override void PickIDBtn()
    {
        PickWordBtn();
    }

    public void PickWordBtn()
    {
        if (WordManager.enableWordBtnList.Count != 0)
        {
            for (int i = WordManager.enableWordBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.ObjectPick(WordManager.enableWordBtnList[i]);
            }
            WordManager.enableWordBtnList.Clear();
        }
    }

    public void PickWordAction()
    {
        if (WordManager.enableWordActionBtnList.Count != 0)
        {
            for (int i = WordManager.enableWordActionBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.ObjectPick(WordManager.enableWordActionBtnList[i]);
            }
            WordManager.enableWordActionBtnList.Clear();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PickWordAction();
    }
}
