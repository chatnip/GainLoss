using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerInteract : InteractObject
{
    #region Value

    [Header("*property")]
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] PlayerInputController PlayerInputController;
    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;

    [Header("*Camera")]
    [Tooltip("���� ���� ī�޶�")]
    [SerializeField] GameObject quarterViewCamera;
    [Tooltip("��ũ�� ī�޶�")]
    [SerializeField] GameObject screenViewCamera;
    [SerializeField] GameObject screenObject;
    [Space(10)]
    [SerializeField] public GameObject Computer2DCamera;

    [Header("*UICanvas")]
    [SerializeField] GameObject InteractionUI3D;
    [SerializeField] GameObject PhoneOpenBtns;

    [Header("*Description")]
    [TextArea] [SerializeField] public string description;

    #endregion

    #region Screen

    private IEnumerator ScreenZoomIn() // ��ǻ�� ȭ�� �� ���̰� ī�޶� ����
    {
        screenObject.SetActive(true);
        quarterViewCamera.SetActive(false);
        screenViewCamera.SetActive(true);

        SchedulePrograss.gameObject.SetActive(false);
        PhoneOpenBtns.SetActive(false);
        PhoneHardware.ResetPhoneBtns();


        ScreenOn();

        yield break;
    }

    public IEnumerator ScreenZoomOut() // ���� ī�޶�� ����
    {
        ScreenOff();
        quarterViewCamera.SetActive(true);
        screenViewCamera.SetActive(false);

        PhoneOpenBtns.SetActive(true);
        SchedulePrograss.gameObject.SetActive(true);
        screenObject.SetActive(false);

        yield break;
    }

    private void ScreenOn()
    {
        ObjectInteractionButtonGenerator.SetOnOffInteractObjectBtn();
        // ���߿� �ִϸ��̼� �߰�!!!
        Computer2DCamera.SetActive(true);
        InteractionUI3D.SetActive(false);

        PlayerInputController.StopMove();


        ScheduleManager.ResetDotweenGuide();
    }

    private void ScreenOff()
    {
        Computer2DCamera.SetActive(false);
        InteractionUI3D.SetActive(true);

        PlayerInputController.CanMove = true;


        ScheduleManager.SetDotweenGuide();
    }

    #endregion

    #region Interact

    public override void Interact()
    {
        if ((ScheduleManager.currentPrograssScheduleID == "S01" ||
            ScheduleManager.currentPrograssScheduleID == "S03") &&
            !ScheduleManager.currentPrograssScheduleComplete) // S01: ���� ���� | S03: ��� | ���� ������ ���� ��Ȳ
        { StartCoroutine(ScreenZoomIn()); }
        else { GameSystem.ObjectDescriptionOn(description); base.Interact(); }
        //StartCoroutine(ScreenZoomIn());
    }

    #endregion

}
