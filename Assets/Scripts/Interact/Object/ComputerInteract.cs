using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerInteract : InteractObject
{
    #region Value

    [Header("*property")]
    [SerializeField] PhoneHardware PhoneHardware;
    [SerializeField] ObjectInteractionButtonGenerator ObjectInteractionButtonGenerator;
    [SerializeField] ActivityController ActivityController;

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
    [SerializeField] GameObject IconCollectionGO;

    [Header("*Description")]
    [TextArea] [SerializeField] public string description;

    #endregion

    #region Screen

    private IEnumerator ScreenZoomIn() // ��ǻ�� ȭ�� �� ���̰� ī�޶� ����
    {
        screenObject.SetActive(true);
        quarterViewCamera.SetActive(false);
        screenViewCamera.SetActive(true);


        ScreenOn();

        yield break;
    }

    public IEnumerator ScreenZoomOut() // ���� ī�޶�� ����
    {
        ScreenOff();
        quarterViewCamera.SetActive(true);
        screenViewCamera.SetActive(false);
        screenObject.SetActive(false);

        yield break;
    }

    private void ScreenOn()
    {
        Computer2DCamera.SetActive(true);
        InteractionUI3D.SetActive(false);

        PlayerInputController.Instance.StopMove();

        ActivityController.gameObject.SetActive(false);
        IconCollectionGO.gameObject.SetActive(false);
    }

    private void ScreenOff()
    {
        Computer2DCamera.SetActive(false);
        InteractionUI3D.SetActive(true);

        PlayerInputController.Instance.CanMove = true;

        ActivityController.gameObject.SetActive(true);
        IconCollectionGO.gameObject.SetActive(true);
        
    }

    #endregion

    #region Interact

    public override void Interact()
    {
        
        //GameSystem.ObjectDescriptionOn(description); base.Interact();
        StartCoroutine(ScreenZoomIn());
    }

    #endregion

}
