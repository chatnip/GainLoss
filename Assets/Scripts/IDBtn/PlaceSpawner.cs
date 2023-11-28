using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceSpawner : IDBtnSpawner
{
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] RectTransform behaviorActionParentObject;

    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = true;
        wordBtn.buttonValue = word;
        return wordBtn;
    }

    public void SpawnBehaviorActionBtn()
    {
        PickBehaviorActionBtn(); // 버튼 초기화
        for (int i = 0; i < PlaceManager.currentBehaviorActionList.Count; i++)
        {
            IDBtn behaviorActionBtn = CreateIDBtn(PlaceManager.currentBehaviorActionList[i]); // 생성
            behaviorActionBtn.transform.SetParent(behaviorActionParentObject); // 부모 설정
            behaviorActionBtn.buttonType = ButtonType.BehaviorActionType; // 정렬
            PlaceManager.enableBehaviorActionBtnList.Add(behaviorActionBtn); // 활성화 리스트에 삽입
            PlaceManager.BehaviorActionBtnListSet(); // 데이터 삽입
            behaviorActionBtn.gameObject.SetActive(true); // 활성화
        }
    }

    protected override void PickIDBtn()
    {
        PickPlaceBtn();
    }

    public void PickPlaceBtn()
    {
        if (PlaceManager.enablePlaceBtnList.Count != 0)
        {
            for (int i = PlaceManager.enablePlaceBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.ObjectPick(PlaceManager.enablePlaceBtnList[i]);
            }
            PlaceManager.enablePlaceBtnList.Clear();
        }
    }

    public void PickBehaviorActionBtn()
    {
        if (PlaceManager.enableBehaviorActionBtnList.Count != 0)
        {
            for (int i = PlaceManager.enableBehaviorActionBtnList.Count - 1; i >= 0; i--)
            {
                ObjectPooling.ObjectPick(PlaceManager.enableBehaviorActionBtnList[i]);
            }
            PlaceManager.enableBehaviorActionBtnList.Clear();
        }
    }



    protected override void OnDisable()
    {
        base.OnDisable();
        PickBehaviorActionBtn();
    }
}