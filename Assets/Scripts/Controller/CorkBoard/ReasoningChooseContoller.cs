//Refactoring v1.0
using DG.Tweening;
using UnityEngine;

public class ReasoningChooseContoller : Singleton<ReasoningChooseContoller>
{
    #region Value

    [Header("=== Component")]
    [SerializeField] CanvasGroup thisCG;
    [SerializeField] RectTransform thisRT;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        this.gameObject.SetActive(false);
        thisCG.alpha = 0f;
        thisRT.anchoredPosition = new Vector2(-thisRT.sizeDelta.x, thisRT.anchoredPosition.y);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Active On / Off

    public void ActiveOn(float time)
    {
        this.gameObject.SetActive(true);
        thisCG.DOFade(1f, time);
        thisRT.DOAnchorPos(new Vector2(0f, thisRT.anchoredPosition.y), time);
    }

    public void ActiveOff(float time)
    {
        thisCG.DOFade(0f, time);
        thisRT.DOAnchorPos(new Vector2(-thisRT.sizeDelta.x, thisRT.anchoredPosition.y), time)
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
    }

    #endregion
}
