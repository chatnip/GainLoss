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

    /*
    [HideInInspector] public bool CanInter = true;
    
    private void OnEnable()
    {
        string id = ScheduleManager.currentPrograssScheduleID;
        if(id == null || id == "") { return; }
        else if("S03" == id) // ������ ���̵� ��� ��û�϶��...
        {
            CanInteract = false;
        }
    }
    */

    #region Screen

    private IEnumerator ScreenZoomIn() // ��ǻ�� ȭ�� �� ���̰� ī�޶� ����
    {
        screenObject.SetActive(true);
        quarterViewCamera.SetActive(false);
        screenViewCamera.SetActive(true);

        SchedulePrograss.gameObject.SetActive(false);
        //SchedulePrograss.ResetExlanation();
        PhoneOpenBtns.SetActive(false);
        PhoneHardware.ResetPhoneBtns();

        //yield return new WaitForSeconds(1f);

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

        //yield return new WaitForSeconds(1f);

        //quarterViewCamera.SetActive(true);
        //screenViewCamera.SetActive(false);
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
    }

    private void ScreenOff()
    {
        Computer2DCamera.SetActive(false);
        InteractionUI3D.SetActive(true);

        PlayerInputController.CanMove = true;
    }

    #endregion

    #region Interact

    public override void Interact()
    {
        if (ScheduleManager.currentPrograssScheduleID == "S01" ||
            ScheduleManager.currentPrograssScheduleID == "S03") // S01: ���� ���� | S03: ���
        { StartCoroutine(ScreenZoomIn()); }
        else { GameSystem.ObjectDescriptionOn(description); base.Interact(); }
        //StartCoroutine(ScreenZoomIn());
    }

    #endregion

    /*    public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            StartCoroutine(ScreenZoomIn());
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
        }*/
}
