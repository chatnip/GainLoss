using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ClueData : MonoBehaviour
{
    #region Value

    [Header("*Img")]
    [SerializeField] public Sprite mainSprite;

    [Header("*IDBtn")]
    [SerializeField] public List<IDBtn> ClueSpotBtns = new List<IDBtn>();

    [Header("*What Value is Changing")]
    [SerializeField] public Button partnerBtn;
    [SerializeField] public List<string> CanCombineIDList = new List<string>();

    #endregion

    #region Main

    private void Awake()
    {
        foreach(IDBtn idBtn in ClueSpotBtns)
        {
            idBtn.TryGetComponent(out Button btn);
            btn.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ft_getID(idBtn);
                });
        }
    }

    public void ft_getID(IDBtn idBtn)
    {
        if (!CanCombineIDList.Contains(idBtn.buttonValue.ID))
        {
            idBtn.ClueSpotTypeInput();
            CanCombineIDList.Add(idBtn.buttonValue.ID);
        }
    }

    #endregion


}
