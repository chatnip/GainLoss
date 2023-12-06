using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class ActionEventManager : Manager<ActionEventManager>
{
    [SerializeField] GameSystem GameSystem;
    [HideInInspector] public string currentActionEventID;
    [SerializeField] List<PlaceDataBase> placeList = new();

    [SerializeField] Transform placeParent;
    [SerializeField] GameObject currentPlace;
    [SerializeField] GameObject home;


    [SerializeField] private ReactiveProperty<PlaceDataBase> placeData = new ReactiveProperty<PlaceDataBase>();


    protected override void Awake()
    {
        base.Awake();

        placeData
            .Where(data => data != null)
            .Subscribe(data =>
            {
                StartCoroutine(ParsePlace(data));
            });
    }

    public IEnumerator PlaceSetting()
    {
        GameSystem.TurnOnLoading();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ChangePlacePerData(FindPlace()));
    }

    private PlaceDataBase FindPlace() // 장소를 리소스에서 찾기
    {
        var allPlace = ResourceData<PlaceDataBase>.GetDatas("Place/PlaceData");
        return Array.Find(allPlace, x => x.placeID == currentActionEventID.Substring(0, 3));
    }

    private IEnumerator ParsePlace(PlaceDataBase data) // 장소 세팅하기
    {
        yield return new WaitForEndOfFrame();
        home.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        currentPlace = Instantiate(data.place, placeParent);
        GameSystem.playerPos.position = data.spawnPos;
    }

    public IEnumerator ChangePlacePerData(PlaceDataBase placeData) // UniRX 데이터에 찾은 장소 넣기
    {
        this.placeData.Value = null;
        yield return new WaitForEndOfFrame();
        this.placeData.Value = placeData;
    }
}
