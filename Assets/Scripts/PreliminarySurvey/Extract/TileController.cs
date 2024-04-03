using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    [SerializeField] PreliminarySurveyWindow_Extract PreliminarySurveyWindow_Extract;
    public int tileHP;
    public Image thisImg;

    public void ft_setSprite(List<Sprite> EachBlockSprite)
    {
        if (thisImg == null) { thisImg = GetComponent<Image>(); }

        if (tileHP == 0)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            thisImg.sprite = EachBlockSprite[tileHP - 1];
            this.gameObject.SetActive(true);
        }
    }

}
