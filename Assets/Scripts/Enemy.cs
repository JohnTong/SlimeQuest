using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;
    public int enemyHealth;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;

    private bool skipMove;

    protected override void Start()
    {
        GameManager.instance.AddEnemiesToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir , int yDir)
    {
        if(skipMove) {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir , yDir);
    }

    public void LoseHealth(int damage)
    {
        enemyHealth -= damage;
        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if(enemyHealth <= 0) {
            animator.SetTrigger("EnemyDeath");
            GameManager.instance.RemoveEnemyFromList(gameObject.GetComponent<Enemy>());
            gameObject.SetActive(false);
        }
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<PlayerPlayer>(xDir , yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        PlayerPlayer hitPlayer = component as PlayerPlayer;

        animator.SetTrigger("EnemyAttack");
        //SoundManager.instance.RandomizeSfx(enemyAttack1 , enemyAttack2);
        hitPlayer.LoseHealth(playerDamage);
    }
}
