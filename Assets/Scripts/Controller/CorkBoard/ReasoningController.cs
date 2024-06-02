//Refactoring v1.0
using System.Collections.Generic;
using UnityEngine;

public class ReasoningController : Singleton<ReasoningController>
{
    #region Value

    [Header("=== ID")]
    [SerializeField] public string reasoningID;

    [Header("=== Photo")]
    [SerializeField] Transform photoParentTF;
    [SerializeField] List<ReasoningPhoto> photos = new List<ReasoningPhoto>();

    [Header("=== Arrow")]
    [SerializeField] Transform arrowParentTF;
    [SerializeField] List<ReasoningArrow> arrows = new List<ReasoningArrow>();

    #endregion

    #region Framework & Base Set

    public void Offset()
    {
        this.gameObject.SetActive(false);

        // Photo
        foreach (Transform photoTF in photoParentTF)
        {
            if (photoTF.TryGetComponent(out ReasoningPhoto RP))
            { photos.Add(RP); }
        }

        // Arrow
        foreach (Transform photoTF in arrowParentTF)
        {
            if (photoTF.TryGetComponent(out ReasoningArrow RA))
            { arrows.Add(RA); }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        Offset();
    }

    #endregion
}
