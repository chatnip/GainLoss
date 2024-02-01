using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class ObjectInteractionButtonGenerator : MonoBehaviour, IInteract
{
    #region Value

    [Header("*Property")]
    [SerializeField] public PlayerInputController PlayerInputController;
    [SerializeField] SetInteractionObjects SetInteractionObjects;
    [SerializeField] GameObject phone2D;
    [SerializeField] GameObject computer2D;

    [Header("*Component")]
    [SerializeField] CanvasScaler thisScaler;
    [SerializeField] CanvasGroup thisGroup;

    [Header("*Btn")]
    [SerializeField] Button InteractionBtn;
    [SerializeField] GameObject parentGO;

    List<GameObject> allInteractionBtns = new List<GameObject>();
    List<GameObject> activeInteractionBtns = new List<GameObject>();

    [HideInInspector] public bool SectionIsThis = false;
    

    #endregion

    #region Btns

    public void ObPooling(GameObject targetGO, List<GameObject> activeGOs)
    {
        if (NeedGenBtn(targetGO)) { GenBtn(targetGO); }
        SetActiveBtns(activeGOs);
    }

    // 새로 생성해야하는지 판별
    bool NeedGenBtn(GameObject targetGO)
    {
        foreach (GameObject CanInterationBtn in allInteractionBtns) 
        {
            if(CanInterationBtn.GetComponent<InteractObjectBtn>().TargetGO == targetGO)
            {
                return false;
            }
        }
        return true;
    }
    
    // 없다면 생성
    void GenBtn(GameObject targetGO)
    {
        GameObject btn = Instantiate(InteractionBtn.gameObject, parentGO.transform);
        btn.SetActive(false);
        btn.name = targetGO.name + "Btn";
        
        InteractObjectBtn interactionBtn = btn.GetComponent<InteractObjectBtn>();
        interactionBtn.TargetGO = targetGO;
        interactionBtn.txt_name_left.text = targetGO.name;

        allInteractionBtns.Add(btn);
    }

    // 활성화 버튼 판별 및 적용
    public void SetActiveBtns(List<GameObject> activeGOs)
    {
        for (int i = 0; i < activeInteractionBtns.Count; i++)
        {
            activeInteractionBtns[i].SetActive(false);
        }
        activeInteractionBtns.Clear();
        foreach (GameObject Btn in allInteractionBtns)
        {
            foreach (GameObject activeGO in activeGOs)
            {
                if (activeGO.name + "Btn" == Btn.name)
                {
                    activeInteractionBtns.Add(Btn);
                }
            }
        }

        Vector3 v3_pos;
        for (int i = 0; i < activeInteractionBtns.Count; i++)
        {
            v3_pos = new Vector3(0, i * InteractionBtn.GetComponent<RectTransform>().rect.height, 0);
            activeInteractionBtns[activeInteractionBtns.Count - i - 1].GetComponent<RectTransform>().anchoredPosition = v3_pos;
            activeInteractionBtns[activeInteractionBtns.Count - i - 1].SetActive(true);
        }

    }

    public void SetOnOffAllBtns(bool OnOff)
    {
        foreach(GameObject Btn in allInteractionBtns) 
        {
            if(Btn.TryGetComponent(out Button buttonComp))
            {
                buttonComp.interactable = OnOff;
            }
            if (Btn.TryGetComponent(out CanvasGroup CG))
            {
                if (OnOff) { CG.alpha = 1.0f; }
                else { CG.alpha = 0.3f; }
            }
        }
    }

    #endregion
    
    #region Interact
    public void SetOnOffInteractObjectBtn()
    {
        if (!phone2D.activeSelf && !computer2D.activeSelf)
        {
            // 오브젝트 상호작용으로 변경
            if (!SectionIsThis)
            {
                PlayerInputController.interact = this;
                SectionIsThis = true;
                PlayerInputController.SetSectionBtns(SetSectionBtns(), this);
                DOTween.To(() => thisScaler.scaleFactor, x => thisScaler.scaleFactor = x, 1f, 0.3f);
                DOTween.To(() => thisGroup.alpha, x => thisGroup.alpha = x, 1f, 0.3f);
            }
            // 오브젝트 상호작용에서 벗어나기
            else
            {
                SetOffAllOutline();
                PlayerInputController.ClearSeletedBtns();
                SectionIsThis = false;
                PlayerInputController.SetSectionBtns(null, null);
                DOTween.To(() => thisScaler.scaleFactor, x => thisScaler.scaleFactor = x, 0.6f, 0.3f);
                DOTween.To(() => thisGroup.alpha, x => thisGroup.alpha = x, 0.6f, 0.3f)
                    .OnComplete(() =>
                    {
                        SetOffAllOutline();
                    });


            }
        }
    }
    public void Interact()
    {
        if(PlayerInputController.SelectBtn != null) 
        { 
            if(PlayerInputController.SelectBtn.TryGetComponent(out InteractObjectBtn interactObjectBtn))
            {
                interactObjectBtn.interactObject();
            }
        }
    }
    public List<Button> SetSectionBtns()
    {
        if(this.transform.GetChild(0) != null)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>();
            List<Button> btns = new List<Button>();
            foreach (Transform child in allChildren)
            {
                if (child.TryGetComponent(out Button btn))
                {
                    btns.Add(btn);
                }
            }

            return btns;
        }
        else
        {
            return null;
        }
    }

    public void SetOffAllOutline()
    {
        foreach (GameObject child in allInteractionBtns)
        {
            if (child.TryGetComponent(out UnityEngine.UI.Outline childOutline))
            {
                childOutline.enabled = false;
            }
        }
    }

    #endregion

}
