using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenObject : InteractCore
{
    [Header("*Camera")]
    [Tooltip("���� ���� ī�޶�")]
    [SerializeField] GameObject mainVitureCamera;
    [SerializeField] GameObject mainVitureCamera2; // �÷��̾� �� ��������, ī�޶� �ϳ��� ���ܵα�
    [Tooltip("��ũ�� ī�޶�")]
    [SerializeField] GameObject screenCamera;
    [SerializeField] GameObject screenObject;
    [Space(10)]
    [SerializeField] GameObject Computer2DCamera;

    public override void Interact()
    {
        base.Interact();
        StartCoroutine(ScreenZoomIn());
    }

    public override void InteractCancel()
    {
        base.InteractCancel();
        StartCoroutine(ScreenZoomOut());
    }

    private IEnumerator ScreenZoomIn() // ��ǻ�� ȭ�� �� ���̰� ī�޶� ����
    {
        screenObject.SetActive(true);
        mainVitureCamera.SetActive(false);
        mainVitureCamera2.SetActive(false);
        screenCamera.SetActive(true);

        yield return new WaitForSeconds(3f);

        ScreenOn();

        yield break;
    }
    public IEnumerator ScreenZoomOut() // ���� ī�޶�� ����
    {
        // Debug.Log("out");
        ScreenOff();
        mainVitureCamera.SetActive(true);
        screenCamera.SetActive(false);

        yield return new WaitForSeconds(3f);

        mainVitureCamera.SetActive(true);
        screenCamera.SetActive(false);
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
}
