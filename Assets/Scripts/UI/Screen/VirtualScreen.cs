using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class VirtualScreen : GraphicRaycaster
{
    [Header("*Camera")]
    [Tooltip("2D 화면을 비추는 카메라")]
    [SerializeField] Camera screenCamera;
    [Tooltip("2D 화면의 UI 캔버스")]
    [SerializeField] GraphicRaycaster screenCaster;
    [SerializeField] GameObject TargetOB;

    [HideInInspector] public Vector3 eventdataPos;

    Ray ray;
    RaycastHit hit;

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        ray = eventCamera.ScreenPointToRay(eventData.position);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == TargetOB && hit.collider.transform == transform)
        {
            Debug.Log("In Field");
            Vector3 virtualPos = new Vector3(hit.textureCoord.x, hit.textureCoord.y);
            virtualPos.x *= screenCamera.targetTexture.width;
            virtualPos.y *= screenCamera.targetTexture.height;

            eventData.position = virtualPos;
            eventdataPos = virtualPos;
            screenCaster.Raycast(eventData, resultAppendList);
        }
        else
        {
            Debug.Log("Out Field");
        }
    }
}