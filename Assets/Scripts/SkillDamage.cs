using System.Collections;
using System.Collections.Generic;
using SmallScaleInc.TopDownPixelCharactersPack1;
using UnityEngine;

public class SkillDamage : MonoBehaviour
{
    [SerializeField] float destroyDelay = 0.3f;

    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        Physics.IgnoreLayerCollision(6, 9);
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.transform.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            float dist = Vector3.Distance(enemy.transform.position, playerController.transform.position);

            if (dist >= playerController.skill.skillRange)
            {
                enemy.TakeDamage(playerController.skill.skillDamage);
                //PlayHitEffect();
                Destroy(gameObject, destroyDelay);
            }

        }
        else
        {
            Debug.Log("Enemy is not found");
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
