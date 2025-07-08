using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 鶏プレイヤー
public class PlayerCController : MonoBehaviour
{
    const KeyCode jumpKey = KeyCode.Space; // ジャンプキー

    public float moveSpeed = 6; // 移動速度
    public float turnSpeed = 3; // 旋回速度
    bool isMoving; // 移動中フラグ

    public float jumpSpeed = 4; // ジャンプ力
    float maxJumpHeight; // ジャンプ時の最高点
    bool isJumping; // ジャンプ開始フラグ

    public float hoverTime = 1.5f; // ホバリング可能時間
    float startHoverTime; // ホバリング開始時間
    float hoveringWaitTime = 0.5f; // ホバリング移行待ち時間
    bool isHovering; // ホバリング開始フラグ

    public float maxUpperPos = 6; // 最大Y上昇幅
    //float currentYPos; // ジャンプ中のY座標
    bool isUpper; // 上昇中フラグ

    public LayerMask groundLayer; // 接地判定対象レイヤー

    bool isGrounded; // 接地中フラグ
    Vector3 move; // 移動座標

    Rigidbody rbody;
    Animator anime;

    // Start is called before the first frame update
    void Start()
    {
        // コンポーネント取得
        rbody = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();

        // ジャンプの最高点を計算
        maxJumpHeight = (jumpSpeed * jumpSpeed) / (2f * Mathf.Abs(Physics.gravity.y));
    }

    // Update is called once per frame
    void Update()
    {
        // 接地中は左右キーで旋回
        float axisH = Input.GetAxis("Horizontal");
        transform.Rotate(0, axisH * turnSpeed * 100 * Time.deltaTime, 0);

        // 上キーで前移動
        float axisV = Input.GetAxis("Vertical");
        move.z = axisV > 0.0f ? axisV * moveSpeed : 0;

        // グローバル座標に変換して移動
        // velocityにはTime.deltaTimeをかけない
        Vector3 moveGlobal = transform.TransformDirection(new Vector3(0, rbody.velocity.y, move.z));
        rbody.velocity = moveGlobal;

        // 接地中
        if (isGrounded)
        {
            // 移動中フラグ更新
            isMoving = axisH != 0.0f || axisV != 0.0f;

            // ホバリングフラグ更新
            isHovering = false;

            // 上昇中フラグ更新
            isUpper = false;

            // 接地中にキーが押されたらジャンプ開始
            if (!isJumping && Input.GetKeyDown(jumpKey))
            {
                // ジャンプ開始
                isJumping = true;

                // ある程度地面から離れてからホバリング
                Invoke("StartHovering", hoveringWaitTime);
            }
        }
        // 空中
        else
        {
            // ホバリング中
            if (isHovering)
            {
                // ホバリング可能時間を過ぎたらホバリング終了
                if (Time.time - startHoverTime > hoverTime + hoveringWaitTime)
                {
                    isHovering = false;
                    rbody.useGravity = true;
                }
            }

            // ホバリング指示があったとき
            if (Input.GetKey(jumpKey))
            {
                // とりあえず羽ばたく
                isUpper = true;

                // ホバリングしていいときはゆっくり上昇
                if (isHovering && transform.position.y < maxJumpHeight + maxUpperPos)
                {
                    rbody.useGravity = false;
                    rbody.velocity = new Vector3(rbody.velocity.x, Mathf.Abs(rbody.velocity.y) * 1.005f, rbody.velocity.z);
                }
                // ホバリングできないときはゆっくり下降
                else
                {
                    rbody.useGravity = true;
                    if (rbody.velocity.y < 0.5f)
                    {
                        rbody.velocity = new Vector3(rbody.velocity.x, -0.5f, rbody.velocity.z);
                    }
                }
            }
            // キーが離されたらホバリングをやめる
            else
            {
                rbody.useGravity = true;
                isUpper = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // 接地中か判定
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.4f);

        // 接地中
        if (isGrounded)
        {
            // ジャンプアニメ切り替え
            anime.SetBool("jump", false);

            // 移動中アニメ切り替え
            anime.SetBool("move", isMoving);
        }

        // ジャンプ開始
        if (isJumping)
        {
            // ジャンプ
            rbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);

            // ジャンプ終了
            isJumping = false;
        }

        // ホバリング中
        anime.SetBool("jump", isUpper);
    }

    // ホバリング開始
    void StartHovering()
    {
        // ホバリング開始時間をセット
        startHoverTime = Time.time;

        isHovering = true;
    }
}
