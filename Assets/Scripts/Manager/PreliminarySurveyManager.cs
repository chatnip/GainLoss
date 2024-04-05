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
    [SerializeField] List<PreliminarySurveySO> PSSOs_FindClue_All; // ��ü FindClue
    public List<string> PSSO_FindClue_ExceptionIDs = new List<string>(); // �̹� ���� ȹ���� PS ID�� (Json)
    [SerializeField] public List<PreliminarySurveySO> PSSOs_FindClue_Available; // ��� ������ SO��

    [Header("*Extract")]
    [SerializeField] List<PreliminarySurveySO> PSSOs_Extract_All; // ��ü FindClue
    public List<string> PSSO_Extract_ExceptionIDs = new List<string>(); // �̹� ���� ȹ���� PS ID�� (Json)
    [SerializeField] public List<PreliminarySurveySO> PSSOs_Extract_Available; // ��� ������ SO��

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


    // �̹� ��ȣ�ۿ��ϰ� ������ ȹ���� PSSO ����
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

    // �̹� ����� ������ �ִ� PSSO ����
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

    // ������ ���ؿ� �����ϴ���
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
