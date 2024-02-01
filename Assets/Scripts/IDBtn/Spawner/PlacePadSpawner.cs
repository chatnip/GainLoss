using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePadSpawner : IDBtnSpawner
{
    [SerializeField] PlaceManager PlaceManager;

    [Header("*WordParentObj")]
    [SerializeField] RectTransform PlaceParentObject;

    protected override void SpawnIDBtn()
    {
        for (int i = 0; i < PlaceManager.currentPlaceIDList.Count; i++)
        {
            IDBtn PlaceBtn = CreateIDBtn(new ButtonValue(PlaceManager.currentPlaceIDList[i], DataManager.PlaceDatas[1][PlaceManager.currentPlaceIDList[i]].ToString()));
            PlaceBtn.transform.SetParent(PlaceParentObject);
            PlaceBtn.buttonType = ButtonType.WordPadType;
            PlaceManager.enablePlaceBtnList.Add(PlaceBtn);
            PlaceBtn.gameObject.SetActive(true);
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
                ObjectPooling.WordObjectPick(PlaceManager.enablePlaceBtnList[i]);
            }
            PlaceManager.enablePlaceBtnList.Clear();
        }
    }

    protected override IDBtn CreateIDBtn(ButtonValue word)
    {
        IDBtn placeBtn = ObjectPooling.WordBtnObjectPool();
        placeBtn.isButton = false;
        placeBtn.buttonValue = word;
        return placeBtn;
    }
}
