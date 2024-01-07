using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GetDataWithID : MonoBehaviour
{
    [Header("*Manager")]
    [SerializeField] WordManager WordManager;

    [Header("*GO")]
    [SerializeField] GameObject Popup;

    [Header("*TMP_Text")]
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Extension;
    [HideInInspector] public string id;
    [HideInInspector] public string type;

    public void SetData(string _id)
    {
        id = _id;
        char[] c_id = id.ToCharArray();
        if (c_id[0] == 'W')
        {
            if (c_id[1] == 'A')
            {
                setUI((string)DataManager.WordActionDatas[3][id], ".EXE");

                WordManager.currentWordActionIDList.Add(id);
            }
            else
            {
                setUI((string)DataManager.WordDatas[5][id], ".AIL");

                WordManager.currentWordIDList.Add(id);
            }
        }
    }

    private void setUI(string name, string type)
    {
        DOTween.Kill("GetDataWithID");
        Name.text = name;
        Extension.text = type;
        DOTween.To(() => Popup.GetComponent<RectTransform>().anchoredPosition, x => Popup.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(-20, Popup.GetComponent<RectTransform>().anchoredPosition.y), 1)
            .OnComplete(() =>
            {
                DOTween.To(() => Popup.GetComponent<RectTransform>().anchoredPosition, x => Popup.GetComponent<RectTransform>().anchoredPosition = x, new Vector2(1000, Popup.GetComponent<RectTransform>().anchoredPosition.y), 1)
                .SetDelay(2.5f)
                .SetId("GetDataWithID");
            })
            .SetId("GetDataWithID");
    }


    private void clearCurrentGetID()
    {
        id = null;
        type = null;
        Name.text = null;
        Extension.text = null;
    }
}