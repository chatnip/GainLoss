using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteract : InteractObject
{
    [SerializeField] GameSystem GameSystem;
    [SerializeField] ScheduleManager ScheduleManager;

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

    [TextArea]
    [SerializeField] public string description;

    private void OnEnable()
    {
        /*string id = ScheduleManager.currentPrograssScheduleID;
        if(id == null || id == "") { return; }
        else if("S03" == id) // ������ ���̵� ��� ��û�϶��...
        {
            CanInteract = false;
        }*/
    }

    private IEnumerator ScreenZoomIn() // ��ǻ�� ȭ�� �� ���̰� ī�޶� ����
    {
        screenObject.SetActive(true);
        quarterViewCamera.SetActive(false);
        screenViewCamera.SetActive(true);

        //yield return new WaitForSeconds(1f);

        ScreenOn();

        yield break;
    }

    public IEnumerator ScreenZoomOut(bool isNext) // ���� ī�޶�� ����
    {
        ScreenOff();
        quarterViewCamera.SetActive(true);
        screenViewCamera.SetActive(false);

        //yield return new WaitForSeconds(1f);

        //quarterViewCamera.SetActive(true);
        //screenViewCamera.SetActive(false);
        screenObject.SetActive(false);

        if(isNext)
        {
            GameSystem.GameStart();
        }

        yield break;
    }

    private void ScreenOn()
    {
        // ���߿� �ִϸ��̼� �߰�!!!
        Computer2DCamera.SetActive(true);
        InteractionUI3D.SetActive(false);
    }

    private void ScreenOff()
    {
        Computer2DCamera.SetActive(false);
        InteractionUI3D.SetActive(true);
    }

    public override void Interact()
    {
        StartCoroutine(ScreenZoomIn());
    }

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
