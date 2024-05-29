//Refactoring v1.0
using DG.Tweening;
using UnityEngine;

public class StreamingController : DesktopController
{
    #region Value

    [Header("=== Loading Screen")]
    [SerializeField] GameObject loadingScreenGO;
    [SerializeField] RectTransform rotateRT;

    #endregion

    #region Framework & Base Set



    #endregion

    #region Active On/Off

    public override void ActiveOn()
    {
        base.ActiveOn();

        // Loading Screen
        loadingScreenGO.gameObject.SetActive(true);
        rotateRT.DORotate(new Vector3(0f, 0f, -360f), 0.2f, RotateMode.FastBeyond360)
            .SetLoops(5, LoopType.Restart)
            .OnComplete(() =>
            {
                Debug.Log("Start Streaming");
            });
    }

    #endregion
}
