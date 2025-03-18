using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpawnAttackPrefab : MonoBehaviour
{
    [SerializeField] GameObject attackPrefab;

    private float projectileSpeed;
    private Camera mainCamera;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        playerController = GetComponentInParent<PlayerController>();
    }

    public void SpawnPrefabForAttack()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPoint = hit.point;
            Vector3 direction = (targetPoint - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);

            if (playerController.activeOrbs.Count != playerController.maxOrbsCount) return;

            ParticleSystem ps = attackPrefab.GetComponentInChildren<ParticleSystem>();
            var main = ps.main;
            main.startColor = Color.clear;

            if (playerController.activeOrbs.Contains(Orb.Quas) && playerController.activeOrbs.Contains(Orb.Wex))
            {
                main.startColor = Color.magenta;
            }
            else if (playerController.activeOrbs.Contains(Orb.Quas) && !playerController.activeOrbs.Contains(Orb.Wex))
            {
                main.startColor = (Color.red + Color.yellow) / 2;
            }
            else if (playerController.activeOrbs.Contains(Orb.Wex) && !playerController.activeOrbs.Contains(Orb.Quas))
            {
                main.startColor = Color.cyan;
            }

            GameObject attack = Instantiate(attackPrefab, transform.position, targetRotation);

            Rigidbody attackRb = attack.GetComponent<Rigidbody>();

            if (attack != null)
            {
                direction.y = 0;
                attackRb.velocity = direction * playerController.attackProjectileSpeed;
                attackRb.useGravity = false;
                attackRb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
}
