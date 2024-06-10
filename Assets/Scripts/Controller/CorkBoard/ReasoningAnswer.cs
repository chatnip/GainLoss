//Refactoring v1.0
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReasoningAnswer : ReasoningModule
{
    #region Value

    [Header("=== Data")]
    [SerializeField] string thisTagID;
    [SerializeField] ReasoningArrow relationRA;
    [SerializeField] ReasoningPhoto relationPT;

    [Header("=== Component")]
    [SerializeField] CanvasGroup thisCG;
    [SerializeField] TMP_Dropdown thisDropdown;


    [Header("=== Content")]
    [SerializeField] List<string> tagContentIDs;
    [SerializeField] public string currentSelectedContentID;

    #endregion

    #region Active On

    public void Offset()
    {
        
    }

    public void CheckVisible(float time)
    {
        if (relationRA != null && !this.gameObject.activeSelf && relationRA.isVisible)
        {
            this.gameObject.SetActive(true);

            thisCG.alpha = 0f;

            thisCG.DOFade(1f, time);
        }
        else if (relationPT != null && !this.gameObject.activeSelf && relationPT.isVisible)
        {
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
    }

    #endregion

}
