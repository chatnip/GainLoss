using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PreliminarySurveyManager : Manager<PreliminarySurveyManager>
{
    #region Value

    [Header("*Property")]
    [SerializeField] PreliminarySurveyWindow_FindClue PreliminarySurveyWindow;
    [SerializeField] WordManager WordManager;
    [SerializeField] PlaceManager PlaceManager;

    [Header("*Find Clue")]
    [SerializeField] List<PreliminarySurveySO_FindClue> PSSOs_FindClue_All; // 전체 FindClue
    public List<string> PSSO_FindClue_ExceptionIDs = new List<string>(); // 이미 보상 획득한 PS ID들 (Json)
    [SerializeField] public List<PreliminarySurveySO_FindClue> PSSOs_FindClue_Available; // 사용 가능한 SO들

    [Header("*Extract")]
    [SerializeField] List<PreliminarySurveySO_FindClue> PSSOs_Extract_All; // 전체 FindClue
    public List<string> PSSO_Extract_ExceptionIDs = new List<string>(); // 이미 보상 획득한 PS ID들 (Json)
    [SerializeField] public List<PreliminarySurveySO_FindClue> PSSOs_Extract_Available; // 사용 가능한 SO들

    #endregion


    #region Find Clue

    public void ft_setAPSSOs_FindClue()
    {
        PSSOs_FindClue_Available = PSSOs_FindClue_All;
        PSSOs_FindClue_Available = ft_removeCPSSOs(PSSOs_FindClue_Available);
        PSSOs_FindClue_Available = ft_removeAlreadyPSSOs(PSSOs_FindClue_Available);
        PSSOs_FindClue_Available = ft_removeSpecialCase(PSSOs_FindClue_Available);
    }

    public PreliminarySurveySO_FindClue ft_startPS_FindClue()
    {
        int random = Random.Range(0, PSSOs_FindClue_Available.Count);
        return PSSOs_FindClue_Available[random];
    }

    #endregion

    #region Extract

    #endregion

    #region Popular in this Class


    // 이미 상호작용하고 보상을 획득한 PSSO 제외
    private List<PreliminarySurveySO_FindClue> ft_removeCPSSOs(List<PreliminarySurveySO_FindClue> APSSOs)
    {
        List<PreliminarySurveySO_FindClue> removeSOs = new List<PreliminarySurveySO_FindClue>();
        foreach (string ID in PSSO_FindClue_ExceptionIDs)
        {
            foreach (PreliminarySurveySO_FindClue SO in APSSOs)
            {
                if (SO.name == ID)
                {
                    removeSOs.Add(SO);
                }
            }
        }
        List<PreliminarySurveySO_FindClue> result = APSSOs.Except(removeSOs).ToList();
        return result;
    }

    // 이미 결과를 가지고 있는 PSSO 제외
    private List<PreliminarySurveySO_FindClue> ft_removeAlreadyPSSOs(List<PreliminarySurveySO_FindClue> APSSOs)
    {
        List<PreliminarySurveySO_FindClue> removeSOs = new List<PreliminarySurveySO_FindClue>();
        List<string> AllIDs = new List<string>();
        AllIDs.AddRange(WordManager.currentWordActionIDList);
        AllIDs.AddRange(WordManager.currentWordIDList);
        AllIDs.AddRange(PlaceManager.currentPlaceID_Dict.Keys);

        foreach (PreliminarySurveySO_FindClue SO in APSSOs)
        {
            if (AllIDs.Contains(SO.getID))
            {
                removeSOs.Add(SO);
            }
        }

        List<PreliminarySurveySO_FindClue> result = APSSOs.Except(removeSOs).ToList();
        return result;
    }

    // 특별 케이스 제외
    private List<PreliminarySurveySO_FindClue> ft_removeSpecialCase(List<PreliminarySurveySO_FindClue> APSSOs)
    {
        List<PreliminarySurveySO_FindClue> removeSOs = new List<PreliminarySurveySO_FindClue>();

        Debug.Log("조건(=이 사전조사가 발생할 수 있는 조건)을 판별할 장소");
        foreach (PreliminarySurveySO_FindClue SO in APSSOs)
        {
            // Example
            if (SO.name == "PSSO_03" && !WordManager.currentWordIDList.Contains("W005"))
            {
                Debug.Log("사전 조사 판별 예시");
                removeSOs.Add(SO);
            }
        }

        List<PreliminarySurveySO_FindClue> result = APSSOs.Except(removeSOs).ToList();
        return result;

    }

    #endregion
}
