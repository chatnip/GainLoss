//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReasoningModule : MonoBehaviour
{
    #region Value

    [Header("--- Base Value")]
    public string thisID;
    public CanvasGroup thisCG;
    public bool isActive;
    [SerializeField] protected List<TMP_Text> thisTmpTexts = new List<TMP_Text>();

    #endregion

    #region Offset
    public void SetThisTmp()
    {
        foreach (TMP_Text tmp in thisTmpTexts)
        { LanguageManager.Instance.SetLanguageTxt(tmp); }
        this.gameObject.SetActive(false);
    }

    #endregion

    #region OnEnable

    public virtual void SetEachTime(float time)
    {
        // Set Visible
        if (!this.gameObject.activeSelf && isActive)
        {
            this.gameObject.SetActive(true);
            thisCG.alpha = 0f;
            thisCG.DOFade(1f, time);
        }
    }

    #endregion
}
