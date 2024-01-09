using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class CheckGetAllDatas : MonoBehaviour
{
    [SerializeField] GameObject CurrentMap;
    [SerializeField] TMP_Text Info;
    [SerializeField] Button TerminateBtn;


    private void Awake()
    {
        TerminateBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                Debug.Log("���� ��Ʈ�� �Ѿ�� �Ǵ� ���ϱ�");
            });
    }

    private void OnEnable()
    {
        ApplyTerminateBtnAndText();
    }

    public void ApplyTerminateBtnAndText()
    {
        if (checkGetAllDatas(CurrentMap) <= 0)
        {
            Info.text = "Terminate this Part";
            TerminateBtn.gameObject.SetActive(true);
        }
        else
        {
            Info.text = checkGetAllDatas(CurrentMap) + " Remain(s)";
            TerminateBtn.gameObject.SetActive(false);
        }
    }

    private int checkGetAllDatas(GameObject OBparentMap)
    {
        int _remain = 0;

        Transform[] allChildren = OBparentMap.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.TryGetComponent(out InteractObject interactObject))
            {
                if (interactObject.getWordID != "" || interactObject.getWordActionID != "")
                {
                    _remain++;
                    //Debug.Log("���� ����");
                }
            }
        }

        return _remain;
    }

}