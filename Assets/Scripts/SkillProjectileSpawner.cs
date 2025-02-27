using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillProjectileSpawner : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject fireballPrefab;

    void OnFireball()
    {
        var fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

        ParticleSystem fireballEffect = fireball.GetComponentInChildren<ParticleSystem>();

        fireballEffect.transform.rotation = Quaternion.LookRotation(playerController.lastUsingSkillDirection);

        fireballEffect.Play();
    }
}
