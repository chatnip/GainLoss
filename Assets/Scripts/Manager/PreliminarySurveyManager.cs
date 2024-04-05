using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreliminarySurveyManager : Manager<PreliminarySurveyManager>
{
    #region Value

    [Header("*Property")]
    [SerializeField] PreliminarySurveyWindow_FindClue PreliminarySurveyWindow;
    [SerializeField] WordManager WordManager;
    [SerializeField] PlaceManager PlaceManager;
    [SerializeField] GameManager GameManager;

    [Header("*Find Clue")]
    [SerializeField] List<PreliminarySurveySO> PSSOs_FindClue_All; // 전체 FindClue
    public List<string> PSSO_FindClue_ExceptionIDs = new List<string>(); // 이미 보상 획득한 PS ID들 (Json)
    [SerializeField] public List<PreliminarySurveySO> PSSOs_FindClue_Available; // 사용 가능한 SO들

    [Header("*Extract")]
    [SerializeField] List<PreliminarySurveySO> PSSOs_Extract_All; // 전체 FindClue
    public List<string> PSSO_Extract_ExceptionIDs = new List<string>(); // 이미 보상 획득한 PS ID들 (Json)
    [SerializeField] public List<PreliminarySurveySO> PSSOs_Extract_Available; // 사용 가능한 SO들

    #endregion

    #region Set Data

    public void ft_setAPSSOs()
    {
        ft_setAPSSOs_FindClue();
        ft_setAPSSOs_Extract();
    }
    private void ft_setAPSSOs_FindClue()
    {
        PSSOs_FindClue_Available = PSSOs_FindClue_All;
        PSSOs_FindClue_Available = ft_removeCPSSOs(PSSOs_FindClue_Available);
        PSSOs_FindClue_Available = ft_removeAlreadyPSSOs(PSSOs_FindClue_Available);
        PSSOs_FindClue_Available = ft_removeSpecialCase(PSSOs_FindClue_Available);
    }
    private void ft_setAPSSOs_Extract()
    {
        PSSOs_Extract_Available = PSSOs_Extract_All;
        PSSOs_Extract_Available = ft_removeCPSSOs(PSSOs_Extract_Available);
        PSSOs_Extract_Available = ft_removeAlreadyPSSOs(PSSOs_Extract_Available);
        PSSOs_Extract_Available = ft_removeSpecialCase(PSSOs_Extract_Available);
    }

    #endregion

    #region Find Clue

    public PreliminarySurveySO_FindClue ft_startPS_FindClue()
    {
        int random = Random.Range(0, PSSOs_FindClue_Available.Count);
        return (PreliminarySurveySO_FindClue)PSSOs_FindClue_Available[random];
    }

    #endregion

    #region Extract

    public PreliminarySurveySO_Extract ft_startPS_Extract()
    {
        int random = Random.Range(0, PSSOs_Extract_Available.Count);
        return (PreliminarySurveySO_Extract)PSSOs_Extract_Available[random];
    }

    #endregion

    #region Popular in this Class


    // 이미 상호작용하고 보상을 획득한 PSSO 제외
    private List<PreliminarySurveySO> ft_removeCPSSOs(List<PreliminarySurveySO> APSSOs)
    {
        List<PreliminarySurveySO> removeSOs = new List<PreliminarySurveySO>();
        foreach (string ID in PSSO_FindClue_ExceptionIDs)
        {
            foreach (PreliminarySurveySO SO in APSSOs)
            {
                if (SO.name == ID)
                {
                    removeSOs.Add(SO);
                }
            }
        }
        List<PreliminarySurveySO> result = APSSOs.Except(removeSOs).ToList();
        return result;
    }

    // 이미 결과를 가지고 있는 PSSO 제외
    private List<PreliminarySurveySO> ft_removeAlreadyPSSOs(List<PreliminarySurveySO> APSSOs)
    {
        List<PreliminarySurveySO> removeSOs = new List<PreliminarySurveySO>();
        List<string> AllIDs = new List<string>();
        AllIDs.AddRange(WordManager.currentWordActionIDList);
        AllIDs.AddRange(WordManager.currentWordIDList);
        AllIDs.AddRange(PlaceManager.currentPlaceID_Dict.Keys);

        foreach (PreliminarySurveySO SO in APSSOs)
        {
            if (AllIDs.Contains(SO.getID))
            {
                removeSOs.Add(SO);
            }
        }

        List<PreliminarySurveySO> result = APSSOs.Except(removeSOs).ToList();
        return result;
    }

    // 과부하 기준에 충족하는지
    private List<PreliminarySurveySO> ft_removeSpecialCase(List<PreliminarySurveySO> APSSOs)
    {
        List<PreliminarySurveySO> removeSOs = new List<PreliminarySurveySO>();

        foreach (PreliminarySurveySO SO in APSSOs)
        {
            if (GameManager.currentMainInfo.overloadGage < SO.conditionToOverload)
            {
                removeSOs.Add(SO);
            }
        }

        List<PreliminarySurveySO> result = APSSOs.Except(removeSOs).ToList();
        return result;

    }

    #endregion
}
