using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
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
        cutsceneImg.color = Color.black;
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
                })
                .OnComplete(() =>
                {
                    cutsceneIsPlaying = true;
                    thisSequence.Pause();
                }));
            thisSequence.Append(cutsceneImg.DOColor(Color.white, 0.25f)
                .OnPlay(() =>
                {
                    cutsceneIsPlaying = true;
                    cutsceneTxt.text = "";
                    cutsceneTxt.transform.parent.gameObject.SetActive(false);
                }));
        }

        return thisSequence;
    }

    public static Sequence justImgCutscene(Image cutsceneImg, List<Sprite> cutsceneSprites, float time)
    {
        cutsceneImg.color = Color.black;
        cutsceneImg.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        float eachTime = time/cutsceneSprites.Count;
        cutsceneImg.sprite = cutsceneSprites[0];

        for (int i = 0; i < cutsceneSprites.Count; i++)
        {
            seq.Append(cutsceneImg.DOColor(Color.white, eachTime * 1/6));
            seq.AppendInterval(eachTime* 4/6);
            seq.Append(cutsceneImg.DOColor(Color.white, eachTime * 1/6)
                .OnComplete(() =>
                {
                    int amount = cutsceneSprites.IndexOf(cutsceneImg.sprite);
                    if(amount < cutsceneSprites.Count - 1)
                    {
                        cutsceneImg.sprite = cutsceneSprites[amount + 1];
                    }
                }));
        }
        return seq;
    }
    public static void skipOrCompleteSeq(Image cutsceneImg, TMP_Text cutsceneTxt)
    {
        if (!cutsceneIsPlaying) { return; }

        cutsceneSeq.timeScale = 1.0f;
        if (!cutsceneSeq.IsPlaying())
        {
            if (currentCSSO.cutsceneSprites.IndexOf(cutsceneImg.sprite) < currentCSSO.cutsceneSprites.Count - 1)
            {
                cutsceneTxt.text = "";
                setImg(currentCSSO.cutsceneSprites, cutsceneImg.sprite, cutsceneImg);
            }
            cutsceneSeq.Play();
        }
        else
        {
            cutsceneSeq.timeScale = 100.0f;
        }

        cutsceneIsPlaying = false;
        return;
    }
    // 성공 시 연출 (컷씬 보여주기)
    public static void setImg(List<Sprite> spriteList, Sprite currentSprite, Image cutsceneImg)
    {
        //yield return new WaitForSeconds(0.25f);
        Sprite s = spriteList[spriteList.IndexOf(currentSprite) + 1];
        cutsceneImg.sprite = s;
    }
}
