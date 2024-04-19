using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class GameSettingManager : Manager<GameSettingManager>
{
    [Header("*Property")]
    [SerializeField] public GameSetting GameSetting;

    [Header("*UI")]
    [SerializeField] TMP_Text FPSText;
    [SerializeField] List<GameObject> PadUIs;
    [SerializeField] Canvas MainCanvas;

    protected override void Awake()
    {
        SetAllSetting();
    }

    #region Apply By Json

    public void SetAllSetting()
    {
        Apply_Gs();
        Apply_As();
        Apply_Vs();
    }

    // Game Setting
    public void Apply_Gs()
    {

    }

    // Audio Setting
    public void Apply_As()
    {
        // Pad UI
        if (GameSetting.GameSetting_Game.ShowGuidePadUI)
        {
            foreach (GameObject PadUI in PadUIs) { PadUI.gameObject.SetActive(true); }
        }
        else
        {
            foreach (GameObject PadUI in PadUIs) { PadUI.gameObject.SetActive(false); }
        }

        // UI Scale
        if(MainCanvas != null)
        {

        }
    }

    // Video Setting
    public void Apply_Vs()
    {
        // FPS Show
        if (GameSetting.GameSetting_Video.ShowFPS)
        {
            FPSText.gameObject.SetActive(true);
            StartCoroutine(SetFPS()); 
        }
        else
        {
            FPSText.gameObject.SetActive(false);
            StopCoroutine(SetFPS());
        }

        // FullScreen, Resolution
        List<int> Width_Height = 
            GameSetting.GameSetting_Video.GetDisplayValueByEnum_Reso(
                GameSetting.GameSetting_Video.display_Resolution);
        Screen.SetResolution(Width_Height[0], Width_Height[1], GameSetting.GameSetting_Video.FullScreen);

        // FPS
        Application.targetFrameRate =
            GameSetting.GameSetting_Video.GetDisplayValueByEnum_Fps(
                GameSetting.GameSetting_Video.display_FPSLimit);
    }

    // Credit Setting
    public void Apply_Cs()
    {

    }

    #endregion

    #region Init InData

    public void InitData_Gs()
    {
    }

    #endregion

    #region Other

    IEnumerator SetFPS()
    {
        FPSText.text = "FPS: " + Application.targetFrameRate;
        yield return new WaitForSeconds(1f);
        StartCoroutine(SetFPS());
    }

    #endregion

}


#region Setting Detail

[System.Serializable]
public class GameSetting
{
    
    public GameSetting_Game GameSetting_Game;
    public GameSetting_Audio GameSetting_Audio;
    public GameSetting_Video GameSetting_Video;
}


[System.Serializable]
public class GameSetting_Game
{
    public bool ShowGuidePadUI = true;
    public int MainUIScale = 5;


}
[System.Serializable]
public class GameSetting_Audio
{
    
}
[System.Serializable]
public class GameSetting_Video
{
    public bool FullScreen = true;
    public Display_Resolution display_Resolution = Display_Resolution.FHD;
    public FramePerSecond display_FPSLimit = FramePerSecond.FPS_144;
    public bool ShowFPS = false;

    private Dictionary<Display_Resolution, List<int>> display_ResolusionValueDict;
    private Dictionary<FramePerSecond, int> display_FPSValueDict;


    public GameSetting_Video()
    {
        display_ResolusionValueDict = new Dictionary<Display_Resolution, List<int>>
        {
            { Display_Resolution.HD, new List<int> { 1280, 720 } },
            { Display_Resolution.HD_plus, new List<int> { 1600, 900 } },
            { Display_Resolution.FHD, new List<int> { 1920, 1080 } },
            { Display_Resolution.QHD, new List<int> { 2560, 1440 } }
        };
        display_FPSValueDict = new Dictionary<FramePerSecond, int>
        {   
            { FramePerSecond.FPS_30, 30 },
            { FramePerSecond.FPS_60, 60 },
            { FramePerSecond.FPS_70, 70 },
            { FramePerSecond.FPS_100, 100 },
            { FramePerSecond.FPS_120, 120 },
            { FramePerSecond.FPS_144, 144 }
        };
    }


    // Get Resolution Values
    public List<int> GetDisplayValueByEnum_Reso(Display_Resolution DR)
    { return display_ResolusionValueDict[DR]; }
    public Display_Resolution GetDisplayEnumByValue_Reso(List<int> resoInts)
    {
        foreach (var e in display_ResolusionValueDict)
        {
            if (e.Value == resoInts)
            {
                Debug.Log("도출");
                return e.Key;
            }
        }
        return Display_Resolution.FHD;
    }


    // Get Fps Values
    public int GetDisplayValueByEnum_Fps(FramePerSecond FPS)
    { return display_FPSValueDict[FPS]; }
    public FramePerSecond GetDisplayEnumByValue_Fps(int FpsInts)
    { return display_FPSValueDict.FirstOrDefault(x => x.Value == FpsInts).Key; }
}

#endregion

#region Vedio Value

public enum Display_Resolution
{
    HD = 0, HD_plus = 1, FHD = 2, QHD = 3
}
public enum FramePerSecond
{
    FPS_30 = 0, FPS_60 = 1, FPS_70 = 2, FPS_100 = 3, FPS_120 = 4, FPS_144
}

#endregion

#region Other (Cacul)

// Enum확장
public static class Enum_Extensions
{
    // Enum 다음 값 받기 -> 최대치 넘어가면 최초값
    public static T Next<T>(this T source) where T : System.Enum
    {
        T[] Arr = (T[])Enum.GetValues(source.GetType());
        int j = Array.IndexOf<T>(Arr, source) + 1;
        return (Arr.Length <= j) ? Arr[0] : Arr[j];
    }
    // Enum 이전 값 받기 -> 최소치 전으로 가면 최대치
    public static T Before<T>(this T source) where T : System.Enum
    {
        T[] Arr = (T[])Enum.GetValues(source.GetType());
        int j = Array.IndexOf<T>(Arr, source) - 1;
        return (0 > j) ? Arr[Arr.Length - 1] : Arr[j];
    }

}

#endregion