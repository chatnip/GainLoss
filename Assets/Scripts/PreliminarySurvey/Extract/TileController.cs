using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    [SerializeField] PreliminarySurveyWindow_Extract PreliminarySurveyWindow_Extract;
    public int tileHP;
    public Image thisImg;
    public BoxCollider2D thisColl;
    public RectTransform thisRT;

    public void ft_setSprite(List<Sprite> EachBlockSprite)
    {
        if (thisImg == null) { thisImg = GetComponent<Image>(); }
        if (thisColl == null) { thisColl = GetComponent<BoxCollider2D>(); }
        if (thisRT == null) { thisRT = GetComponent<RectTransform>(); }

        if (tileHP == 0)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            thisColl.enabled = true;
            thisImg.sprite = EachBlockSprite[tileHP - 1];
            this.gameObject.SetActive(true);
        }
    }

    public void ft_hitted(List<Sprite> EachBlockSprite, List<Sprite> EachBlockEffectfulSprite)
    {
        tileHP--;
        if(tileHP == 0)
        {
            thisColl.enabled = false;

            if (DOTween.IsTweening(gameObject)) { DOTween.Complete(gameObject); }

            Sequence seq = DOTween.Sequence();
            thisImg.sprite = EachBlockEffectfulSprite[tileHP];
            seq.Append(thisRT.DOShakeRotation(0.25f, new Vector3(0f, 0f, 10f), 6));
            seq.Join(thisRT.DOSizeDelta(PreliminarySurveyWindow_Extract.blockSizeDelta * 1.1f, 0.25f));
            seq.Join(thisImg.DOFade(0, 0.25f));

            seq.OnComplete(() =>
            {
                PreliminarySurveyWindow_Extract.ft_getGage();

                GameObject Effectful = this.transform.GetChild(0).gameObject;
                Effectful.SetActive(true);
                Effectful.TryGetComponent(out RectTransform EffectfulRT);
                Effectful.TryGetComponent(out Image EffectfulImg);

                Sequence seq2 = DOTween.Sequence();
                seq2.Append(EffectfulRT.DOScale(Vector2.one * 2, 0.25f));
                seq2.Join(EffectfulImg.DOFade(0, 0.25f));

                seq2.OnComplete(() =>
                {
                    thisRT.rotation = Quaternion.Euler(Vector3.zero);
                    thisRT.sizeDelta = PreliminarySurveyWindow_Extract.blockSizeDelta;
                    thisImg.color = Color.white;

                    EffectfulRT.localScale = Vector3.one;
                    EffectfulImg.color = Color.white;

                    Effectful.gameObject.SetActive(false);
                    this.gameObject.SetActive(false);
                });

            });
        }
        else
        {
            if (DOTween.IsTweening(gameObject)) { DOTween.Complete(gameObject); }

            Sequence seq = DOTween.Sequence();
            thisImg.sprite = EachBlockEffectfulSprite[tileHP];
            seq.Append(thisRT.DOShakeRotation(0.25f, new Vector3(0f, 0f, 10f), 6)
                .OnComplete(() =>
                {
                    thisRT.rotation = Quaternion.Euler(Vector3.zero);
                }));
            seq.Join(thisRT.DOSizeDelta(PreliminarySurveyWindow_Extract.blockSizeDelta * 1.1f, 0.25f)
                .OnComplete(() =>
                {
                    thisImg.sprite = EachBlockEffectfulSprite[tileHP - 1];
                }));
            seq.Append(thisRT.DOSizeDelta(PreliminarySurveyWindow_Extract.blockSizeDelta, 0.25f)
                .OnComplete(() =>
                {
                    thisImg.sprite = EachBlockSprite[tileHP - 1];
                }));
        }

    }

}
