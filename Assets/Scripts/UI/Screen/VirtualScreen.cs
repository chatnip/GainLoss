using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualScreen : GraphicRaycaster
{
    [Header("*Camera")]
    [Tooltip("2D 화면을 비추는 카메라")]
    [SerializeField] Camera screenCamera;
    [Tooltip("2D 화면의 UI 캔버스")]
    [SerializeField] GraphicRaycaster screenCaster;

    
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        
        Ray ray = eventCamera.ScreenPointToRay(eventData.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);
            if (hit.collider.transform == transform)
            {
                Vector3 virtualPos = new Vector3(hit.textureCoord.x, hit.textureCoord.y);
                virtualPos.x *= screenCamera.targetTexture.width;
                virtualPos.y *= screenCamera.targetTexture.height;

                eventData.position = virtualPos;

                screenCaster.Raycast(eventData, resultAppendList);
            }
        }
    }
    

    /*
    // public Camera screenCamera;

    // Called by Unity when a Raycaster should raycast because it extends BaseRaycaster.
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        Ray ray = eventCamera.ScreenPointToRay(eventData.position); // Mouse
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            RaycastBeyondTV(hit, resultAppendList);
        }
    }

    private void RaycastBeyondTV(RaycastHit originHit, List<RaycastResult> resultAppendList)
    {
        // Figure out where the pointer would be in the second camera based on texture position or RenderTexture.
        Vector3 virtualPos = new Vector3(originHit.textureCoord.x, originHit.textureCoord.y);
        Ray ray = screenCamera.ViewportPointToRay(virtualPos);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 0.2f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            RaycastResult result = new RaycastResult
            {
                gameObject = hit.collider.gameObject,
                module = this,
                distance = hit.distance,
                index = resultAppendList.Count,
                worldPosition = hit.point,
                worldNormal = hit.normal,
            };
            resultAppendList.Add(result);
        }
    }
    */
    
}