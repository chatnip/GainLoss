using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TMP_WordClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Camera uicamera;
    [SerializeField] TMP_Text text;
    [SerializeField] VirtualScreen VirtualScreen;
    public string LastClickedWord;
    /*
    public TextMeshProUGUI text;

    public string LastClickedWord;
    */

    public void OnPointerClick(PointerEventData eventData)
    {
        var wordIndex = TMP_TextUtilities.FindIntersectingWord(text, VirtualScreen.eventdataPos, uicamera);

        if (wordIndex != -1)
        {
            LastClickedWord = text.textInfo.wordInfo[wordIndex].GetWord();

            Debug.Log(LastClickedWord);
        }
    }

    /*
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var wordIndex = TMP_TextUtilities.FindIntersectingWord(text, Input.mousePosition, null);

            if (wordIndex != -1)
            {
                LastClickedWord = text.textInfo.wordInfo[wordIndex].GetWord();

                Debug.Log("Clicked on " + LastClickedWord);
            }
        }
    }
    */
}
