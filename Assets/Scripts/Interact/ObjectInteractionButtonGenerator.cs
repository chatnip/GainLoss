using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionButtonGenerator : MonoBehaviour
{
    [Header("*Btn")]
    [SerializeField] Button InteractionBtn;
    [SerializeField] GameObject parentGO;
    [SerializeField] List<GameObject> leftPoints = new List<GameObject>();
    [SerializeField] List<GameObject> rightPoints = new List<GameObject>();



    List<GameObject> CanInterationBtns = new List<GameObject>();


    public void ObPooling(GameObject targetGO)
    {
        string name = targetGO.name;
        Vector3 pos = targetGO.transform.position;

        bool NeedGen = NeedGenBtn(targetGO);
        if (NeedGen)
        { GenerateBtn(targetGO, pos, name); }
        else
        { SetActiveBtn(targetGO, true); }
    }

    bool NeedGenBtn(GameObject targetGO)
    {
        foreach (GameObject CanInterationBtn in CanInterationBtns) 
        {
            if(CanInterationBtn.GetComponent<InteractionBtn>().TargetGO == targetGO)
            {
                return false;
            }
        }
        return true;
    }
    public void SetActiveBtn(GameObject targetGO, bool set)
    {
        foreach (GameObject CanInterationBtn in CanInterationBtns)
        {
            if (CanInterationBtn.GetComponent<InteractionBtn>().TargetGO == targetGO)
            {
                CanInterationBtn.SetActive(set);
            }
        }
    }
    void GenerateBtn(GameObject targetGO, Vector3 pos, string name)
    {
        GameObject btn = Instantiate(InteractionBtn.gameObject, parentGO.transform);
        btn.name = name + "Btn";
        btn.transform.position = pos;
        btn.transform.localPosition += Vector3.back * 1000;

        Ray ray = new Ray();
        RaycastHit rayHit;
        ray.origin = btn.transform.position;
        ray.direction = btn.transform.forward;
        if(Physics.Raycast(ray.origin, ray.direction, out rayHit, 1000.0f))
        {
            btn.transform.position = rayHit.point;
            btn.transform.localPosition += Vector3.back * 70;
        }

        InteractionBtn interactionBtn = btn.GetComponent<InteractionBtn>();
        interactionBtn.TargetGO = targetGO;
        interactionBtn.txt_name_left.text = name;
        interactionBtn.txt_name_right.text = name;

        LeftOrRight(btn, interactionBtn.txt_name_left, interactionBtn.txt_name_right);

        CanInterationBtns.Add(btn);

        //interactionBtn.SetInput();
    }
    void LeftOrRight(GameObject Btn, TMP_Text left, TMP_Text right)
    {
        float leftDisMin = 10000;
        foreach (GameObject leftPoint in leftPoints)
        {
            if(Vector3.Distance(leftPoint.transform.position, Btn.transform.position) < leftDisMin)
            {
                leftDisMin = Vector3.Distance(leftPoint.transform.position, Btn.transform.position);
            }
        }
        float rightDisMin = 10000;
        foreach (GameObject rightPoint in rightPoints)
        {
            if (Vector3.Distance(rightPoint.transform.position, Btn.transform.position) < rightDisMin)
            {
                rightDisMin = Vector3.Distance(rightPoint.transform.position, Btn.transform.position);
            }
        }

        if (leftDisMin < rightDisMin)
        {
            right.gameObject.SetActive(true);
            left.gameObject.SetActive(false);
        }
        else
        {
            right.gameObject.SetActive(false);
            left.gameObject.SetActive(true);
        }
    }
}
