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

    [Header("* SO")]
    [SerializeField] List<PreliminarySurveySO> AllPreliminarySurveySOs;
    [SerializeField] List<PreliminarySurveySO> UnavailablePreliminarySurveySOs;
    public List<string> CPSSO_IDs;
    [SerializeField] public List<PreliminarySurveySO> AvailablePreliminarySurveySOs;

    #endregion

    #region Func

    public void ft_setAPSSOs()
    {
        AvailablePreliminarySurveySOs = AllPreliminarySurveySOs;
        AvailablePreliminarySurveySOs = ft_removeCPSSOs(AvailablePreliminarySurveySOs);
        AvailablePreliminarySurveySOs = ft_removeSpecialCase(AvailablePreliminarySurveySOs);
    }

    private List<PreliminarySurveySO> ft_removeCPSSOs(List<PreliminarySurveySO> APSSOs)
    {
        List<PreliminarySurveySO> removeSOs = new List<PreliminarySurveySO>();
        foreach(string ID in CPSSO_IDs)
        {
            foreach(PreliminarySurveySO SO in AvailablePreliminarySurveySOs)
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
    private List<PreliminarySurveySO> ft_removeSpecialCase(List<PreliminarySurveySO> APSSOs)
    {
        List<PreliminarySurveySO> removeSOs = new List<PreliminarySurveySO>();

        Debug.Log("조건(=이 사전조사가 발생할 수 있는 조건)을 판별할 장소");
        foreach(PreliminarySurveySO SO in APSSOs)
        {
            
            if (SO.name == "PSSO_03" && !WordManager.currentWordIDList.Contains("W005")) // SO.name == "PSSO_03" && (추가할 조건을 넣어야 충족)
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
