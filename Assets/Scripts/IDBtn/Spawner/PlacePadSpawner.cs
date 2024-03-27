using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacePadSpawner : IDBtnSpawner
{
    [SerializeField] PlaceManager PlaceManager;

    [Header("*WordParentObj")]
    [SerializeField] RectTransform PlaceParentObject;

    protected override void SpawnIDBtn()
    {
        for (int i = 0; i < PlaceManager.currentPlaceID_Dict.Count; i++)
        {
            IDBtn PlaceBtn = CreateIDBtn(new ButtonValue(PlaceManager.currentPlaceID_Dict.Keys.ToList()[i], DataManager.PlaceDatas[1][PlaceManager.currentPlaceID_Dict.Keys.ToList()[i]].ToString()));
            PlaceBtn.AddVisiableWordRate("");
            PlaceBtn.transform.SetParent(PlaceParentObject);
            PlaceBtn.buttonType = ButtonType.PlacePadType;
            //if (PlaceBtn.TryGetComponent(out RectTransform RT)) { RT.sizeDelta = new Vector2(Screen.width, Screen.height * 0.3f); }
            //PlaceBtn.text.fontSize = 90;
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
