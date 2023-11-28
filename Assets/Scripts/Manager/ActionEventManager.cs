using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ActionEventManager : Manager<ActionEventManager>
{
    [HideInInspector] public string currentActionEventID;
    [SerializeField] List<Place> placeList = new();
    [SerializeField] CanvasGroup loading;

    private Place currentPlace;

    public void PlaceSetting()
    {
        TurnOnLoading();

        foreach (var place in placeList) // ������ ���̾�α� ��ȸ
        {
            if (place.placeID.Contains(currentActionEventID))
            {
                
            }
        }
    }

    private void TurnOnLoading()
    {
        Sequence loadingSequence = DOTween.Sequence();
        loadingSequence.OnStart(() =>
        {
            loading.DOFade(1f, 1f);
        })
        .SetDelay(5f)
        .Append(loading.DOFade(0f, 1f));
    }
}
