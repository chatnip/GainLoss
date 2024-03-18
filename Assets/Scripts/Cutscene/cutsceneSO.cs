using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CutScene SO", menuName = "Scriptable Object/CutScene SO", order = int.MaxValue)]
public class cutsceneSO : ScriptableObject
{
    [SerializeField] public List<Sprite> cutsceneSprites = new List<Sprite>();
    [TextArea][SerializeField] public List<string> cutsceneDialogs = new List<string>();

    public static cutsceneSO currentCSSO = null;
    public static Sequence cutsceneSeq = null;
    public static bool cutsceneIsPlaying = false;
    public static Sequence makeCutscene(Image cutsceneImg, TMP_Text cutsceneTxt)
    {
        cutsceneImg.color = new Color(0, 0, 0, 0);
        cutsceneImg.gameObject.SetActive(true);
        cutsceneTxt.text = "";

        Sequence thisSequence = DOTween.Sequence();
        thisSequence.timeScale = 1.0f;
        cutsceneImg.sprite = currentCSSO.cutsceneSprites[0];

        thisSequence.Append(cutsceneImg.DOFade(1.0f, 0.25f));
        for (int i = 0; i < currentCSSO.cutsceneSprites.Count; i++)
        {
            thisSequence.Append(cutsceneImg.DOColor(Color.white, 0.25f));
            thisSequence.Append(cutsceneTxt.DOText(currentCSSO.cutsceneDialogs[i], 2.0f)
                .OnStart(() =>
                {
                    cutsceneTxt.transform.parent.gameObject.SetActive(true);
                    cutsceneIsPlaying = true;
                })
                .OnComplete(() =>
                {
                    thisSequence.Pause();
                }));
            thisSequence.Append(cutsceneImg.DOColor(Color.black, 0.25f)
                .OnStart(() =>
                {
                    cutsceneTxt.text = "";
                    cutsceneTxt.transform.parent.gameObject.SetActive(false);
                }));
        }

        return thisSequence;
    }
    public static void skipOrCompleteSeq(Image cutsceneImg)
    {
        if (cutsceneIsPlaying)
        {
            if (!cutsceneSeq.IsPlaying())
            {
                if (currentCSSO.cutsceneSprites.IndexOf(cutsceneImg.sprite) < currentCSSO.cutsceneSprites.Count - 1)
                {
                    cutsceneSeq.timeScale = 1.0f;
                    setImg(currentCSSO.cutsceneSprites, cutsceneImg.sprite, cutsceneImg);
                }
                cutsceneIsPlaying = false;
                cutsceneSeq.Play();
            }
            else
            {
                cutsceneSeq.timeScale = 25.0f;
            }
        }


    }
    // 성공 시 연출 (컷씬 보여주기)
    public static void setImg(List<Sprite> spriteList, Sprite currentSprite, Image cutsceneImg)
    {
        //yield return new WaitForSeconds(0.25f);
        Sprite s = spriteList[spriteList.IndexOf(currentSprite) + 1];
        cutsceneImg.sprite = s;
    }
}
