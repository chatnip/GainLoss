//Refactoring v1.0
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ReasoningPhoto : MonoBehaviour
{
    #region Value

    [Header("=== Relation")]
    [SerializeField] public string relationObejctID;
    [SerializeField] public bool isVisible = false;

    [Header("=== Name")]
    [SerializeField] string nameID;
    [SerializeField] bool isVisibleName = false;
    [SerializeField] TMP_Text nameTxt;

    [Header("=== Component")]
    [SerializeField] public CanvasGroup actualPhotoCG;

    #endregion

    #region Base Set

    public void Offset()
    {
        LanguageManager.Instance.SetLanguageTxt(nameTxt);
    }

    #endregion

    #region Active On

    public void CheckVisible(float time)
    {
        if (!actualPhotoCG.gameObject.activeSelf && isVisible)
        {
            actualPhotoCG.gameObject.SetActive(true);

            actualPhotoCG.alpha = 0f;

            actualPhotoCG.DOFade(1f, time);
        }

        if(!isVisibleName)
        {
            nameTxt.text = "???";
        }
        else
        {
            nameTxt.text = DataManager.Instance.ObjectCSVDatas[LanguageManager.Instance.languageNum][nameID].ToString();
        }
    }

    #endregion
}
