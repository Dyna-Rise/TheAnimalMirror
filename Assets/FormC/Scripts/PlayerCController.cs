using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 鶏プレイヤー
public class PlayerCController : MonoBehaviour
{
    const KeyCode jumpKey = KeyCode.Space; // ジャンプキー

    public float moveSpeed = 4; // 移動速度
    public float turnSpeed = 3; // 旋回速度
    bool isMoving; // 移動中フラグ

    public float jumpSpeed = 4; // ジャンプ力
    float maxJumpHeight; // ジャンプ時の最高点
    bool isJumping; // ジャンプ開始フラグ

    public float maxUpperPos = 3; // 最大Y上昇幅
    float currentYPos; // ジャンプ前のY座標

    public float hoverTime = 2; // ホバリング可能時間
    float startHoverTime; // ホバリング開始時間
    float hoveringWaitTime = 0.5f; // ホバリング移行待ち時間
    bool isHovering; // ホバリング開始フラグ
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

                // ジャンプ前の高さを保存
                currentYPos =　transform.position.y;

                // ある程度地面から離れてからホバリング
                Invoke("StartHovering", hoveringWaitTime);
            }
        }
        // 空中
        else
        {
            // 歩かせない
            isMoving = false;

            // ホバリング可能時間内
            if (isHovering)
            {
                // ホバリング可能時間を過ぎたらホバリング終了
                if (Time.time - startHoverTime > hoverTime + hoveringWaitTime)
                {
                    //Debug.Log("ホバリング終了");
                    rbody.useGravity = true;
                    isUpper = false;
                    isHovering = false;
                }
                else
                {
                    // 上昇するとき
                    if (Input.GetKey(jumpKey))
                    {
                        // 高さが制限内ならゆっくり上昇
                        //Debug.Log("ホバリング中");
                        rbody.useGravity = false;
                        float y = transform.position.y < currentYPos + maxJumpHeight + maxUpperPos ? Mathf.Abs(rbody.velocity.y) * 1.005f : 0;
                        rbody.velocity = new Vector3(rbody.velocity.x, y, rbody.velocity.z);
                        isUpper = true;
                    }
                    // キーが離されたらホバリングをやめる
                    else
                    {
                        //Debug.Log("ホバリング中断");
                        rbody.useGravity = true;
                        isUpper = false;
                    }
                }
            }
        }

        // ゆっくり落とす?
        if (rbody.velocity.y < -1f)
        {
            rbody.velocity = new Vector3(rbody.velocity.x, -1f, rbody.velocity.z);
        }
    }

    private void FixedUpdate()
    {
        // 接地中か判定
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.4f);

        // 移動中アニメ切り替え
        anime.SetBool("move", isMoving);

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
