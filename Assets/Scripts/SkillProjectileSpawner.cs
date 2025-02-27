using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject fireballPrefab;

    void OnFireball()
    {
        var fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fireball.GetComponentInChildren<ParticleSystem>().Play();
    }
}
