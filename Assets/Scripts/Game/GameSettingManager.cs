using UnityEngine;

public class GameSettingManager : MonoBehaviour
{
    public GameSetting GameSetting;
}

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

}
[System.Serializable]
public class GameSetting_Audio
{

}
[System.Serializable]
public class GameSetting_Video
{

}
