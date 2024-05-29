using DG.Tweening;
using UnityEngine;

public class EffectfulWindow : MonoBehaviour
{
    public static void AppearEffectful(RectTransform RT, float time, float startSize, Ease ease)
    {
        GameManager.Instance.canInput = false;
        RT.transform.localScale = Vector3.one * startSize;

        RT.gameObject.SetActive(true);
        RT.transform.DOScale(Vector3.one, time)
            .SetEase(ease)
            .SetUpdate(true)
            .OnComplete(() => { GameManager.Instance.canInput = true; });
            
    }
    public static void DisappearEffectful(RectTransform RT, float time, float endSize, Ease ease)
    {
        GameManager.Instance.canInput = false;
        RT.transform.localScale = Vector3.one;

        RT.transform.DOScale(Vector3.one * endSize, time)
            .SetEase(ease)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                GameManager.Instance.canInput = true;
                RT.gameObject.SetActive(false);
            });
    }
}
