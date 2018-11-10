using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hwalgong
{
    public class CharacterMovement : MonoBehaviour
    {
        public float baseMoveSpeed = 3.0f;
        public float baseJumpForce = 400.0f;

        private Transform target;
        private Rigidbody rb;
        [SerializeField]
        private bool isGround = true;



        private void Awake()
        {
            target = this.transform;
            rb = target.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float zAxis = Input.GetAxis("Vertical");
            float xAxis = Input.GetAxis("Horizontal");

            Vector3 moveVec = new Vector3(xAxis, 0.0f, zAxis) * baseMoveSpeed * Time.deltaTime;
            target.Translate(moveVec, Space.Self);
            
            Ray ray = new Ray(target.position, Vector3.down);
            Debug.DrawRay(ray.origin, ray.direction * 0.2f);
            isGround = Physics.Raycast(ray, 0.2f, (1 << LayerMask.NameToLayer("Map")));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = Vector3.zero;
                if (isGround)
                    rb.AddForce(Vector3.up * baseJumpForce, ForceMode.Force);
                else
                    rb.AddForce((target.forward + new Vector3(0.0f, 0.5f, 0.0f)).normalized * baseJumpForce, ForceMode.Force);

                Debug.Log((target.forward + new Vector3(0.0f, 0.5f, 0.0f)).normalized);
            }
        }
    }
}