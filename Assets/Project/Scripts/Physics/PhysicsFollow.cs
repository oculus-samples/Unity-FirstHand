// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    /// <summary>
    /// Updates physics objects position to follow the target
    /// </summary>
    public class PhysicsFollow : MonoBehaviour
    {
        public Transform target;

        private Rigidbody m_Rigidbody;

        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        void SyncToTarget()
        {
            m_Rigidbody.position = transform.position = target.position;
            m_Rigidbody.rotation = transform.rotation = target.rotation;
        }

        public void FixedUpdate()
        {
            UpdateVelocities();
        }

        public virtual void UpdateVelocities()
        {
            SetRigidbodyVelocitiesForTarget(target.position, target.rotation);
        }

        void SetRigidbodyVelocitiesForTarget(Vector3 targetPosition, Quaternion targetRotation)
        {
            Rigidbody rb = m_Rigidbody;
            float deltaTime = Time.deltaTime;

            float velocityMagic = 6000f / (deltaTime / 0.0111f);
            float angularVelocityMagic = 50f / (deltaTime / 0.0111f);

            Quaternion rotationDelta;
            Vector3 positionDelta;

            float angle;
            Vector3 axis;

            rotationDelta = targetRotation * Quaternion.Inverse(rb.transform.rotation);
            positionDelta = (targetPosition - rb.transform.position);

            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
                angle -= 360;

            if (angle != 0)
            {
                Vector3 angularTarget = angle * axis;
                if (float.IsNaN(angularTarget.x) == false && deltaTime > 0)
                {
                    angularTarget = (angularTarget * angularVelocityMagic) * deltaTime;
                    rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, 200f);
                }
            }

            Vector3 velocityTarget = (positionDelta * velocityMagic) * deltaTime;
            if (float.IsNaN(velocityTarget.x) == false)
            {
                rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, 100f);
            }
        }
    }
}
