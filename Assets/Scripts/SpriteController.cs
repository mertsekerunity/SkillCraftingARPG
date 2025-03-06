using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private Camera mainCamera;
    private Transform parentTransform;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        parentTransform = transform.parent;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Always face the camera
        transform.rotation = mainCamera.transform.rotation;

        // Flip the sprite based on parent's movement direction
        Vector3 parentForward = parentTransform.forward;
        // We only care about the x and z components for horizontal movement
        Vector2 flatDirection = new Vector2(parentForward.x, parentForward.z).normalized;

        // Assuming your camera is at a 45-degree angle on X and -135 on Y
        // You may need to adjust this logic based on your specific camera angle
        if (flatDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
