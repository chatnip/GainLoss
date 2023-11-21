using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractController : MonoBehaviour
{
    [SerializeField] InputAction axis;

    private void OnEnable()
    {
        axis.Enable();
        axis.performed += context =>
        {
            Vector2 cursorVector = context.ReadValue<Vector2>();
            RaycastCursor(cursorVector);
        };
    }

    private void OnDisable()
    {
        axis.Disable();
    }

    private void RaycastCursor(Vector2 cursorVector)
    {
        Ray ray = Camera.main.ScreenPointToRay(cursorVector);
        int layerMask = 1 << LayerMask.NameToLayer("InteractObject");
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, layerMask))
        {
            hit.transform.TryGetComponent(out IInteract interact);
            interact.Interact();
        }
    }
}
