using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// �{�v���C���[
public class PlayerCController : MonoBehaviour
{
    const KeyCode jumpKey = KeyCode.Space; // �W�����v�L�[

    public float moveSpeed = 6; // �ړ����x
    public float turnSpeed = 3; // ���񑬓x
    bool isMoving; // �ړ����t���O

    public float jumpSpeed = 4; // �W�����v��
    float maxJumpHeight; // �W�����v���̍ō��_
    bool isJumping; // �W�����v�J�n�t���O

    public float hoverTime = 1.5f; // �z�o�����O�\����
    float startHoverTime; // �z�o�����O�J�n����
    float hoveringWaitTime = 0.5f; // �z�o�����O�ڍs�҂�����
    bool isHovering; // �z�o�����O�J�n�t���O

    public float maxUpperPos = 6; // �ő�Y�㏸��
    //float currentYPos; // �W�����v����Y���W
    bool isUpper; // �㏸���t���O

    public LayerMask groundLayer; // �ڒn����Ώۃ��C���[

    bool isGrounded; // �ڒn���t���O
    Vector3 move; // �ړ����W

    Rigidbody rbody;
    Animator anime;

    // Start is called before the first frame update
    void Start()
    {
        // �R���|�[�l���g�擾
        rbody = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();

        // �W�����v�̍ō��_���v�Z
        maxJumpHeight = (jumpSpeed * jumpSpeed) / (2f * Mathf.Abs(Physics.gravity.y));
    }

    // Update is called once per frame
    void Update()
    {
        // �ڒn���͍��E�L�[�Ő���
        float axisH = Input.GetAxis("Horizontal");
        transform.Rotate(0, axisH * turnSpeed * 100 * Time.deltaTime, 0);

        // ��L�[�őO�ړ�
        float axisV = Input.GetAxis("Vertical");
        move.z = axisV > 0.0f ? axisV * moveSpeed : 0;

        // �O���[�o�����W�ɕϊ����Ĉړ�
        // velocity�ɂ�Time.deltaTime�������Ȃ�
        Vector3 moveGlobal = transform.TransformDirection(new Vector3(0, rbody.velocity.y, move.z));
        rbody.velocity = moveGlobal;

        // �ڒn��
        if (isGrounded)
        {
            // �ړ����t���O�X�V
            isMoving = axisH != 0.0f || axisV != 0.0f;

            // �z�o�����O�t���O�X�V
            isHovering = false;

            // �㏸���t���O�X�V
            isUpper = false;

            // �ڒn���ɃL�[�������ꂽ��W�����v�J�n
            if (!isJumping && Input.GetKeyDown(jumpKey))
            {
                // �W�����v�J�n
                isJumping = true;

                // ������x�n�ʂ��痣��Ă���z�o�����O
                Invoke("StartHovering", hoveringWaitTime);
            }
        }
        // ��
        else
        {
            // �z�o�����O��
            if (isHovering)
            {
                // �z�o�����O�\���Ԃ��߂�����z�o�����O�I��
                if (Time.time - startHoverTime > hoverTime + hoveringWaitTime)
                {
                    isHovering = false;
                    rbody.useGravity = true;
                }
            }

            // �z�o�����O�w�����������Ƃ�
            if (Input.GetKey(jumpKey))
            {
                // �Ƃ肠�����H�΂���
                isUpper = true;

                // �z�o�����O���Ă����Ƃ��͂������㏸
                if (isHovering && transform.position.y < maxJumpHeight + maxUpperPos)
                {
                    rbody.useGravity = false;
                    rbody.velocity = new Vector3(rbody.velocity.x, Mathf.Abs(rbody.velocity.y) * 1.005f, rbody.velocity.z);
                }
                // �z�o�����O�ł��Ȃ��Ƃ��͂�����艺�~
                else
                {
                    rbody.useGravity = true;
                    if (rbody.velocity.y < 0.5f)
                    {
                        rbody.velocity = new Vector3(rbody.velocity.x, -0.5f, rbody.velocity.z);
                    }
                }
            }
            // �L�[�������ꂽ��z�o�����O����߂�
            else
            {
                rbody.useGravity = true;
                isUpper = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // �ڒn��������
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.4f);

        // �ڒn��
        if (isGrounded)
        {
            // �W�����v�A�j���؂�ւ�
            anime.SetBool("jump", false);

            // �ړ����A�j���؂�ւ�
            anime.SetBool("move", isMoving);
        }

        // �W�����v�J�n
        if (isJumping)
        {
            // �W�����v
            rbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);

            // �W�����v�I��
            isJumping = false;
        }

        // �z�o�����O��
        anime.SetBool("jump", isUpper);
    }

    // �z�o�����O�J�n
    void StartHovering()
    {
        // �z�o�����O�J�n���Ԃ��Z�b�g
        startHoverTime = Time.time;

        isHovering = true;
    }
}
