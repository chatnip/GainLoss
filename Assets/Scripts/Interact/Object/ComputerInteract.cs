using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteract : InteractObject
{
    [SerializeField] GameSystem GameSystem;
    [SerializeField] ScheduleManager ScheduleManager;
    [SerializeField] SchedulePrograss SchedulePrograss;
    [SerializeField] PhoneHardware PhoneHardware;

    [Header("*Camera")]
    [Tooltip("메인 가상 카메라")]
    [SerializeField] GameObject quarterViewCamera;
    [Tooltip("스크린 카메라")]
    [SerializeField] GameObject screenViewCamera;
    [SerializeField] GameObject screenObject;
    [Space(10)]
    [SerializeField] public GameObject Computer2DCamera;

    [Header("*UICanvas")]
    [SerializeField] GameObject InteractionUI3D;
    [SerializeField] GameObject PhoneOpenBtns;

    [TextArea]
    [SerializeField] public string description;

    //[HideInInspector] public bool CanInter = true;

    private void OnEnable()
    {
        /*string id = ScheduleManager.currentPrograssScheduleID;
        if(id == null || id == "") { return; }
        else if("S03" == id) // 스케쥴 아이디가 방송 시청하라면...
        {
            CanInteract = false;
        }*/
    }

    private IEnumerator ScreenZoomIn() // 컴퓨터 화면 잘 보이게 카메라 변경
    {
        screenObject.SetActive(true);
        quarterViewCamera.SetActive(false);
        screenViewCamera.SetActive(true);

        SchedulePrograss.gameObject.SetActive(false);
        SchedulePrograss.ResetExlanation();
        PhoneOpenBtns.SetActive(false);
        PhoneHardware.ResetPhoneBtns();

        //yield return new WaitForSeconds(1f);

        ScreenOn();

        yield break;
    }

    public IEnumerator ScreenZoomOut(bool isNext) // 본래 카메라로 변경
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

        if(isNext)
        {
            GameSystem.GameStart();
        }

        yield break;
    }

    private void ScreenOn()
    {
        // 나중에 애니메이션 추가!!!
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
        if (ScheduleManager.currentPrograssScheduleID == "S03") { StartCoroutine(ScreenZoomIn()); }
        else { GameSystem.ObjectDescriptionOn(description); base.Interact(); }

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
