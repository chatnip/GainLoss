//Refactoring v1.0
using DG.Tweening;
using UnityEngine;

public class ReasoningChooseContoller : Singleton<ReasoningChooseContoller>
{
    #region Value

    [Header("=== Component")]
    [SerializeField] CanvasGroup thisCG;

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        this.gameObject.SetActive(false);
        thisCG.alpha = 0f;
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
        thisCG.alpha = 0f;
        thisCG.DOFade(1f, time);
    }

    public void ActiveOff(float time)
    {
        thisCG.DOFade(0f, time)
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
    }

    #endregion
}
