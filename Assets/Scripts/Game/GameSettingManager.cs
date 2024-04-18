using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


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

    public void SetAllSetting()
    {
        LoadAndSet_GameSetting();
        LoadAndSet_AudioSetting();
        LoadAndSet_VedioSetting();
    }

    public void LoadAndSet_GameSetting()
    {

    }

    public void LoadAndSet_AudioSetting()
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

    public void LoadAndSet_VedioSetting()
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
        List<int> Width_Height = GameSetting.GameSetting_Video.GetDisplayValueByEnum(GameSetting.GameSetting_Video.display_Resolution);
        Screen.SetResolution(Width_Height[0], Width_Height[1], GameSetting.GameSetting_Video.FullScreen);

        // FPS
        Application.targetFrameRate = GameSetting.GameSetting_Video.GetDisplayValueByEnum(GameSetting.GameSetting_Video.display_FPSLimit);
    }

    public void LoadAndSet_CreditSetting()
    {

    }

    IEnumerator SetFPS()
    {
        FPSText.text = "FPS: " + Application.targetFrameRate;
        yield return new WaitForSeconds(1f);
        StartCoroutine(SetFPS());
    }

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


    public List<int> GetDisplayValueByEnum(Display_Resolution DisplayEnum)
    {
        return display_ResolusionValueDict[DisplayEnum];
    }
    public int GetDisplayValueByEnum(FramePerSecond DisplayEnum)
    {
        return display_FPSValueDict[DisplayEnum];
    }
}

#endregion

#region Vedio Value

public enum Display_Resolution
{
    HD, HD_plus, FHD, QHD
}
public enum FramePerSecond
{
    FPS_30, FPS_60, FPS_70, FPS_100, FPS_120, FPS_144
}

#endregion