using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerInteract : InteractObject
{
    [Header("*Camera")]
    [Tooltip("메인 가상 카메라")]
    [SerializeField] GameObject quarterViewCamera;
    [Tooltip("스크린 카메라")]
    [SerializeField] GameObject screenViewCamera;
    [SerializeField] GameObject screenObject;
    [Space(10)]
    [SerializeField] GameObject Computer2DCamera;

    private IEnumerator ScreenZoomIn() // 컴퓨터 화면 잘 보이게 카메라 변경
    {
        screenObject.SetActive(true);
        quarterViewCamera.SetActive(false);
        screenViewCamera.SetActive(true);

        yield return new WaitForSeconds(3f);

        ScreenOn();

        yield break;
    }

    public IEnumerator ScreenZoomOut() // 본래 카메라로 변경
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
        // 나중에 애니메이션 추가!!!
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
