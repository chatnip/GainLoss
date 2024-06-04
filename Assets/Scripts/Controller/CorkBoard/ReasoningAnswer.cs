//Refactoring v1.0
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReasoningAnswer : MonoBehaviour
{
    #region Value

    [Header("=== Data")]
    [SerializeField] string thisTagID;
    [SerializeField] ReasoningArrow relationRA;
    [SerializeField] ReasoningPhoto relationPT;

    [Header("=== Component")]
    [SerializeField] CanvasGroup thisCG;
    [SerializeField] Button thisBtn;

    [Header("=== Content")]
    [SerializeField] public TMP_Text currentSelectedContentTxt;
    [SerializeField] List<string> tagContentIDs;
    [SerializeField] public string currentSelectedContentID;

    #endregion

    #region Active On

    public void Offset()
    {
        thisBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ReasoningController.Instance.selectedAnswer = this;
                ReasoningController.Instance.SetReasoningBtn(tagContentIDs);
                ReasoningChooseContoller.Instance.ActiveOn_ChooseBtn(0.2f);
            });
    }

    public void CheckVisible(float time)
    {
        if (relationRA != null && !this.gameObject.activeSelf && relationRA.isVisible)
        {
            currentSelectedContentTxt.text = null;

            this.gameObject.SetActive(true);

            thisCG.alpha = 0f;

            thisCG.DOFade(1f, time);
        }
        else if (relationPT != null && !this.gameObject.activeSelf && relationPT.isVisible)
        {
            currentSelectedContentTxt.text = null;

            this.gameObject.SetActive(true);

            thisCG.alpha = 0f;

            thisCG.DOFade(1f, time);
        }

        // Set Tag IDs
        tagContentIDs = new List<string>();
        foreach (string id in ReasoningManager.Instance.reasoningContentIDs)
        {
            if (id.Substring(0, 3) == thisTagID)
            {
                tagContentIDs.Add(id);
            }
        }

    }

    #endregion

    #region Set

    public void SetContent(string id)
    {
        currentSelectedContentID = id;
        currentSelectedContentTxt.text = DataManager.Instance.ReasoningContentCSVDatas[LanguageManager.Instance.languageNum][currentSelectedContentID].ToString();
    }

    #endregion

}
