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
    [Space(10)]
    [SerializeField] Image blackScreen;

    public override void Interact()
    {
        base.Interact();
        StartCoroutine(ScreenZoomIn());
    }

    private IEnumerator ScreenZoomIn() // ��ǻ�� ȭ�� �� ���̰� ī�޶� ����
    {
        mainVitureCamera.SetActive(false);
        mainVitureCamera2.SetActive(false);
        screenCamera.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        ScreenOn();

        yield break;
    }
    public IEnumerator ScreenZoomOut() // ���� ī�޶�� ����
    {
        Debug.Log("out");
        mainVitureCamera.SetActive(true);
        screenCamera.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        ScreenOff();

        yield break;
    }

    private void ScreenOn()
    {
        // ���߿� �ִϸ��̼� �߰�!!!
        blackScreen.gameObject.SetActive(false);
    }

    private void ScreenOff()
    {
        blackScreen.gameObject.SetActive(true);
    }
}
