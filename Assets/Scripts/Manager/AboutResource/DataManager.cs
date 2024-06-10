//Refactoring v1.0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    [SerializeField] TextAsset Chapter_CSV;
    [SerializeField] TextAsset Location_CSV;
    [SerializeField] TextAsset HomeObject_CSV;
    [SerializeField] TextAsset Object_CSV;
    [SerializeField] TextAsset Dialog_CSV;
    [SerializeField] TextAsset Choice_CSV;
    [SerializeField] TextAsset SDialog_CSV;
    [SerializeField] TextAsset SChoice_CSV;

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

    #region About Chapter

    // 챕터 이름 가져오기
    public string Get_ChapterName(string chapterID)
    {
        string[] lines = Get_lines(Chapter_CSV);
        return Get_String(lines, chapterID, "Idx_Chapter", "Name");
    }

    // 챕터 시작 날짜
    public int Get_ChapterStartDay(string chapterID)
    {
        string[] lines = Get_lines(Chapter_CSV);
        return Get_Int(lines, chapterID, "Idx_Chapter", "StartDay");
    }
    
    // 챕터 마지막 날짜
    public int Get_ChapterEndDay(string chapterID)
    {
        string[] lines = Get_lines(Chapter_CSV);
        return Get_Int(lines, chapterID, "Idx_Chapter", "EndDay");
    }

    // 챕터 매일 주는 행동력 판별
    public int Get_GiveActivity(string chapterID)
    {
        string[] lines = Get_lines(Chapter_CSV);
        return Get_Int(lines, chapterID, "Idx_Chapter", "GiveActivity");
    }

    // 챕터 정신력, 설득력, 관찰력 초기값
    public int Get_Men(string chapterID)
    {
        string[] lines = Get_lines(Chapter_CSV);
        return Get_Int(lines, chapterID, "Idx_Chapter", "Men");
    }
    public int Get_Obs(string chapterID)
    {
        string[] lines = Get_lines(Chapter_CSV);
        return Get_Int(lines, chapterID, "Idx_Chapter", "Obs");
    }
    public int Get_Soc(string chapterID)
    {
        string[] lines = Get_lines(Chapter_CSV);
        return Get_Int(lines, chapterID, "Idx_Chapter", "Soc");
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
                lineData[languageIndex] == LanguageManager.Instance.languageID)
            {
                result.Add(lineData[descIndex]);
            }
        }
        return result.Distinct().ToList();
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
                lineData[languageIndex] == LanguageManager.Instance.languageID)
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
                lineData[languageIndex] == LanguageManager.Instance.languageID)
            {
                return lineData[nameIndex];
            }
        }
        return "";
    }

    // 장소에 따른 방송 시작과 끝 Dialog
    public string Get_StartSDialog(string locationID)
    {
        string[] lines = Get_lines(Location_CSV);
        return Get_String(lines, locationID, "Idx_Location", "Idx_SDialog_Start");
    }
    public string Get_EndSDialog(string locationID)
    {
        string[] lines = Get_lines(Location_CSV);
        return Get_String(lines, locationID, "Idx_Location", "Idx_SDialog_End");
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
                lineData[languageIndex] == LanguageManager.Instance.languageID)
            {
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
                lineData[languageIndex] == LanguageManager.Instance.languageID)
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
                lineData[languageIndex] == LanguageManager.Instance.languageID)
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
                lineData[languageIndex] == LanguageManager.Instance.languageID)
            {
                return lineData[extraIndex];
            }
        }
        return null;
    }

    #endregion

    #region About Streaming

    #endregion

    #region About Dialog

    // 다이얼로그 Text
    public string Get_DialogText(string DialogID)
    {
        string[] lines = Get_lines(Dialog_CSV);
        return Get_String(lines, DialogID, "Idx_Dialog", "DialogText");
    }

    // 다음 다이얼로그 ID
    public string Get_NextDialogID(string DialogID)
    {
        string[] lines = Get_lines(Dialog_CSV);
        return Get_String(lines, DialogID, "Idx_Dialog", "NextDialogID");
    }

    // 다이얼로그 화자
    public string Get_DialogSpeaker(string DialogID)
    {
        string[] lines = Get_lines(Dialog_CSV);
        return Get_String(lines, DialogID, "Idx_Dialog", "Name");
    }

    // 다이얼로그 Anim Name
    public string Get_DialogIllust(string DialogID)
    {
        string[] lines = Get_lines(Dialog_CSV);
        return Get_String(lines, DialogID, "Idx_Dialog", "Idx_Illust");
    }

    // 다이얼로그 Anim Name
    public string Get_DialogAnim(string DialogID)
    {
        string[] lines = Get_lines(Dialog_CSV);
        return Get_String(lines, DialogID, "Idx_Dialog", "Idx_Animation");
    }

    // 다이얼로그가 선택지를 가지고 있는가
    public bool Get_DialogHasChoice(string DialogID)
    {
        string[] lines = Get_lines(Dialog_CSV);
        return Get_Bool(lines, DialogID, "Idx_Dialog", "HasChoices");
    }

    // 오브젝트 ID로 다이얼로그 시작 ID 가져오기 
    public string Get_DialogID(string ObjectID)
    {
        string[] lines = Get_lines(Dialog_CSV);
        return Get_String(lines, ObjectID, "Idx_Object", "Idx_Dialog");
    }

    #endregion

    #region About Choice

    // 다이얼로그ID로 선택지 가져오기
    public List<string> Get_ChoiceIDs(string DialogID)
    {
        List<string> result = new List<string>();

        string[] lines = Get_lines(Choice_CSV);

        int comparisonIndex = Get_Index(lines[dataReadLine], "Idx_Dialog");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int getIdxValueIndex = Get_Index(lines[dataReadLine], "Idx_Choice");

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[comparisonIndex] == DialogID &&
                lineData[languageIndex] == LanguageManager.Instance.languageID)
            {
                result.Add(lineData[getIdxValueIndex]);
            }
        }
        return result.Distinct().ToList();
    }

    // 선택지ID로 선택지Text 가져오기
    public string Get_ChoiceText(string ChoiceID)
    {
        string[] lines = Get_lines(Choice_CSV);
        return Get_String(lines, ChoiceID, "Idx_Choice", "ChoiceText");
    }

    // 선택지에 필요한 요소 이름
    public string Get_ChoiceNeedAbility(string ChoiceID)
    {
        string[] lines = Get_lines(Choice_CSV);
        return Get_String(lines, ChoiceID, "Idx_Choice", "RequiredPointType");
    }

    // 선택지에 필요한 요소 양
    public int Get_ChoiceNeedAbilityAmount(string ChoiceID)
    {
        string[] lines = Get_lines(Choice_CSV);
        return Get_Int(lines, ChoiceID, "Idx_Choice", "RequiredPoints");
    }

    // 선택지 다음 Dialog ID
    public string Get_NextDialogByChoice(string ChoiceID)
    {
        string[] lines = Get_lines(Choice_CSV);
        return Get_String(lines, ChoiceID, "Idx_Choice", "NextDialogID");
    }

    // 선택지 SDialog ID
    public string Get_SDialogByChoice(string ChoiceID)
    {
        string[] lines = Get_lines(Choice_CSV);
        return Get_String(lines, ChoiceID, "Idx_Choice", "Idx_SDialog");
    }

    // 선택지로 추리소재 얻기
    public string Get_ReasoningMaterial(string ChoiceID)
    {
        string[] lines = Get_lines(Choice_CSV);
        return Get_String(lines, ChoiceID, "Idx_Choice", "Idx_Material");
    }

    #endregion

    #region Streaming Dialog

    // S다이얼로그 Text
    public string Get_SDialogText(string SDialogID)
    {
        string[] lines = Get_lines(SDialog_CSV);
        return Get_String(lines, SDialogID, "Idx_SDialog", "ChatingText");
    }

    // S다이얼로그 Name
    public string Get_SDialogName(string SDialogID)
    {
        string[] lines = Get_lines(SDialog_CSV);
        return Get_String(lines, SDialogID, "Idx_SDialog", "Name");
    }

    // S다이얼로그 Anim Name
    public string Get_SDialogAnim(string DialogID)
    {
        string[] lines = Get_lines(SDialog_CSV);
        return Get_String(lines, DialogID, "Idx_SDialog", "Idx_SAnimation");
    }

    // 다음 S다이얼로그 ID
    public string Get_NextSDialogID(string SDialogID)
    {
        string[] lines = Get_lines(SDialog_CSV);
        return Get_String(lines, SDialogID, "Idx_SDialog", "NextDialogID");
    }

    // 다이얼로그가 선택지를 가지고 있는가
    public bool Get_SDialogHasChoice(string SDialogID)
    {
        string[] lines = Get_lines(SDialog_CSV);
        return Get_Bool(lines, SDialogID, "Idx_SDialog", "HasChoices");
    }

    #endregion

    #region Streaming Choice

    // 선택지 다음 SDialog ID
    public string Get_NextSDialogBySChoice(string SChoiceID)
    {
        string[] lines = Get_lines(SChoice_CSV);
        return Get_String(lines, SChoiceID, "Idx_SChoices", "NextDialogID");
    }

    // S다이얼로그ID로 선택지 가져오기
    public List<string> Get_SChoiceIDs(string SDialogID)
    {
        List<string> result = new List<string>();

        string[] lines = Get_lines(SChoice_CSV);

        int comparisonIndex = Get_Index(lines[dataReadLine], "Idx_SDialog");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int getIdxValueIndex = Get_Index(lines[dataReadLine], "Idx_SChoices");

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[comparisonIndex] == SDialogID &&
                lineData[languageIndex] == LanguageManager.Instance.languageID)
            {
                result.Add(lineData[getIdxValueIndex]);
            }
        }
        return result.Distinct().ToList();
    }


    // 선택지ID로 선택지Text 가져오기
    public string Get_SChoiceText(string SChoiceID)
    {
        string[] lines = Get_lines(SChoice_CSV);
        return Get_String(lines, SChoiceID, "Idx_SChoices", "ChoiceText");
    }
    
    // 선택지ID로 선택지Text 가져오기
    public string Get_GEPoint(string SChoiceID)
    {
        string[] lines = Get_lines(SChoice_CSV);
        return Get_String(lines, SChoiceID, "Idx_SChoices", "GEPoint");
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

    private bool Get_Bool(string[] lines, string findID, string comparisonIdx, string getIdxValue)
    {
        int comparisonIndex = Get_Index(lines[dataReadLine], comparisonIdx);

        int getIdxValueIndex = Get_Index(lines[dataReadLine], getIdxValue);

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[comparisonIndex] == findID)
            {
                return Convert.ToBoolean(lineData[getIdxValueIndex]);
            }
        }
        return false;
    }

    private int Get_Int(string[] lines, string findID, string comparisonIdx, string getIdxValue)
    {
        int comparisonIndex = Get_Index(lines[dataReadLine], comparisonIdx);

        int getIdxValueIndex = Get_Index(lines[dataReadLine], getIdxValue);

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[comparisonIndex] == findID)
            {
                return Convert.ToInt32(lineData[getIdxValueIndex]);
            }
        }
        return 0;
    }

    private string Get_String(string[] lines, string findID, string comparisonIdx, string getIdxValue)
    {
        int comparisonIndex = Get_Index(lines[dataReadLine], comparisonIdx);
        int getIdxValueIndex = Get_Index(lines[dataReadLine], getIdxValue);
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[comparisonIndex] == findID &&
                lineData[languageIndex] == LanguageManager.Instance.languageID)
            {
                return lineData[getIdxValueIndex];
            }
        }
        return "";
    }

    #endregion

    #region Other

    public Dictionary<string, List<string>> abilityTypeLanguage = new Dictionary<string, List<string>>
    {
        { "observation", new List<string> { "observation", "관찰력" } },
        { "sociability", new List<string> { "sociability", "설득력" } },
        { "mentality", new List<string> { "mentality", "정신력" } }
    };

    #endregion
}
