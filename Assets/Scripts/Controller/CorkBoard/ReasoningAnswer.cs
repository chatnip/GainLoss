//Refactoring v1.0
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ReasoningAnswer : MonoBehaviour
{
    #region Value

    [Header("=== Data")]
    [SerializeField] string thisTagID;
    [SerializeField] ReasoningArrow relationRA; 
    
    [Header("=== Component")]
    [SerializeField] CanvasGroup thisCG;
    [SerializeField] Button thisBtn;

    [Header("=== Content")]
    [SerializeField] TMP_Text currentSelectedContentTxt;
    [SerializeField] List<string> tagContentIDs;
    [SerializeField] public string currentSelectedContentID;

    public IDisposable thisBtnIDis = null;
    #endregion

    #region Active On

    public void CheckVisible(float time)
    {
        if (!this.gameObject.activeSelf && relationRA.isVisible)
        {
            currentSelectedContentTxt.text = null;

            this.gameObject.SetActive(true);

            thisCG.alpha = 0f;

            thisCG.DOFade(1f, time);

        }

        // Set Tag IDs
        tagContentIDs = new List<string>();
        foreach(string id in ReasoningManager.Instance.reasoningContentIDs)
        {
            if(id.Substring(0, 3) == thisTagID)
            {
                tagContentIDs.Add(id);
            }
        }

        if(thisBtnIDis == null)
        {
            thisBtnIDis = thisBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ReasoningController.Instance.SetReasoningBtn(tagContentIDs);
            });
        }
        
    }

    #endregion

}
