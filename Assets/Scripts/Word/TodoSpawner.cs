using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TodoSpawner : WordBtnSpawner
{
    [SerializeField] RectTransform wordActionParentObject;

    public override void SpawnWordBtn()
    {
        for (int i = 0; i < WordManager.currentWordList.Count; i++)
        {
            WordBtn wordBtn = CreateWordBtn(WordManager.currentWordList[i]); // 생성
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
            WordBtn wordBtn = CreateWordBtn(WordManager.currentWordActionList[i]); // 생성
            wordBtn.transform.SetParent(wordActionParentObject); // 부모 설정
            WordManager.enableWordActionBtnList.Add(wordBtn); // 활성화 리스트에 삽입
            WordManager.WordActionBtnListSet(); // 데이터 삽입
            wordBtn.gameObject.SetActive(true); // 활성화
        }
    }

    protected override WordBtn CreateWordBtn(Word word)
    {
        WordBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = true;
        wordBtn.word = word;
        return wordBtn;
    }

    public override void PickWordAction()
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
}
