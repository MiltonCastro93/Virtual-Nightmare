using UnityEngine;

public class CharacterInteractor : MonoBehaviour
{//detecta/interactúa
    [Header("La Punta inicial del RayCast")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 2f;
    private IInteractable currentInteractable;

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        Debug.DrawRay(
            ray.origin,
            ray.direction * interactDistance,
            Color.red
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            //Debug.Log($"Raycast golpeó: {hit.collider.gameObject.name}");

            currentInteractable =
                hit.collider.GetComponent<IInteractable>();
        }
    }

    public void Interact()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact();//hago que ejecute lo que tiene que ejecutar
    }
}
