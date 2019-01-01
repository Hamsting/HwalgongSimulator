using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hwalgong
{
    public class CharacterMovement : MonoBehaviour
    {
        private const float YVelocityAirResistance = 0.90f;
        private const float IsGroundMaxRayDistance = 0.03f;
        private const float IsGroundDuration = 0.2f;

        public CharacterController target;
        public Animator targetAnimator;
        public float baseMoveSpeed = 4.5f;
        public float baseRunAcceleration = 2.25f;
        public float baseJumpSpeed = 0.35f;

        [SerializeField]
        private bool isGround = false;
        private Vector3 moveVelocity = Vector3.zero;
        private Vector3 multiJumpVelocity = Vector3.zero;
        private Vector3 resVelocity = Vector3.zero;
        private float yVelocity = 0.0f;
        private float isGroundTimer = 0.0f;
        private float isGroundRadius = 0.0f;

        public GameObject testPoopPrefab;
        public List<GameObject> poops = new List<GameObject>();



        private void Awake()
        {
            if (target == null)
            {
                Debug.Log("CharacterMovement::The target CharacterController is null.");
                this.enabled = false;
                return;
            }
            
            isGroundRadius = target.radius - 0.01f;
        }

        private void FixedUpdate()
        {
            UpdateIsGround();

            float zAxis = Input.GetAxis("Vertical");
            float xAxis = Input.GetAxis("Horizontal");
            Vector3 localDirection = target.transform.TransformDirection(new Vector3(xAxis, 0.0f, zAxis));
            moveVelocity = localDirection * baseMoveSpeed * Time.deltaTime;

            float runAxis = Input.GetAxis("Run");
            if (runAxis > 0.0f)
                moveVelocity *= baseRunAcceleration;

            UpdateMultiJump();
            if (!isGround)
            {
                yVelocity += Physics.gravity.y * (1.0f - YVelocityAirResistance) * Time.deltaTime;
                isGroundTimer += Time.deltaTime;
            }
            else
            {
                yVelocity = 0.0f;
                isGroundTimer = 0.0f;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGround || isGroundTimer <= IsGroundDuration)
                {
                    yVelocity = baseJumpSpeed;
                    isGroundTimer = IsGroundDuration + 1.0f;
                }
                else
                {
                    yVelocity = baseJumpSpeed;
                    Vector3 multiJumpDirection = target.transform.right * xAxis + target.transform.forward * zAxis;
                    multiJumpVelocity += multiJumpDirection * baseMoveSpeed * 10.5f;
                }
            }

            resVelocity = moveVelocity + new Vector3(0.0f, yVelocity, 0.0f) + (multiJumpVelocity * Time.deltaTime);
            target.Move(resVelocity);

            UpdateAnimator();

            if (Input.GetKeyDown(KeyCode.T))
            {
                GameObject poop = GameObject.Instantiate<GameObject>(testPoopPrefab, target.transform.position, target.transform.rotation);
                poops.Add(poop);
            }
        }

        private void OnGUI()
        {
            foreach (GameObject p in poops)
            {
                float dis = Vector3.Distance(target.transform.position, p.transform.position);
                Vector3 scrnPos = Camera.main.WorldToScreenPoint(p.transform.position);

                GUI.Label(new Rect(scrnPos.x - 50.0f, Screen.height - scrnPos.y - 5.0f, scrnPos.x + 50.0f, Screen.height - scrnPos.y + 5.0f), dis.ToString("F1") + "m");
            }
        }

        private void UpdateIsGround()
        {
            // Ray ray = new Ray(target.transform.position, Vector3.down);
            // isGround = Physics.Raycast(ray, IsGroundMaxRayDistance);
            Ray ray = new Ray(target.transform.position + new Vector3(0.0f, isGroundRadius, 0.0f), Vector3.down);
            isGround = Physics.SphereCast(ray, isGroundRadius, IsGroundMaxRayDistance);
        }

        private void UpdateAnimator()
        {
            if (targetAnimator == null)
                return;

            float zAxis = Input.GetAxis("Vertical");
            float xAxis = Input.GetAxis("Horizontal");
            Vector3 vel = new Vector3(xAxis, 0.0f, zAxis) * baseMoveSpeed;
            
            float runAxis = Input.GetAxis("Run");
            if (runAxis > 0.0f)
                vel *= baseRunAcceleration;

            targetAnimator.SetFloat("XVelocity", vel.x);
            targetAnimator.SetFloat("YVelocity", vel.y);
            targetAnimator.SetFloat("ZVelocity", vel.z);
        }

        private void UpdateMultiJump()
        {
            if (isGround)
                multiJumpVelocity = Vector3.zero;

            if (multiJumpVelocity.magnitude > 0.0f)
            {
                multiJumpVelocity = Vector3.Lerp(multiJumpVelocity, Vector3.zero, Time.deltaTime);
                if (multiJumpVelocity.magnitude < 0.05f)
                    multiJumpVelocity = Vector3.zero;
            }
        }
    }
}