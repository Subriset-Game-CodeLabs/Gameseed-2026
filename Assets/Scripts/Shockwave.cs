using System;
using Input;
using UnityEngine;

public class Shockwave : MonoBehaviour
{

    [SerializeField] private float _shockwaveRadius = 5f;
    [SerializeField] private float _maxPower = 5f;
    [SerializeField] private float _coneAngle = 45f;
    [SerializeField] private Transform _targetIndicator;

    public event Action OnSmash;

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(_targetIndicator.position - transform.position);
    }

    public void Explode(float powerModifier, string targetTag)
    {
        OnSmash?.Invoke();
        Debug.Log(transform.position);
        Vector3 explosionPosition = transform.position;
        Vector3 coneDirection = transform.forward;

        Collider[] colliders = Physics.OverlapSphere(explosionPosition, _shockwaveRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (rb.transform.tag != targetTag)
                    continue;
                
                Vector3 directionToTarget = (hit.transform.position - explosionPosition).normalized;
                float angle = Vector3.Angle(coneDirection, directionToTarget);

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Debug.Log(angle);
                Debug.Log(_coneAngle);
                
                // if (angle <= _coneAngle)
                // {
                Debug.Log(powerModifier * _maxPower);
                rb.AddExplosionForce(powerModifier * _maxPower, explosionPosition, _shockwaveRadius, 2.0f, ForceMode.Impulse);
                // }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;

        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(pos, _shockwaveRadius);

        Vector3 leftBoundary =
            Quaternion.AngleAxis(-_coneAngle, Vector3.up) *
            transform.forward;

        Vector3 rightBoundary =
            Quaternion.AngleAxis(_coneAngle, Vector3.up) *
            transform.forward;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(pos, pos + transform.forward * _shockwaveRadius);
        Gizmos.DrawLine(pos, pos + leftBoundary * _shockwaveRadius);
        Gizmos.DrawLine(pos, pos + rightBoundary * _shockwaveRadius);
    }
}