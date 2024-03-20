using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModuleUISO", menuName = "Scriptable Object/ModuleUISO", order = int.MaxValue)]
public class ModuleUISO : ScriptableObject
{
    public List<Sprite> HUD_SourceSprites;
    public List<Sprite> Phone_SourceSprites;
    public List<Sprite> Computer_SourceSprites;
}
