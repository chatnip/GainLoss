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
    [SerializeField] TextAsset StreamCSV;
    [SerializeField] TextAsset StreamModuleCSV;
    public List<Dictionary<string, object>> DesktopAppCSVDatas = new();
    public List<Dictionary<string, object>> StreamCSVDatas = new();
    public List<Dictionary<string, object>> StreamModuleCSVDatas = new();

    [Header("-- About Phone App")]
    [SerializeField] TextAsset PhoneOptionAppCSV;
    public List<Dictionary<string, object>> PhoneOptionAppCSVDatas = new();

    [Header("-- About Language")]
    [SerializeField] TextAsset LanguageCSV;
    [SerializeField] TextAsset StaticTextCSV; 
    public List<Dictionary<string, object>> LanguageCSVDatas = new();
    public List<Dictionary<string, object>> StaticTextCSVDatas = new();

    
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
        // (amount*2) + 3~5. Default Observational / Persuasive / MentalStength
        // (amount*2) + 6. Visitable Place IDs / + 7. Interactable Object IDs / + 8. Set Streaming IDs

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
        // (amount*5) + 0. Defualt Img ID / 1. Extra Img ID / 2. Defualt Anim ID / 3. Extra Anim ID  

        ObjectCSVDatas = CSVReader.Read(this.ObjectCSV);


        // (amount*0) + 0~2. Eng / Kor / Jpn
        // (amount*1) + 0~2. AnswerDesc Eng / Kor / Jpn
        // (amount*2) + 0~2. AnswerName Eng / Kor / Jpn
        // (amount*3) + 0~2. Need Observational / Persuasive / MentalStrength
        // (amount*3) + 3. GetContents / 4. ImgID / 5. Anim ID / 6. Set Stream Quarter(MxxQxx)

        ObjectChoiceCSVDatas = CSVReader.Read(this.ObjectChoiceCSV);

        #endregion

        #region Desktop App

        // (amount*0) + 0~2. Eng / Kor / Jpn
        // (amount*1) + 0~2. Confirm Eng / Kor / Jpn

        DesktopAppCSVDatas = CSVReader.Read(this.DesktopAppCSV);

        // (amount*0) + 0~2. Title Eng / Kor / Jpn
        // (amount*1) + 0~2. ResultTextByGetTypeKind Eng / Kor / Jpn
        // (amount*2) + 0. Icon / 1. Get TypeKind / 2~ Min-Max-ID

        StreamCSVDatas = CSVReader.Read(this.StreamCSV);


        // (amount*0) + 0~2. Choice Eng / Kor / Jpn
        // (amount*1) + 0~2. Dialog Eng / Kor / Jpn
        // (amount*2) + 0. AnimeID / 1. Left or Right (Left = false | Right = true) / 2. Get Gage Value

        StreamModuleCSVDatas = CSVReader.Read(this.StreamModuleCSV);

        #endregion

        #region Phone App

        // (amount*0) + 0~2. Choice Eng / Kor / Jpn

        PhoneOptionAppCSVDatas = CSVReader.Read(this.PhoneOptionAppCSV);

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
