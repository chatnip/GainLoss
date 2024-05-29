//Refactoring v1.0
using UnityEngine;

public class DesktopController : Singleton<DesktopController>
{
    #region Value

    [Header("=== ID")]
    [SerializeField] public string desktopAppID;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region Active On/Off

    public virtual void ActiveOn() 
    {
        this.gameObject.SetActive(true);
    } 

    #endregion
}
