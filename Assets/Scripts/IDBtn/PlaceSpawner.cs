using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceSpawner : IDBtnSpawner
{
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] RectTransform placeParentObject;

    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn wordBtn = ObjectPooling.WordBtnObjectPool();
        wordBtn.isButton = true;
        wordBtn.buttonValue = word;
        return wordBtn;
    }

    public void SpawnBehaviorActionBtn()
    {
        /*
        for (int i = 0; i < PlaceManager.currentPlaceList.Count; i++)
        {
            IDBtn placeBtn = CreateIDBtn(PlaceManager.currentPlaceList[i]); // 생성
            placeBtn.transform.SetParent(placeParentObject); // 부모 설정
            placeBtn.buttonType = ButtonType.UnSortType; // 정렬
            PlaceManager.enablePlaceBtnList.Add(placeBtn); // 활성화 리스트에 삽입
            PlaceManager.PlaceBtnListSet(); // 데이터 삽입
            placeBtn.gameObject.SetActive(true); // 활성화
        }
        */
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

    protected override void OnDisable()
    {
        base.OnDisable();
        // PickBehaviorAction();
    }
}
