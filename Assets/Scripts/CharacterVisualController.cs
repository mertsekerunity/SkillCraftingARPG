using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is basically a brdige between the (parent and child game objects, specifically, between PlayerController and the SkillProjectileSpawner)
public class CharacterVisualController : MonoBehaviour
{
    private PlayerController playerController;
    private SkillProjectileSpawner projectileSpawner;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        projectileSpawner = GetComponentInParent<SkillProjectileSpawner>();

        if (playerController == null)
        {
            Debug.LogError("No PlayerController found in parent hierarchy!");
        }

        if (projectileSpawner == null)
        {
            Debug.LogError("No SkillProjectileSpawner found in parent hierarchy!");
        }
    }

    public void OnFireballAnimationEvent()
    {
        if (playerController != null && playerController.currentActiveSkill != null && projectileSpawner != null)
        {
            projectileSpawner.SpawnProjectileForSkill(playerController.currentActiveSkill);
        }
    }
}
