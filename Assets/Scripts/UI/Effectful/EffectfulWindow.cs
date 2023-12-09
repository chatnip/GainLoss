using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class EffectfulWindow : MonoBehaviour
{
    public static void AppearEffectful(RectTransform RT, float time, float size, Ease ease)
    {
        RT.transform.localScale = Vector3.one * size;

        RT.gameObject.SetActive(true);
        RT.transform.DOScale(Vector3.one, time)
            .SetEase(ease);
    }
    public static void DisappearEffectful(RectTransform RT, float time, float size, Ease ease)
    {
        RT.transform.localScale = Vector3.one;

        RT.transform.DOScale(Vector3.one * size, time)
            .SetEase(ease)
            .OnComplete(() =>
            {
                RT.gameObject.SetActive(false);
            });
    }
}
