using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionButtonGenerator : MonoBehaviour
{
    [Header("*Btn")]
    [SerializeField] Button InteractionBtn;
    [SerializeField] GameObject parentGO;

    List<GameObject> allInteractionBtns = new List<GameObject>();
    List<GameObject> activeInteractionBtns = new List<GameObject>();

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
            if(CanInterationBtn.GetComponent<InteractionBtn>().TargetGO == targetGO)
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
        
        InteractionBtn interactionBtn = btn.GetComponent<InteractionBtn>();
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
            activeInteractionBtns[i].GetComponent<RectTransform>().anchoredPosition = v3_pos;
            activeInteractionBtns[i].SetActive(true);
        }

    }

    

}
