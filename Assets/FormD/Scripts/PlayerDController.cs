using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDController : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.gameState != GameState.playing)
        {
            if (GameController.gameState == GameState.gameover && !isDeadAnime)
            {
                anime.SetTrigger("GameOver");
                isDeadAnime = true;
            }
            return;
        }

        if (isDamage) return;

        // 入力取得
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(axisH, 0, axisV).normalized;

        // アニメーション
        anime.SetBool("move", move.magnitude > 0);

        // 向き変更
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        // ジャンプ入力（Updateで入力検出）
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

        // 地面判定（カプセルの底から下へRayを飛ばす）
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f);

        // 移動（FixedUpdateで物理移動）
        Vector3 move = new Vector3(axisH, 0, axisV).normalized;
        Vector3 velocity = move * moveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // Yは維持
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
