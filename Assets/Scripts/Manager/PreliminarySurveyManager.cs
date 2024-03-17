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

    // �̹� ��ȣ�ۿ��ϰ� ������ ȹ���� PSSO ����
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

    // �̹� ����� ������ �ִ� PSSO ����
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

    // Ư�� ���̽� ����
    private List<PreliminarySurveySO> ft_removeSpecialCase(List<PreliminarySurveySO> APSSOs)
    {
        List<PreliminarySurveySO> removeSOs = new List<PreliminarySurveySO>();

        Debug.Log("����(=�� �������簡 �߻��� �� �ִ� ����)�� �Ǻ��� ���");
        foreach(PreliminarySurveySO SO in APSSOs)
        {
            // Example
            if (SO.name == "PSSO_03" && !WordManager.currentWordIDList.Contains("W005"))
            {
                Debug.Log("���� ���� �Ǻ� ����");
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
