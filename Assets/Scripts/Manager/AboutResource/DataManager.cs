//Refactoring v1.0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    #region New CSV Data

    [SerializeField] TextAsset Chapter_CSV;
    [SerializeField] TextAsset Location_CSV;
    [SerializeField] TextAsset HomeObject_CSV;
    [SerializeField] TextAsset Object_CSV;
    [SerializeField] TextAsset Dialog_CSV;
    [SerializeField] TextAsset Choice_CSV;
    [SerializeField] TextAsset SDialog_CSV;
    [SerializeField] TextAsset SChoice_CSV;
    [SerializeField] TextAsset Material_CSV;

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


    // 오브젝트 ID로 해금 추리 ID 가져오기
    public string Get_VisibleReasoningID(string objectID)
    {
        string[] lines = Get_lines(Object_CSV);
        return Get_String(lines, objectID, "Idx_Object", "Idx_Reason");
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

    // 다이얼로그 Illust
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

    // 다이얼로그 Animation 주인공의 애니메이션이가
    public bool Get_IsPlayerAnimationDialog(string DialogID)
    {
        string[] lines = Get_lines(Dialog_CSV);
        return Get_Bool(lines, DialogID, "Idx_Dialog", "IsPlayerAnimation");
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

    #region Reasoning

    // Chapter에 기본으로 주는 모든 소재 ID 가져오기
    public List<string> Get_MaterialIDsByChapter(string chapterID)
    {
        List<string> result = new List<string>();
        string[] lines = Get_lines(Material_CSV);
        int comparisonIndex = Get_Index(lines[dataReadLine], "Idx_Chapter");

        int getIdxValueIndex = Get_Index(lines[dataReadLine], "Idx_Material");
        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[comparisonIndex] == chapterID)
            {
                result.Add(lineData[getIdxValueIndex]);
            }
        }
        return result.Distinct().ToList();
    }

    // Reasoning으로 맞는 모든 소재 ID 찾기
    public List<string> Get_MaterialIDs(string reasoningID)
    {
        List<string> result = new List<string>();
        string[] lines = Get_lines(Material_CSV);
        int comparisonIndex = Get_Index(lines[dataReadLine], "Idx_Reason");
        int languageIndex = Get_Index(lines[dataReadLine], "Language");

        int getIdxValueIndex = Get_Index(lines[dataReadLine], "Idx_Material");
        foreach (string line in lines)
        {
            string[] lineData = Regex.Split(line, SPLIT_RE);
            if (lineData[comparisonIndex] == reasoningID &&
                lineData[languageIndex] == LanguageManager.Instance.languageID)
            {
                result.Add(lineData[getIdxValueIndex]);
            }
        }
        return result;
    }

    // 소재로 Name 가져오기
    public string Get_ReasoningName(string MaterialID)
    {
        string[] lines = Get_lines(Material_CSV);
        return Get_String(lines, MaterialID, "Idx_Material", "Name");
    }
    
    // 소재로 Description 가져오기
    public string Get_ReasoningDesc(string MaterialID)
    {
        string[] lines = Get_lines(Material_CSV);
        return Get_String(lines, MaterialID, "Idx_Material", "Description");
    }

    // 소재로 루트타입 가져오기
    public string Get_RootType(string MaterialID)
    {
        string[] lines = Get_lines(Material_CSV);
        return Get_String(lines, MaterialID, "Idx_Material", "RootType");
    }

    // 소재로 루트타입 점수 가져오기
    public int Get_RootTypePoint(string MaterialID)
    {
        string[] lines = Get_lines(Material_CSV);
        return Get_Int(lines, MaterialID, "Idx_Material", "RootPoint");
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
            if (lineData[comparisonIndex] == findID &&
                lineData[getIdxValueIndex] != "")
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
