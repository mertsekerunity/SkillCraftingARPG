using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float damage = 25f;
    public void OnAttack()
    {
        if (player == null || player.GetComponent<PlayerHealth>().IsPlayerDead) return;

        player.GetComponent<PlayerHealth>().TakeDamage(damage);

        //show damage impact??
    }

}
