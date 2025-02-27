using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkillProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] Vector3 spawnOffset = new Vector3(0, 1, 0); // Offset relatively from the player position

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void SpawnProjectile()
    {
        // Get mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Calculate direction from player to hit point
            Vector3 playerPos = transform.position;
            Vector3 targetPoint = hit.point;

            // Maintain y-component for proper 3D direction
            // This is important for the rotation calculation
            Vector3 direction = (targetPoint - playerPos).normalized;

            // Calculate spawn position (slightly offset from player if needed)
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Create rotation that looks in the direction of the target
            // Adding a 180-degree Y rotation to fix the backwards orientation
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);

            // Instantiate fireball at player's position with the proper rotation
            GameObject fireball = Instantiate(fireballPrefab, spawnPosition, targetRotation);

            // Get the Rigidbody component of the fireball
            Rigidbody fireballRb = fireball.GetComponent<Rigidbody>();
            if (fireballRb != null)
            {
                // Set velocity in the calculated direction
                fireballRb.velocity = direction * projectileSpeed;

                // Ensure no gravity and y-movement if needed in your game
                fireballRb.useGravity = false;

                // You may want to adjust or remove these constraints based on your game's design
                // For a true 3D projectile, you might not want to freeze Y position
                fireballRb.constraints = RigidbodyConstraints.FreezeRotation;
            }

            Debug.Log($"Fireball instantiated! Direction: {direction}, Rotation: {targetRotation.eulerAngles}");
        }
    }
}