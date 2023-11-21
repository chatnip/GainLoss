using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerInteract : InteractObject
{
    [Header("*Camera")]
    [Tooltip("���� ���� ī�޶�")]
    [SerializeField] GameObject quarterViewCamera;
    [Tooltip("��ũ�� ī�޶�")]
    [SerializeField] GameObject screenViewCamera;
    [SerializeField] GameObject screenObject;
    [Space(10)]
    [SerializeField] GameObject Computer2DCamera;

    private IEnumerator ScreenZoomIn() // ��ǻ�� ȭ�� �� ���̰� ī�޶� ����
    {
        screenObject.SetActive(true);
        quarterViewCamera.SetActive(false);
        screenViewCamera.SetActive(true);

        yield return new WaitForSeconds(3f);

        ScreenOn();

        yield break;
    }

    public IEnumerator ScreenZoomOut() // ���� ī�޶�� ����
    {
        ScreenOff();
        quarterViewCamera.SetActive(true);
        screenViewCamera.SetActive(false);

        yield return new WaitForSeconds(3f);

        quarterViewCamera.SetActive(true);
        screenViewCamera.SetActive(false);
        screenObject.SetActive(false);

        yield break;
    }

    private void ScreenOn()
    {
        // ���߿� �ִϸ��̼� �߰�!!!
        Computer2DCamera.SetActive(true);
    }

    private void ScreenOff()
    {
        Computer2DCamera.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
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
    }
}
