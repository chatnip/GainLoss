using DG.Tweening;
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


    public static bool cutsceneIsPlaying = false;
    public static Sequence makeCutscene(cutsceneSO CSSO, Image cutsceneImg, TMP_Text cutsceneTxt)
    {
        cutsceneImg.color = new Color(0, 0, 0, 0);
        cutsceneImg.gameObject.SetActive(true);
        cutsceneTxt.text = "";

        Sequence thisSequence = DOTween.Sequence();
        thisSequence.timeScale = 1.0f;
        cutsceneImg.sprite = CSSO.cutsceneSprites[0];

        thisSequence.Append(cutsceneImg.DOFade(1.0f, 0.25f));
        for (int i = 0; i < CSSO.cutsceneSprites.Count; i++)
        {
            thisSequence.Append(cutsceneImg.DOColor(Color.white, 0.25f));
            thisSequence.Append(cutsceneTxt.DOText(CSSO.cutsceneDialogs[i], 2.0f)
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
}
