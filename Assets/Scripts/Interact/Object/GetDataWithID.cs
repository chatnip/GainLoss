using UnityEngine;
using TMPro;
using DG.Tweening;

public class GetDataWithID : MonoBehaviour
{
    [Header("*Manager")]
    [SerializeField] WordManager WordManager;
    [SerializeField] PlaceManager PlaceManager;

    [Header("*GO")]
    [SerializeField] GameObject Popup;

    [Header("*TMP_Text")]
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Extension;
    [HideInInspector] public string id;
    [HideInInspector] public string type;

    public void SetData(string _id)
    {
        if (_id == "" || _id == null) { return; }
        id = _id;
        char[] c_id = id.ToCharArray();
        if (c_id[0] == 'W')
        {
            if (c_id[1] == 'A')
            {
                setUI(DataManager.WordActionDatas[3][id].ToString(), ".EXE");
                WordManager.currentWordActionIDList.Add(id);
            }
            else
            {
                setUI(DataManager.WordDatas[5][id].ToString(), ".AIL");
                WordManager.currentWordIDList.Add(id);
            }
        }
        else if (c_id[0] == 'P')
        {
            setUI(DataManager.PlaceDatas[1][id].ToString(), "(Place)");
            PlaceManager.currentPlaceID_Dict.Add(id, 0);
        }
    }

    private void setUI(string name, string type)
    {
        Popup.gameObject.SetActive(false);
        DOTween.Complete(this.gameObject.GetComponent<RectTransform>());
        Name.text = name;
        Extension.text = type;
        Popup.gameObject.SetActive(true);
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