using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// ��������ƶ�
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero; // ÿ�����ƶ��ľ���
    private Vector3 yRotation = Vector3.zero; // ��ת��ɫ
    private Vector3 xRotation = Vector3.zero; // ��ת�ӽ�
    private Vector3 thrusterForce = Vector3.zero; // ���ϵ�����

    private Vector3 lastFramePosition = Vector3.zero; // ��¼��һ֡��λ��
    private float eps = 0.01f;
    private Animator animator;
    private float distToGround = 0f;

    private void Start()
    {
        lastFramePosition = transform.position;
        animator = GetComponentInChildren<Animator>();

        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    private float cameraRoationTotal = 0f;

    private float recoilForce = 0f; // ������
    [SerializeField]
    private float cameraRotationLimit = 85f;

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Ratate(Vector3 _yRotation, Vector3 _xRotation)
    {
        yRotation = _yRotation;
        xRotation = _xRotation;
    }

    public void Thrust(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    public void AddRecoilForce(float newRecoilForce)
    {
        recoilForce += newRecoilForce;
    }

    private void PerformMovement()
    {
        
        if (velocity != Vector3.zero)
        {
            // fixedDeltaTime ָ������������ FixedUpdate ��ִ�м��

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce); // ���� Time.fixedDeltaTime ��
            thrusterForce = Vector3.zero;
        }

 
    }

    private void PerformRotation()
    {
        if (recoilForce < 0.1f)
        {
            recoilForce = 0f;
        }
        if (yRotation != Vector3.zero || recoilForce > 0)
        {
            rb.transform.Rotate(yRotation + rb.transform.up * Random.Range(-2f * recoilForce, 2f * recoilForce));
        }

        if (xRotation != Vector3.zero || recoilForce > 0)
        {
            cameraRoationTotal += xRotation.x - recoilForce;
            cameraRoationTotal = Mathf.Clamp(cameraRoationTotal, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(cameraRoationTotal, 0, 0);
        }
        recoilForce *= 0.5f;
    }

    private void PerformAnimation()
    {
        Vector3 deltaPosition = transform.position - lastFramePosition;
        lastFramePosition = transform.position;

        float forward = Vector3.Dot(deltaPosition, transform.forward);
        float right = Vector3.Dot(deltaPosition, transform.right);

        int direction = 0;
        if (forward > eps)
        {
            direction = 1;
        } else if (forward < -eps)
        {
            if (right > eps)
            {
                direction = 4; // �Һ�
            } else if (right < -eps)
            {
                direction = 6; // ���
            } else
            {
                direction = 5;// ��
            }
        } else if (right > eps)
        {
            direction = 3; // ��
        } else if (right < -eps)
        {
            direction = 7; // ��
        }

        if (!Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))
        {
            direction = 0;
        }

        if (GetComponent<Player>().IsDead())
        {
            direction = -1;
        }

        animator.SetInteger("direction", direction);
    }
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            PerformMovement();
            PerformRotation();
            PerformAnimation();
        }
    }

    private void Update()
    {
        if (!IsLocalPlayer)
        {
            PerformAnimation();
        }
    }
}