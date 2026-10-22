using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Move")]
    public Rigidbody2D rb;
    private float xInput;
    public float moveSpeed = 5;
    public Animator playerAnimator;
    [HideInInspector]public bool isRight = true;
    public SpriteRenderer playerSprite;

    [Header("Player Jump")]
    public float jumpForce = 300;
    public GameObject playerGO;
    public LayerMask groundLayer;
    [HideInInspector] public bool isGround = true;
    [HideInInspector] public bool isJump = false;
    [HideInInspector] public bool isAir = false;

    [Header("Player Dash")]
    public float dashSpeed = 10f;
    public float dashTime = 0.2f;
    public float dashCoolDownTime = 1f;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool canDash = true;

    [Header("Player Attack")]
    public Transform attackPoint1;
    public Transform attackPoint12;
    public Transform attackPoint2;
    public Transform attackPoint22;
    public Transform attackPoint3;
    public Transform attackPoint32;
    public GameObject attackBox;
    [HideInInspector] public int combo = 1;
    [HideInInspector] public bool isAttack = false;
    public float attackAddForce = 300.0f;

    [Header("Player Attributes")]
    public float HP = 100f;
    public float HPNow = 100f;
    public float ATK = 10f;

    void Start()
    {
        Application.targetFrameRate = 60;
        HPNow = HP;
    }

    void Update()
    {
        PlayerMove();
        PlayerJump();
        CheckGround();
        PlayerDash();
        PlayerAttack();
    }

    void FixedUpdate()
    {
        PlayerMoveFix();
    }
    public void PlayerMove()
    {
        if (!isAttack) {
            xInput = Input.GetAxisRaw("Horizontal");
            if (xInput > 0)
            {
                isRight = true;
                playerSprite.flipX = false;
                playerAnimator.SetBool("isRun", true);
            }
            else if (xInput < 0)
            {
                isRight = false;
                playerSprite.flipX = true;
                playerAnimator.SetBool("isRun", true);
            }
            else if (!(xInput != 0) && playerAnimator.GetBool("isRun") == true)
            {
                playerAnimator.SetBool("isRun", false);
            }
        }
    }

    public void PlayerMoveFix()
    {
        if (!isAttack)
        {
            if (isDashing)
            {
                rb.velocity = new Vector2((isRight ? 1 : -1) * dashSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    public void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround) 
            {
                rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
                playerAnimator.SetTrigger("isJump");
                Invoke("SetIsJump",0.1f);
            }
        }
        if (!isGround && rb.velocity.y < 0 && !playerAnimator.GetBool("isAir") && !isDashing)
        {
            playerAnimator.SetBool("isAir", true);
            isAir = true;
        }
        if (isGround && (isJump || isAir))
        {
            playerAnimator.SetTrigger("isGround");
            playerAnimator.SetBool("isAir", false);
            isJump = false;
            isAir = false;
        }         
    }

    public void CheckGround()
    {
        Vector2 startPos = playerGO.transform.position;
        Vector2 endPod = playerGO.transform.position + Vector3.down*0.7f;
        RaycastHit2D hit = Physics2D.Linecast(startPos, endPod,groundLayer);
        if(hit.collider != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }

    public void SetIsJump()
    {
        isJump = true;
    }

    public void PlayerDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            if (!isDashing && canDash)
            {
                isDashing = true;
                canDash = false;
                playerAnimator.SetBool("isDash", true);
                Invoke(nameof(PlayerDashEnd),dashTime);
                Invoke(nameof(DashCoolDown), dashCoolDownTime);
            }
        }
    }

    public void PlayerDashEnd()
    {
        isDashing = false;
        playerAnimator.SetBool("isDash", false);
    }

    public void DashCoolDown()
    {
        canDash = true;
    }

    public void PlayerAttack()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttack && !(isJump || isAir))
        {
            isAttack = true;
            CancelInvoke(nameof(ComboReset));
            Invoke(nameof(ComboReset), 2f);
            switch (combo)
            {
                case 1:
                    {
                        playerAnimator.SetTrigger("Attack1");
                        Invoke(nameof(CreatCombo1AttackBox), 0.4f);
                        Invoke(nameof(AttackEnd), 0.5f);
                        combo = 2;
                        rb.AddForce(new Vector2((isRight ? 1 : -1) * attackAddForce, 0));
                        return;
                    }
                case 2:
                    {
                        playerAnimator.SetTrigger("Attack2");
                        Invoke(nameof(CreatCombo2AttackBox), 0.2f);
                        Invoke(nameof(AttackEnd), 0.35f);
                        combo = 3;
                        rb.AddForce(new Vector2((isRight ? 1 : -1) * attackAddForce, 0));
                        return;
                    }
                case 3:
                    {
                        playerAnimator.SetTrigger("Attack3");
                        Invoke(nameof(CreatCombo3AttackBox), 0.3f);
                        Invoke(nameof(AttackEnd), 0.5f);
                        combo = 1;
                        rb.AddForce(new Vector2((isRight ? 1 : -1) * attackAddForce, 0));
                        return;
                    }
            }
        }
    }

    public void AttackEnd()
    {
        isAttack = false;
    }

    public void ComboReset()
    {
        combo = 1;
    }

    public void CreatCombo1AttackBox()
    {
        if (isRight)
        {
            GameObject go = Instantiate(attackBox, attackPoint1.position, attackPoint1.rotation,transform);
            go.transform.localScale = attackPoint1.localScale;
        }
        else
        {
            GameObject go = Instantiate(attackBox, attackPoint12.position, attackPoint12.rotation,transform);
            go.transform.localScale = attackPoint12.localScale;
        }
    }

    public void CreatCombo2AttackBox()
    {
        if (isRight)
        {
            GameObject go = Instantiate(attackBox, attackPoint2.position, attackPoint2.rotation,transform);
            go.transform.localScale = attackPoint2.localScale;
        }
        else
        {
            GameObject go = Instantiate(attackBox, attackPoint22.position, attackPoint22.rotation,transform);
            go.transform.localScale = attackPoint22.localScale;
        }
    }

    public void CreatCombo3AttackBox()
    {
        if (isRight)
        {
            GameObject go = Instantiate(attackBox, attackPoint3.position, attackPoint3.rotation, transform);
            go.transform.localScale = attackPoint3.localScale;
        }
        else
        {
            GameObject go = Instantiate(attackBox, attackPoint32.position, attackPoint32.rotation, transform);
            go.transform.localScale = attackPoint32.localScale;
        }
    }

    public void GetHit(float demage)
    {

    }
}
