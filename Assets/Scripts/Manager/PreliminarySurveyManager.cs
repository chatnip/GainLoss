using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreliminarySurveyManager : Manager<PreliminarySurveyManager>
{
    #region Value

    [Header("* SO")]
    [SerializeField] PreliminarySurveyWindow PreliminarySurveyWindow;
    [SerializeField] List<PreliminarySurveySO> PreliminarySurveySOs;

    #endregion

    #region Func

    public PreliminarySurveySO ft_startPS()
    {
        //int rand = Random.Range(0, PreliminarySurveySOs.Count);
        //return PreliminarySurveySOs[rand];
        return PreliminarySurveySOs[0];
    }

    #endregion
}
