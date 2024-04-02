using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class ClueData : MonoBehaviour
{
    #region Value

    [Header("*Original")]
    [SerializeField] public string clueName;

    [Header("*Img")]
    [SerializeField] public Sprite mainSprite;

    [Header("*Txt")]
    [SerializeField] public GameObject TxtBox;

    [Header("*Btn")]
    [SerializeField] public Button ClueSpotBtn;

    [Header("*What Value is Changing")]
    [SerializeField] public Button partnerBtn;

    #endregion

    #region Main

    private void Awake()
    {
        ClueSpotBtn.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ft_showInfo();
                });
    }

    public void ft_showInfo()
    {
        ClueSpotBtn.TryGetComponent(out Image img);
        if (img.fillAmount == 0) { img.DOFillAmount(1, 1); }
        TxtBox.SetActive(true);
    }

    #endregion


}
