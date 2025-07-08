using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// �{�v���C���[
public class PlayerCController : MonoBehaviour
{
    const KeyCode jumpKey = KeyCode.Space; // �W�����v�L�[

    public float moveSpeed = 4; // �ړ����x
    public float turnSpeed = 3; // ���񑬓x
    bool isMoving; // �ړ����t���O

    public float jumpSpeed = 4; // �W�����v��
    float maxJumpHeight; // �W�����v���̍ō��_
    bool isJumping; // �W�����v�J�n�t���O

    public float maxUpperPos = 3; // �ő�Y�㏸��
    float currentYPos; // �W�����v�O��Y���W

    public float hoverTime = 2; // �z�o�����O�\����
    float startHoverTime; // �z�o�����O�J�n����
    float hoveringWaitTime = 0.5f; // �z�o�����O�ڍs�҂�����
    bool isHovering; // �z�o�����O�J�n�t���O
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

                // �W�����v�O�̍�����ۑ�
                currentYPos =�@transform.position.y;

                // ������x�n�ʂ��痣��Ă���z�o�����O
                Invoke("StartHovering", hoveringWaitTime);
            }
        }
        // ��
        else
        {
            // �������Ȃ�
            isMoving = false;

            // �z�o�����O�\���ԓ�
            if (isHovering)
            {
                // �z�o�����O�\���Ԃ��߂�����z�o�����O�I��
                if (Time.time - startHoverTime > hoverTime + hoveringWaitTime)
                {
                    //Debug.Log("�z�o�����O�I��");
                    rbody.useGravity = true;
                    isUpper = false;
                    isHovering = false;
                }
                else
                {
                    // �㏸����Ƃ�
                    if (Input.GetKey(jumpKey))
                    {
                        // �������������Ȃ�������㏸
                        //Debug.Log("�z�o�����O��");
                        rbody.useGravity = false;
                        float y = transform.position.y < currentYPos + maxJumpHeight + maxUpperPos ? Mathf.Abs(rbody.velocity.y) * 1.005f : 0;
                        rbody.velocity = new Vector3(rbody.velocity.x, y, rbody.velocity.z);
                        isUpper = true;
                    }
                    // �L�[�������ꂽ��z�o�����O����߂�
                    else
                    {
                        //Debug.Log("�z�o�����O���f");
                        rbody.useGravity = true;
                        isUpper = false;
                    }
                }
            }
        }

        // ������藎�Ƃ�?
        if (rbody.velocity.y < -1f)
        {
            rbody.velocity = new Vector3(rbody.velocity.x, -1f, rbody.velocity.z);
        }
    }

    private void FixedUpdate()
    {
        // �ڒn��������
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.4f);

        // �ړ����A�j���؂�ւ�
        anime.SetBool("move", isMoving);

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
