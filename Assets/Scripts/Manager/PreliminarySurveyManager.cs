using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PreliminarySurveyManager : Manager<PreliminarySurveyManager>
{
    #region Value

    [Header("*Property")]
    [SerializeField] PreliminarySurveyWindow PreliminarySurveyWindow;
    [SerializeField] WordManager WordManager;
    [SerializeField] PlaceManager PlaceManager;

    [Header("* SO")]
    [SerializeField] List<PreliminarySurveySO> AllPreliminarySurveySOs;
    [SerializeField] List<PreliminarySurveySO> UnavailablePreliminarySurveySOs;
    public List<string> ExceptionPSSO_IDs;
    [SerializeField] public List<PreliminarySurveySO> AvailablePreliminarySurveySOs;

    #endregion

    #region Func

    public void ft_setAPSSOs()
    {
        AvailablePreliminarySurveySOs = AllPreliminarySurveySOs;
        AvailablePreliminarySurveySOs = ft_removeCPSSOs(AvailablePreliminarySurveySOs);
        AvailablePreliminarySurveySOs = ft_removeAlreadyPSSOs(AvailablePreliminarySurveySOs);
        AvailablePreliminarySurveySOs = ft_removeSpecialCase(AvailablePreliminarySurveySOs);
        UnavailablePreliminarySurveySOs = AllPreliminarySurveySOs.Except(AvailablePreliminarySurveySOs).ToList();
    }

    // 이미 상호작용하고 보상을 획득한 PSSO 제외
    private List<PreliminarySurveySO> ft_removeCPSSOs(List<PreliminarySurveySO> APSSOs)
    {
        List<PreliminarySurveySO> removeSOs = new List<PreliminarySurveySO>();
        foreach(string ID in ExceptionPSSO_IDs)
        {
            foreach(PreliminarySurveySO SO in APSSOs)
            {
                if(SO.name == ID)
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
        AllIDs.AddRange(PlaceManager.currentPlaceIDList);

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

    // 특별 케이스 제외
    private List<PreliminarySurveySO> ft_removeSpecialCase(List<PreliminarySurveySO> APSSOs)
    {
        List<PreliminarySurveySO> removeSOs = new List<PreliminarySurveySO>();

        Debug.Log("조건(=이 사전조사가 발생할 수 있는 조건)을 판별할 장소");
        foreach(PreliminarySurveySO SO in APSSOs)
        {
            // Example
            if (SO.name == "PSSO_03" && !WordManager.currentWordIDList.Contains("W005"))
            {
                Debug.Log("사전 조사 판별 예시");
                removeSOs.Add(SO);
            }
        }

        List<PreliminarySurveySO> result = APSSOs.Except(removeSOs).ToList();
        return result;
        
    }

    public PreliminarySurveySO ft_startPS()
    {
        int random = Random.Range(0, AvailablePreliminarySurveySOs.Count);
        return AvailablePreliminarySurveySOs[random];
    }

    #endregion
}
