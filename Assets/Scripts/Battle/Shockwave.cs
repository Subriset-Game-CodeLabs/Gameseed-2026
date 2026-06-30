using System;
using Input;
using UnityEngine;

public class Shockwave : MonoBehaviour
{

    [SerializeField] private float _shockwaveRadius = 5f;
    [SerializeField] private float _coneAngle = 45f;
    [SerializeField] private Transform _targetIndicator;

    private Character _character;
    public event Action OnSmash;

    void Start()
    {
        
    }

    public void SetTargetIndicator(Transform transform)
    {
        _targetIndicator = transform;
        _character = _targetIndicator.GetComponent<Character>();
    }

    void Update()
    {

        transform.rotation = Quaternion.LookRotation(_targetIndicator.position - transform.position);
    }

    

    public void ShowHand()
    {
        gameObject.SetActive(true);
    }

    public void HideHand()
    {
        gameObject.SetActive(false);
    }

    public void Explode(float powerModifier, string targetTag)
    {
        float currentPower = _character.CharacterPower;
        OnSmash?.Invoke();
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
                
                // if (angle <= _coneAngle)
                // {
                Debug.Log(powerModifier * currentPower);
                rb.AddExplosionForce(powerModifier * currentPower, explosionPosition, _shockwaveRadius, 2.0f, ForceMode.Impulse);
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