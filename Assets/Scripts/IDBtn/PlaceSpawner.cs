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

    protected override void SpawnIDBtn()
    {
        SpawnPlaceBtn();
    }

    private void SpawnPlaceBtn()
    {
        for (int i = 0; i < PlaceManager.currentPlaceList.Count; i++)
        {
            IDBtn placeBtn = CreateIDBtn(PlaceManager.currentPlaceList[i]); // ����
            placeBtn.transform.SetParent(placeParentObject); // �θ� ����
            placeBtn.buttonType = ButtonType.UnSortType; // ����
            PlaceManager.enablePlaceBtnList.Add(placeBtn); // Ȱ��ȭ ����Ʈ�� ����
            PlaceManager.PlaceBtnListSet(); // ������ ����
            placeBtn.gameObject.SetActive(true); // Ȱ��ȭ
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

    protected override void OnDisable()
    {
        base.OnDisable();
        // PickBehaviorAction();
    }
}
