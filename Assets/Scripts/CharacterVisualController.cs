using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script bridges the parent and child game objects, specifically between PlayerController and animations
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

    // Called from animation events
    public void OnFireballAnimationEvent()
    {
        if (playerController != null && playerController.currentActiveSkill != null && projectileSpawner != null)
        {
            // Forward to player's skill handling method
            playerController.OnSkillAnimationEvent();
        }
    }

    // Called at the end of attack/skill animations
    public void OnAnimationComplete()
    {
        if (playerController != null)
        {
            playerController.ResetState();
        }
    }
}
