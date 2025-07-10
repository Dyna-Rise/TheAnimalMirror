using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float groundCheckDistance = 0.1f; 

    private Rigidbody rb;
    private bool isGrounded;

    float axisH;
    float axisV;

    Animator anime;

    public bool isDamage = false;

    bool isDeadAnime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        if (!(GameController.gameState == GameState.playing || GameController.gameState == GameState.hidden))
        {
            if (GameController.gameState == GameState.gameover && !isDeadAnime)
            {
                anime.SetTrigger("GameOver");
                isDeadAnime = true;
            }
            return;
        }

        if (isDamage) return;

        // ���͎擾
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(axisH, 0, axisV).normalized;

        // �A�j���[�V����
        anime.SetBool("move", move.magnitude > 0);

        // �����ύX
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        // �W�����v���́iUpdate�œ��͌��o�j
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(Physics.gravity.y));
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
            anime.SetTrigger("jump");
        }
    }

    void FixedUpdate()
    {
        if (isDamage) return;

        // �n�ʔ���i�J�v�Z���̒ꂩ�牺��Ray���΂��j
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f);

        // �ړ��iFixedUpdate�ŕ����ړ��j
        Vector3 move = new Vector3(axisH, 0, axisV).normalized;
        Vector3 velocity = move * moveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // Y�͈ێ�
    }
}

