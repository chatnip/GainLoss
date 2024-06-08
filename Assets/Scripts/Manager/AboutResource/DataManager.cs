//Refactoring v1.0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

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

    [Header("-- About Reasoning")]
    [SerializeField] TextAsset ReasoningContentCSV;
    public List<Dictionary<string, object>> ReasoningContentCSVDatas = new();

    [Header("-- About Language")]
    [SerializeField] TextAsset LanguageCSV;
    [SerializeField] TextAsset StaticTextCSV; 
    public List<Dictionary<string, object>> LanguageCSVDatas = new();
    public List<Dictionary<string, object>> StaticTextCSVDatas = new();
    [Space(50)]

    #endregion

    #region New CSV Data

    [SerializeField] TextAsset Location_CSV;
    [SerializeField] TextAsset HomeObject_CSV;
    [SerializeField] TextAsset Object_CSV;

    #endregion

    #region Other
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static int dataReadLine = 3;
    #endregion

    #region Framework & Base Set

    protected override void Awake()
    {
        base.Awake();
        //Offset_ReadData_from_CSV();
    }

    private void Offset_ReadData_from_CSV()
    {
        // amount = LanguageManager.Instance.LanguageTypeAmount

        #region Entire Play

        // (amount*0) + 0~2. Eng / Kor / Jpn 
        // (amount*1) + 0~2. ???-VisitReason Eng / Kor / Jpn
        // (amount*2) + 0~2. Get Activitiy Each Day / Start Day / End Day
        // (amount*2) + 3~5. ???-Default Observational / ???-Persuasive / ???-MentalStength
        // (amount*2) + 6. ???-Visitable Place IDs / 7. Interactable Object IDs / 8. Set Streaming IDs / 9. ReasoningID

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

        #region Reasoning

        // (amount*0) + 0~2. Choice Eng / Kor / Jpn

        ReasoningContentCSVDatas = CSVReader.Read(ReasoningContentCSV);

        #endregion

        #region Language

        // 0. Num

        LanguageCSVDatas = CSVReader.Read(this.LanguageCSV);


        // 0 -> Length (It have one's turn)

        StaticTextCSVDatas = CSVReader.Read(this.StaticTextCSV);

        #endregion

    }

    #endregion

    #region About Location

    // 챕터에 갈 수 있는 모든 장소 ID
    public List<string> Get_AllLocationIDChapter(string chapterID)
    {
        List<string> result = new List<string>();
        string[] lines = Get_lines(Location_CSV);
        int ChapterIndex = Get_Index(lines[dataReadLine], "Chapter");

        int locationIndex = Get_Index(lines[dataReadLine], "Idx_Location");

        foreach(string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if(lineData[ChapterIndex] == chapterID)
            {
                result.Add(lineData[locationIndex]);
            }
        }
        return result.Distinct().ToList();

    }

    // 챕터에 갈 수 있는 모든 장소 Desc
    public List<string> Get_AllLocationDescChapter(string chapterID)
    {
        List<string> result = new List<string>();
        string[] lines = Get_lines(Location_CSV);
        int ChapterIndex = Get_Index(lines[dataReadLine], "Chapter");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int descIndex = Get_Index(lines[dataReadLine], "Description");

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[ChapterIndex] == chapterID &&
                lineData[languageIndex] == GameManager.Instance.languageID)
            {
                result.Add(lineData[descIndex]);
            }
        }
        return result;
    }

    // 장소 설명
    public string Get_LocationDesc(string chapterID, string locationID)
    {
        string[] lines = Get_lines(Location_CSV);
        int ChapterIndex = Get_Index(lines[dataReadLine], "Chapter");
        int locationIndex = Get_Index(lines[dataReadLine], "Idx_Location");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int descIndex = Get_Index(lines[dataReadLine], "Description");

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[ChapterIndex] == chapterID &&
                lineData[locationIndex] == locationID &&
                lineData[languageIndex] == GameManager.Instance.languageID)
            {
                return lineData[descIndex];
            }
        }
        return "";
    }

    // 장소 이름
    public string Get_LocationName(string locationID)
    {
        string[] lines = Get_lines(Location_CSV);
        int locationIndex = Get_Index(lines[dataReadLine], "Idx_Location");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int nameIndex = Get_Index(lines[dataReadLine], "Name");

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[locationIndex] == locationID &&
                lineData[languageIndex] == GameManager.Instance.languageID)
            {
                return lineData[nameIndex];
            }
        }
        return "";
    }

    #endregion

    #region About Object

    // 오브젝트 이름
    public string Get_ObjectName(string objectID)
    {
        string[] lines = Get_lines(Object_CSV);
        int objectIndex = Get_Index(lines[dataReadLine], "Idx_Object");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int nameIndex = Get_Index(lines[dataReadLine], "Name");

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[objectIndex] == objectID &&
                lineData[languageIndex] == GameManager.Instance.languageID)
            {
                Debug.Log(lineData[nameIndex]);
                return lineData[nameIndex];
            }
        }
        return null;
    }

    #endregion

    #region About Home Object

    // 집 오브젝트 이름
    public string Get_HomeObjectName(string homeObjectID)
    {
        string[] lines = Get_lines(HomeObject_CSV);
        int homeObjectIndex = Get_Index(lines[dataReadLine], "Idx_HomeObject");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int nameIndex = Get_Index(lines[dataReadLine], "Name");

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[homeObjectIndex] == homeObjectID &&
                lineData[languageIndex] == GameManager.Instance.languageID)
            {
                return lineData[nameIndex];
            }
        }
        return null;
    }

    // 집 오브젝트 재확인 질문
    public string Get_HomeObjectReconfirm(string homeObjectID)
    {
        string[] lines = Get_lines(HomeObject_CSV);
        int homeObjectIndex = Get_Index(lines[dataReadLine], "Idx_HomeObject");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int reconfirmIndex = Get_Index(lines[dataReadLine], "Reconfirm");
        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[homeObjectIndex] == homeObjectID &&
                lineData[languageIndex] == GameManager.Instance.languageID)
            {
                return lineData[reconfirmIndex];
            }
        }
        return null;
    }

    // 집 오브젝트 추가(수치 표시 또는, 경고문)
    public string Get_HomeObjectExtra(string homeObjectID)
    {
        string[] lines = Get_lines(HomeObject_CSV);
        int homeObjectIndex = Get_Index(lines[dataReadLine], "Idx_HomeObject");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int extraIndex = Get_Index(lines[dataReadLine], "Extra");
        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[homeObjectIndex] == homeObjectID &&
                lineData[languageIndex] == GameManager.Instance.languageID)
            {
                return lineData[extraIndex];
            }
        }
        return null;
    }

    #endregion

    #region About Streaming

    // 장소에 따른 방송 시작 다이얼로그 
    public string Get_StartStreamingDialog(string locationID)
    {
        string[] lines = Get_lines(Location_CSV);
        int locationIndex = Get_Index(lines[dataReadLine], "Idx_Location");

        int sDialogIndex = Get_Index(lines[dataReadLine], "Idx_SDialog");
        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[locationIndex] == locationID)
            {
                Debug.Log(lineData[sDialogIndex]);
                return lineData[sDialogIndex];
            }
        }
        return null;
    }


    #endregion

    #region Cacul

    public string[] Get_lines(TextAsset dataTextAsset)
    {
        return Regex.Split(dataTextAsset.text, LINE_SPLIT_RE);
    }
    private int Get_Index(string line, string indexName)
    {
        string[] lineData = Regex.Split(line, SPLIT_RE);
        return lineData.ToList().IndexOf(indexName);
    }
    
    #endregion

}
