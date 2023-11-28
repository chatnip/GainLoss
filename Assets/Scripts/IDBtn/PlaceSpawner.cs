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
        PickBehaviorActionBtn(); // ��ư �ʱ�ȭ
        for (int i = 0; i < PlaceManager.currentBehaviorActionList.Count; i++)
        {
            IDBtn behaviorActionBtn = CreateIDBtn(PlaceManager.currentBehaviorActionList[i]); // ����
            behaviorActionBtn.transform.SetParent(behaviorActionParentObject); // �θ� ����
            behaviorActionBtn.buttonType = ButtonType.BehaviorActionType; // ����
            PlaceManager.enableBehaviorActionBtnList.Add(behaviorActionBtn); // Ȱ��ȭ ����Ʈ�� ����
            PlaceManager.BehaviorActionBtnListSet(); // ������ ����
            behaviorActionBtn.gameObject.SetActive(true); // Ȱ��ȭ
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