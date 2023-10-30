using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenObject : InteractCore
{
    [Header("*Camera")]
    [Tooltip("메인 가상 카메라")]
    [SerializeField] GameObject mainVitureCamera;
    [SerializeField] GameObject mainVitureCamera2; // 플레이어 뷰 정해지면, 카메라 하나만 남겨두기
    [Tooltip("스크린 카메라")]
    [SerializeField] GameObject screenCamera;
    [Space(10)]
    [SerializeField] Image blackScreen;

    public override void Interact()
    {
        base.Interact();
        StartCoroutine(ScreenZoomIn());
    }

    private IEnumerator ScreenZoomIn() // 컴퓨터 화면 잘 보이게 카메라 변경
    {
        mainVitureCamera.SetActive(false);
        mainVitureCamera2.SetActive(false);
        screenCamera.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        ScreenOn();

        yield break;
    }
    public IEnumerator ScreenZoomOut() // 본래 카메라로 변경
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
        // 나중에 애니메이션 추가!!!
        blackScreen.gameObject.SetActive(false);
    }

    private void ScreenOff()
    {
        blackScreen.gameObject.SetActive(true);
    }
}
