// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Oculus.Interaction.ComprehensiveSample
{
    public class PhysicsNoise : MonoBehaviour
    {
        [SerializeField]
        float noiseFrequency = 0.5f;
        [SerializeField]
        float noisePositionStrength = 0.1f;
        [SerializeField]
        float noiseRotationStrength = 0.1f;

        [SerializeField]
        bool timeBased;

        private float random = 0;

        Rigidbody m_Rigidbody;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            random = Random.Range(0, 1000f);
        }

        public void FixedUpdate()
        {
            Vector3 pos, ang;
            if (!timeBased)
            {
                pos = PerlinNoise3D(transform.position, noiseFrequency, noisePositionStrength);
                ang = PerlinNoise3D(transform.position, noiseFrequency, noiseRotationStrength);
            }
            else
            {
                pos = PerlinNoise3D(Time.unscaledTime + random, noiseFrequency, noisePositionStrength);
                ang = PerlinNoise3D(Time.unscaledTime + random, noiseFrequency, noiseRotationStrength);
            }
            m_Rigidbody.AddForce(pos);
            m_Rigidbody.AddTorque(ang);
        }

        Vector3 PerlinNoise3D(Vector3 pos, float frequency, float amplitude)
        {
            pos *= frequency;
            float px = Mathf.PerlinNoise(pos.y, pos.z);
            float py = Mathf.PerlinNoise(pos.x, pos.z);
            float pz = Mathf.PerlinNoise(pos.x, pos.y);
            return new Vector3(px, py, pz) * amplitude;
        }

        Vector3 PerlinNoise3D(float time, float frequency, float amplitude)
        {
            time *= frequency;
            float px = Mathf.PerlinNoise(time, 0) * 2 - 1;
            float py = Mathf.PerlinNoise(time, 123) * 2 - 1;
            float pz = Mathf.PerlinNoise(time, 654) * 2 - 1;
            return new Vector3(px, py, pz) * amplitude;
        }

    }
}
