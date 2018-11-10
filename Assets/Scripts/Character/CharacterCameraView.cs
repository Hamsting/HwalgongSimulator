using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hwalgong
{
    public class CharacterCameraView : MonoBehaviour
    {
        public Transform camPivot;
        public Collider characterCollider;
        public float baseRotateSpeed = 5.0f;
        public float baseDistanceSpeed = 2.0f;
        public bool isInversedX = false;
        public bool isInversedY = false;

        private Camera cam;
        private Transform target;
        private bool viewEnabled = true;
        private float camDistance = 5.0f;
        private float camAngle = 45.0f;



        private void Awake()
        {
            cam = camPivot.GetComponentInChildren<Camera>();

            if (characterCollider == null)
                characterCollider = this.GetComponent<Collider>();

            if (characterCollider != null)
                target = characterCollider.transform;


            viewEnabled = !viewEnabled;
            Cursor.visible = !viewEnabled;
            Cursor.lockState = (viewEnabled) ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                viewEnabled = !viewEnabled;
                Cursor.visible = !viewEnabled;
                Cursor.lockState = (viewEnabled) ? CursorLockMode.Locked : CursorLockMode.None;
            }

            float mouseWheel = (viewEnabled) ? Input.GetAxis("Mouse ScrollWheel") : 0.0f;
            float mouseXAxis = (viewEnabled) ? Input.GetAxis("Mouse X") : 0.0f;
            float mouseYAxis = (viewEnabled) ? Input.GetAxis("Mouse Y") : 0.0f;
            if (isInversedX)
                mouseXAxis *= -1.0f;
            if (isInversedY)
                mouseYAxis *= -1.0f;

            camDistance += baseDistanceSpeed * mouseWheel;

            camAngle = Mathf.Clamp(camAngle - mouseYAxis * baseRotateSpeed * Time.deltaTime, -89.0f, 89.0f);

            Vector3 chaRot = target.eulerAngles;
            chaRot.y += mouseXAxis * baseRotateSpeed * Time.deltaTime;
            target.eulerAngles = chaRot;
            
            camPivot.position = characterCollider.bounds.center;
            camPivot.eulerAngles = new Vector3(camAngle, chaRot.y, 0.0f);

            Vector3 camPos = new Vector3(0.0f, 0.0f, -camDistance);

            Ray ray = new Ray(camPivot.position, -cam.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, camDistance, (1 << LayerMask.NameToLayer("Map"))))
                camPos.z = -hit.distance + 0.1f;

            cam.transform.localPosition = camPos;
        }
    }
}