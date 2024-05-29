//Refactoring v1.0
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    #region Value

    [Header("=== CSV File")]
    [Header("-- About Entire Play")]
    [SerializeField] TextAsset ChapterCSV;
    [SerializeField] TextAsset AbilityCSV;
    public List<Dictionary<string, object>> ChapterCSVDatas = new();
    public List<Dictionary<string, object>> AbilityCSVDatas = new();


    [Header("-- About Place")]
    [SerializeField] TextAsset PlaceCSV;
    [SerializeField] TextAsset ObjectCSV;
    [SerializeField] TextAsset ObjectChoiceCSV;
    public List<Dictionary<string, object>> PlaceCSVDatas = new();
    public List<Dictionary<string, object>> ObjectCSVDatas = new();
    public List<Dictionary<string, object>> ObjectChoiceCSVDatas = new();

    [Header("-- About Desktop App")]
    [SerializeField] TextAsset DesktopAppCSV;
    public List<Dictionary<string, object>> DesktopAppCSVDatas = new();


    [Header("-- About Language")]
    [SerializeField] TextAsset LanguageCSV;
    [SerializeField] TextAsset StaticTextCSV; 
    public List<Dictionary<string, object>> LanguageCSVDatas = new();
    public List<Dictionary<string, object>> StaticTextCSVDatas = new();

    // Other Data




    #endregion

    #region Framework & Base Set

    protected override void Awake()
    {
        Offset_ReadData_from_CSV();
        base.Awake();
    }

    public void Offset_ReadData_from_CSV()
    {
        // amount = LanguageManager.Instance.LanguageTypeAmount

        #region Entire Play

        // (amount*0) + 0~2. Eng / Kor / Jpn 
        // (amount*1) + 0~2. VisitReason Eng / Kor / Jpn
        // (amount*2) + 0~2. Get Activitiy Each Day / Start Day / End Day
        // (amount*3) + 0~2. Default Observational / Persuasive / MentalStength
        // (amount*4) + 0. Visitable Place IDs / + 1. Interactable Object IDs

        ChapterCSVDatas = CSVReader.Read(this.ChapterCSV);


        // 0~2. Eng / Kor / Jpn

        AbilityCSVDatas = CSVReader.Read(this.AbilityCSV);

        #endregion

        #region Place

        // 0~2. Eng / Kor / Jpn

        PlaceCSVDatas = CSVReader.Read(this.PlaceCSV);


        // (amount*0) + 0~2. Eng / Kor / Jpn
        // (amount*1) + 0~2. Defualt Text Eng / Kor / Jpn
        // (amount*2) + 0~2. Defualt Name Eng / Kor / Jpn
        // (amount*3) + 0~2. Extra Text Eng / Kor / Jpn 
        // (amount*4) + 0~2. Extra Name Eng / Kor / Jpn 
        // (amount*5) + 0. Defualt Img ID / 1. Extra Img ID 
        // (amount*5) + 2. Defualt Anim ID / 3. Extra Anim ID 

        ObjectCSVDatas = CSVReader.Read(this.ObjectCSV);


        // (amount*0) + 0~2. Eng / Kor / Jpn
        // (amount*1) + 0~2. AnswerDesc Eng / Kor / Jpn
        // (amount*2) + 0~2. AnswerName Eng / Kor / Jpn
        // (amount*3) + 0~2. Need Observational / Persuasive / MentalStrength
        // (amount*4) + 0. GetContents / 1. ImgID / 2. Anim ID

        ObjectChoiceCSVDatas = CSVReader.Read(this.ObjectChoiceCSV);

        #endregion

        #region Desktop App

        // (amount*0) + 0~2. Eng / Kor / Jpn
        // (amount*1) + 0~2. Confirm Eng / Kor / Jpn
        DesktopAppCSVDatas = CSVReader.Read(this.DesktopAppCSV);

        #endregion

        #region Language

        // 0. Num

        LanguageCSVDatas = CSVReader.Read(this.LanguageCSV);


        // 0 -> Length (It have one's turn)

        StaticTextCSVDatas = CSVReader.Read(this.StaticTextCSV);

        #endregion

    }



    #endregion

}
