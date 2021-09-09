using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace TFM.DynamicProcedures.Examples
{
    [RequireComponent(typeof(TransformOrbiter))]
    public class CameraOrbitBehaviour : MonoBehaviour
    {
        public static CameraOrbitBehaviour Active { get; private set; }

        private const float orbitSensitivity = 0.5f;
        private const float zoomSensitivity = 0.1f;

        [SerializeField]
        private Transform target;

        [SerializeField, HideInInspector]
        private TransformOrbiter orbiter;

        [SerializeField, Range(0, 1)]
        private float distanceLerp = 0.6f;

        [SerializeField]
        private float minimumDistance = 0.05f;

        [SerializeField]
        private float maximumDistance = Mathf.Infinity;

        private bool isRotating;

        private Vector2 lastMouse;



        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void RotateCamera(Vector2 parameter)
        {
            if(target == null) return;

            parameter *= orbitSensitivity;

            orbiter.Rotation += parameter.x;
            orbiter.Pitch += parameter.y;
        }

        public void ZoomCamera(float zoom)
        {
            if(target == null) return;

            distanceLerp += zoom * zoomSensitivity; // * Mathf.Clamp(distanceLerp * 0.5f, 0.01f, 0.6f);

            distanceLerp = Mathf.Clamp01(distanceLerp);
        }

        private void OnEnable()
        {
            orbiter.enabled = true;
            
            Active = this;
        }

        private void OnDisable()
        {
            orbiter.enabled = false;

            if(Active == this)
                Active = null;
        }

        private void LateUpdate()
        {
            if(target != null)
            {
                Vector3 point = target.position;
                orbiter.orbitPoint = point;

                orbiter.distance = Mathf.Lerp(minimumDistance, maximumDistance, distanceLerp);
            }
        }

        private void Update()
        {
            CheckRotationInput();

            if(isRotating) Rotate();

            ProcessScroll();
        }

        private void CheckRotationInput()
        {
            if(Input.GetMouseButtonDown(0))
            {
                // Ignore input start if hovering UI
                if(EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                isRotating = true;
                lastMouse = Input.mousePosition;
            }
            else if(Input.GetMouseButtonDown(1))
            {
                // Ignore input start if hovering UI
                if(EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                lastMouse = Input.mousePosition;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                if(isRotating) isRotating = false;
            }
        }

        private void Rotate()
        {
            float horizontal = (Input.mousePosition.x - lastMouse.x);
            float vertical = (Input.mousePosition.y - lastMouse.y);
            lastMouse = Input.mousePosition;

            RotateCamera(new Vector2(horizontal, -vertical));
        }

        private void ProcessScroll()
        {
            // Ignore input start if hovering UI
            if(EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            float scroll = Input.mouseScrollDelta.y;
            if(scroll != 0)
            {
                ZoomCamera(-scroll);
            }
        }


        private void OnValidate()
        {
            if(orbiter == null)
            {
                orbiter = GetComponent<TransformOrbiter>();
                if(orbiter == null)
                    orbiter = gameObject.AddComponent<TransformOrbiter>();
            }

            orbiter.enabled = enabled;
        }
    }
}