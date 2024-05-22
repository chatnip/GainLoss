using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIManager : Singleton<MainSceneUIManager>
{
    [Header("*In Game GOs or Components")]
    [SerializeField] InGameHUD InGameHUD;
    [SerializeField] InGamePhoneUI InGamePhoneUI;
    [SerializeField] InGameComputerUI InGameComputerUI;

    [Header("*Source Collections (each condition)")]
    [SerializeField] ModuleUISO BaseUI_collection;
    [SerializeField] ModuleUISO angerUI_collection;


    private void OnEnable()
    {
        InGameHUD.setAllImgs();
        InGamePhoneUI.setAllImgs();
        InGameComputerUI.setAllImgs();
    }

    public void changeUICollection(ModuleUISO targetUiCollection)
    {
        if(InGameHUD.allImgs != null && InGameHUD.allImgs.Count > 0)
        {
            for (int i = 0; i < InGameHUD.allImgs.Count; i++)
            {
                InGameHUD.allImgs[i].sprite = targetUiCollection.HUD_SourceSprites[i];
            }
        }
        if (InGamePhoneUI.allImgs != null && InGamePhoneUI.allImgs.Count > 0)
        {
            for (int i = 0; i < InGamePhoneUI.allImgs.Count; i++)
            {
                InGamePhoneUI.allImgs[i].sprite = targetUiCollection.Phone_SourceSprites[i];
            }
        }
        if (InGameComputerUI.allImgs != null && InGameComputerUI.allImgs.Count > 0)
        {
            for (int i = 0; i < InGameComputerUI.allImgs.Count; i++)
            {
                InGameComputerUI.allImgs[i].sprite = targetUiCollection.Computer_SourceSprites[i];
            }
        }
    }
}

[Serializable]
public class InGameHUD
{
    public Image placeInfo;
    public Image dayInfo;
    public Image moneyInfo;
    public Image phoneImg;
    public Image phoneBtnImg;
    public Image phoneScheduleBtnImg;
    public Image scheduleImg_0;
    public Image scheduleImg_1;
    public Image scheduleImg_2;
    public Image scheduleImg_3;
    public Image npcPanelImg;
    public Image objPanelImg;

    [HideInInspector] public List<Image> allImgs = new List<Image>();
    public void setAllImgs()
    {
        allImgs = new List<Image>
        {
            placeInfo, dayInfo, moneyInfo, phoneImg, phoneBtnImg, phoneScheduleBtnImg,
            scheduleImg_0, scheduleImg_1, scheduleImg_2, scheduleImg_3,
            npcPanelImg, objPanelImg
        };
    }
}
[Serializable]
public class InGamePhoneUI
{
    [HideInInspector] public List<Image> allImgs = new List<Image>();
    public void setAllImgs()
    {
        allImgs = new List<Image>
        {
            
        };
    }
}
[Serializable]
public class InGameComputerUI
{
    [HideInInspector] public List<Image> allImgs = new List<Image>();
    public void setAllImgs()
    {
        allImgs = new List<Image>
        {

        };
    }
}


