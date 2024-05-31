//Refactoring v1.0
using UnityEngine;

public class ComputerInteract : InteractObject
{
    #region Value

    [Header("=== Camera")]
    [SerializeField] GameObject quarterViewCamera;
    [SerializeField] GameObject screenViewCamera;
    [SerializeField] GameObject screenObject;
    [SerializeField] public GameObject Computer2DCamera;

    [Header("=== UICanvas")]
    [SerializeField] GameObject InteractionUI3D;
    [SerializeField] GameObject Panel_InfoGO;

    #endregion

    #region Screen

    public void ScreenOn()
    {
        Panel_InfoGO.gameObject.SetActive(false);
        InteractionUI3D.SetActive(false);
        quarterViewCamera.SetActive(false);
        ActivityController.Instance.gameObject.SetActive(false);
        PlayerInputController.Instance.StopMove();
        PlayerController.Instance.resetAnime();

        screenObject.SetActive(true);
        screenViewCamera.SetActive(true);
        Computer2DCamera.SetActive(true);


    }

    public void ScreenOff()
    {
        Panel_InfoGO.gameObject.SetActive(true);
        InteractionUI3D.SetActive(true);
        quarterViewCamera.SetActive(true);
        ActivityController.Instance.gameObject.SetActive(true);
        PlayerInputController.Instance.CanMove = true;

        screenObject.SetActive(false);
        screenViewCamera.SetActive(false);
        Computer2DCamera.SetActive(false);
    }

    #endregion

    #region Interact

    public override void Interact()
    {
        if (!Outline.enabled) { return; }

        base.Interact();
        if (StreamController.Instance.isStreamingTime)
        { DesktopController.Instance.TurnOn(); }
        else
        { GameSystem.Instance.ObjDescOn(this, null); }
        

    }

    #endregion

}
