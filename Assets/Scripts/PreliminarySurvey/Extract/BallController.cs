using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    #region Value

    [SerializeField] PreliminarySurveyWindow_Extract PreliminarySurveyWindow_Extract;
    [SerializeField] RectTransform thisRT;
    [SerializeField] CircleCollider2D thisCollider;
    [SerializeField] public Rigidbody2D thisRb;
    [SerializeField] [Range(100f, 3000f)] float shotSpeed;

    [SerializeField] public Image thisImg;
    [SerializeField] Image Effectful;
    float randomX;

    #endregion

    #region Main

    private void Awake()
    {
        if (thisRb == null) { thisRb = GetComponent<Rigidbody2D>(); }
        if (thisRT == null) { thisRT = GetComponent<RectTransform>(); }
    }

    #endregion

    #region Collision

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "BreakBall")
        {
            ft_breakBall();
        }
        else if(other.gameObject.tag == "BounceBall_Tile")
        {
            ft_HitTile(other.gameObject);
        }
    }

    private void ft_breakBall()
    {
        thisImg.enabled = false;
        Effectful.gameObject.SetActive(true);
        Effectful.TryGetComponent(out RectTransform RT);

        Sequence seq = DOTween.Sequence();


        thisRb.velocity = Vector3.zero;
        Effectful.color = Color.white;
        RT.localScale = Vector3.one;

        seq.Append(Effectful.DOFade(0, 0.5f));
        seq.Join(RT.DOScale(3f, 0.5f));
        seq.AppendInterval(0.5f);

        seq.OnComplete(() =>
        {
            Effectful.gameObject.SetActive(false);
            PreliminarySurveyWindow_Extract.ft_readySetGo();
        });

    }

    private void ft_HitTile(GameObject Tile)
    {
        Tile.TryGetComponent(out TileController TC);
        if(TC.thisImg.sprite == PreliminarySurveyWindow_Extract.eachBlockSprite[PreliminarySurveyWindow_Extract.eachBlockSprite.Count - 1])
        {
            return;
        }

        TC.tileHP--;
        if(TC.tileHP == 0)
        {
            PreliminarySurveyWindow_Extract.ft_getGage();
        }
        TC.ft_setSprite(PreliminarySurveyWindow_Extract.eachBlockSprite);
    }
    #endregion

    #region Set & Shot

    public void ft_resetPos()
    {
        thisImg.enabled = true;
        thisImg.color = Color.white;
        thisRb.velocity = Vector3.zero;
        thisRT.anchoredPosition = new Vector2(0f, -250f);
    }
    public void ft_shotBall()
    {
        randomX = Random.Range(-1f, 1f);

        Vector2 dir = new Vector2(randomX, 1).normalized;
        thisRb.AddForce(dir * shotSpeed);
    }

    #endregion
}
