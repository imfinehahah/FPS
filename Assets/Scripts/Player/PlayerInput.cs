using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 读入输入信息
public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitity = 8f;
    [SerializeField]
    private float thrusterForce = 20f;
    [SerializeField]
    private PlayerController controller;

    private float distToGround = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal");
        float yMov = Input.GetAxisRaw("Vertical");

        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed; // 为了使各个方向运动的速度都一样
        controller.Move(velocity);

        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");

        Vector3 yRotation = new Vector3(0f, xMouse, 0f) * lookSensitity;
        Vector3 xRotation = new Vector3(-yMouse, 0f, 0f) * lookSensitity;

        controller.Ratate(yRotation, xRotation);

        if (Input.GetButton("Jump"))
        {
            if (Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))
            {
                Vector3 force = Vector3.up * thrusterForce;
                controller.Thrust(force);
            }
        }
    }
}
