using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractObjectBtn : InteractCore
{
    [Header("*Manager")]
    //[SerializeField] GameSystem GameSystem;
    [SerializeField] PlayerInputController PlayerInputController;

    [Header("*Set")]
    [SerializeField] public TMP_Text txt_name_left;
    [SerializeField] public TMP_Text txt_name_right;

    [SerializeField] public GameObject TargetGO;
    [SerializeField] public Button thisBtn;
    //[SerializeField] Button InteractiveBtn_comp;

    public void Awake()
    {
        //InteractiveBtn_comp = GetComponent<Button>();
        //GameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        PlayerInputController = GameObject.Find("PlayerInputController").GetComponent<PlayerInputController>();
        thisBtn = this.gameObject.GetComponent<Button>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        PlayerInputController.SelectBtn = thisBtn;
        PlayerInputController.OnOffSelectedBtn(PlayerInputController.SelectBtn);

        // Base
        thisBtn.TryGetComponent(out RectTransform RT);
        DOTween.Kill(RT.localScale);
        RT.DOScale(Vector3.one * 1.1f, 0.1f);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        // Base
        thisBtn.TryGetComponent(out RectTransform RT);
        DOTween.Kill(RT.localScale);
        RT.DOScale(Vector3.one * 1.0f, 0.1f);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(thisBtn.interactable)
        {
            base.OnPointerDown(eventData);
            interactObject();
        }
    }
    public void interactObject()
    {
        if (TargetGO.TryGetComponent(out InteractObject interactObject)) { interactObject.Interact(); }
    }

}
