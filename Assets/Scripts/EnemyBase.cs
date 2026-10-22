using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Pursuit,
    Attack,
    GetHit,
    Death
}

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Idle")]
    public float idleTime = 2f;
    public SpriteRenderer spriteRenderer;
    public Animator enemyAnimator;

    [Header("Enemy Patrol")]
    public EnemyState currentState = EnemyState.Patrol;
    public Transform left;
    public Transform right;
    public Rigidbody2D rb;
    [HideInInspector] public bool isRight = false;
    public float enemySpeed = 1f;
    [HideInInspector] public bool canMove = true;

    [Header("Enemy Pursuit")]
    public float enemyAttackDis = 0.6f;
    [HideInInspector] public GameObject player;

    [Header("Enemy Attack")]
    public float attackTime = 0.5f;
    public float attackCoolTime = 2.0f;
    public float attackCreatBoxTime = 0.6f;
    private bool canAttack = true;
    private bool isAttacking = false;
    public GameObject attackBox;
    public Transform attackPoint1;
    public Transform attackPoint12;

    [Header("Enemy Attributes")]
    public float HP = 100f;
    public float HPNow = 100f;
    public float ATK = 10f;

    [Header("Enemy GetHit")]
    public float getHitTime = 0.5f;
    [HideInInspector] public bool isGetHit = false;
    public float getHitAddForce = 300.0f;
    public GameObject EnemyAndPosition;
    public virtual void Start()
    {
        PatrolEnter();
        HPNow = HP;
    }

    public virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle: IdleUpdate(); break;
            case EnemyState.Patrol: PatrolUpdate(); break;
            case EnemyState.Pursuit: PursuitUpdate(); break;
            case EnemyState.Attack: AttackUpdate(); break;
            case EnemyState.GetHit: GetHitUpdate(); break;
            case EnemyState.Death: DeathUpdate(); break;
        }
    }

    public virtual void FixedUpdate()
    {
        if (canMove) 
        {
            rb.velocity = new Vector2((isRight ? 1 : -1) * enemySpeed,0);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    #region ×´Ì¬»ú
    //´ý»ú
    public virtual void IdleEnter()
    {
        canMove = false;
        enemyAnimator.SetBool("isRun", false);
        Invoke(nameof(ChangeIdleToPatrolState), idleTime);
    }
    public virtual void IdleUpdate()
    {

    }
    public virtual void IdleExit()
    {

    }
    //Ñ²Âß
    public virtual void PatrolEnter()
    {
        canMove = true;
        enemyAnimator.SetBool("isRun", true);
    }
    public virtual void PatrolUpdate()
    {
        if(transform.position.x <= left.position.x && !isRight)
        {
            ChangecurrentState(EnemyState.Idle);
        }
        else if(transform.position.x >= right.position.x && isRight)
        {
            ChangecurrentState(EnemyState.Idle);
        }
    }
    public virtual void PatrolExit()
    {

    }
    //×·»÷
    public virtual void PursuitEnter()
    {
        CancelInvoke(nameof(ChangeIdleToPatrolState));
        canMove = true;
        enemyAnimator.SetBool("isRun",true);
    }
    public virtual void PursuitUpdate()
    {
        if (player != null)
        {
            if (player.transform.position.x <= transform.position.x)
            {
                isRight = false;
                spriteRenderer.flipX = isRight;
            }
            else
            {
                isRight = true;
                spriteRenderer.flipX = isRight;
            }

            if (Mathf.Abs(player.transform.position.x - transform.position.x) <= enemyAttackDis)
            { 
                if (player.transform.position.x <= transform.position.x && !isRight)
                {
                    ChangecurrentState(EnemyState.Attack);
                }
                else if (player.transform.position.x > transform.position.x && isRight)
                {
                    ChangecurrentState(EnemyState.Attack);
                }
            }
        }
    }
    public virtual void PursuitExit()
    {

    }
    //¹¥»÷
    public virtual void AttackEnter()
    {
        canMove= false;
        enemyAnimator.SetBool("isRun", false);
    }
    public virtual void AttackUpdate()
    {
        if (canAttack && !isAttacking && !isGetHit)
        {
            canAttack = false;
            isAttacking = true;
            enemyAnimator.SetTrigger("Attack1");
            Invoke(nameof(CreatAttackBox), attackCreatBoxTime);
            Invoke(nameof(SetCanAttack),attackCoolTime);
            Invoke(nameof(SetIsAttacking), attackTime);
        }

        if (!isAttacking)
        {
            if (Mathf.Abs(player.transform.position.x - transform.position.x) > enemyAttackDis)
            {
                ChangecurrentState(EnemyState.Pursuit);
            }
            else
            {
                if (player.transform.position.x >= transform.position.x && !isRight)
                {
                    ChangecurrentState(EnemyState.Pursuit);
                }
                else if (player.transform.position.x < transform.position.x && isRight)
                {
                    ChangecurrentState(EnemyState.Pursuit);
                }
            }
        }
    }
    public virtual void AttackExit()
    {

    }
    //ÊÜÉË
    public virtual void GetHitEnter()
    {
        canMove = false;
        CancelInvoke(nameof(CreatAttackBox));
        CancelInvoke(nameof(ChangeStateToPursuit));
        enemyAnimator.SetBool("GetHit", true);
        enemyAnimator.SetBool("isRun", false);
        Invoke(nameof(ChangeStateToPursuit), getHitTime);
        if(player != null)
        {
            if (player.transform.position.x <= transform.position.x)
            {
                rb.AddForce(new Vector2(getHitAddForce, 0));
            }
            else
            {
                rb.AddForce(new Vector2(-getHitAddForce, 0));
            }
        }
    }
    public virtual void GetHitUpdate()
    {
        if (!isGetHit)
        {
            isGetHit = true;
        }
    }
    public virtual void GetHitExit()
    {
        isGetHit = false;
        enemyAnimator.SetBool("GetHit", false);
    }
    //ËÀÍö
    public virtual void DeathEnter()
    {
        CancelInvoke(nameof(CreatAttackBox));
        CancelInvoke(nameof(ChangeStateToPursuit));
        enemyAnimator.SetBool("isRun", false);
        enemyAnimator.SetTrigger("isDead");
        canMove = false;
        Destroy(EnemyAndPosition, 3.0f);
    }
    public virtual void DeathUpdate()
    {

    }
    public virtual void DeathExit()
    {

    }
    #endregion

    public virtual void ChangecurrentState(EnemyState newState)
    {
        switch (currentState)
        {
            case EnemyState.Idle: IdleExit(); break;
            case EnemyState.Patrol:PatrolExit(); break;
            case EnemyState.Pursuit:PursuitExit(); break;
            case EnemyState.Attack:AttackExit(); break;
            case EnemyState.GetHit:GetHitExit(); break;
            case EnemyState.Death:DeathExit(); break;
        }
        currentState = newState;
        switch (currentState)
        {
            case EnemyState.Idle: IdleEnter(); break;
            case EnemyState.Patrol: PatrolEnter(); break;
            case EnemyState.Pursuit: PursuitEnter(); break;
            case EnemyState.Attack: AttackEnter(); break;
            case EnemyState.GetHit: GetHitEnter(); break;
            case EnemyState.Death: DeathEnter(); break;
        }
    }

    public virtual void ChangeIdleToPatrolState()
    {
        isRight = !isRight;
        spriteRenderer.flipX = isRight;
        ChangecurrentState(EnemyState.Patrol);
    }

    public virtual void FindPlayer(GameObject mainPlayer)
    {
        if (currentState != EnemyState.Death)
        {
            player = mainPlayer;
            ChangecurrentState(EnemyState.Pursuit);
        }
    }

    public virtual void PlayerOut()
    {
        if (currentState != EnemyState.Death)
        {
            ChangecurrentState(EnemyState.Patrol);
        }
    }

    public virtual void SetCanAttack()
    {
        canAttack = true;
    }

    public virtual void SetIsAttacking()
    {
        isAttacking = false;
    }

    public virtual void GetHit(float demage)
    {
        if (currentState != EnemyState.Death)
        {
            ChangecurrentState(EnemyState.GetHit);
            HPNow = HPNow - demage;
            if(HPNow <= 0)
            {
                ChangecurrentState(EnemyState.Death);
            }
        }
    }

    public virtual void ChangeStateToPursuit()
    {
        if(currentState != EnemyState.Death)
        {
            ChangecurrentState(EnemyState.Pursuit);
        }
    }

    public virtual void CreatAttackBox()
    {
        if(currentState != EnemyState.Death && currentState != EnemyState.GetHit)
        {
            if (isRight)
            {
                GameObject go = Instantiate(attackBox, attackPoint12.position, attackPoint12.rotation, transform);
                go.transform.localScale = attackPoint12.localScale;
            }
            else
            {
                GameObject go = Instantiate(attackBox, attackPoint1.position, attackPoint1.rotation, transform);
                go.transform.localScale = attackPoint1.localScale;
            }
        }
    }

}