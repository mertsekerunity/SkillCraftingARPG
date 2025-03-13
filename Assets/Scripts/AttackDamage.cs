using System.Collections;
using System.Collections.Generic;
using SmallScaleInc.TopDownPixelCharactersPack1;
using UnityEngine;

public class AttackDamage : MonoBehaviour
{
    [SerializeField] float destroyDelay = 0.005f;

    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        Physics.IgnoreLayerCollision(6, 9);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<EnemyHealth>() != null)
        {
            EnemyHealth enemy = other.transform.GetComponent<EnemyHealth>();

            float dist = Vector3.Distance(enemy.transform.position, playerController.transform.position);

            if (playerController.attackRange >= dist)
            {
                enemy.TakeDamage(playerController.attackDamage);
                //PlayHitEffect();
                Destroy(gameObject,destroyDelay);
            }
        }
        //else
        //{
        //    Debug.Log("Enemy is not found");
        //}

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject,destroyDelay);
        }
    }
}
