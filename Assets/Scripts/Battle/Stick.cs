using System;
using UnityEngine;

public class Stick : MonoBehaviour
{
    private Rigidbody rb;

    public bool IsFlying { get; private set; }
    public bool HasLanded { get; private set; }

    public GameObject landedOnObject;

    public event Action<GameObject> OnLanded;
    public event Action<GameObject> OnLandedOnEnemy;
    public event Action<GameObject> OnLandedOnPlayer;

    [SerializeField] private float velocityThreshold = 0.1f;
    [SerializeField] private float normalThreshold = 0.3f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartFlying()
    {
        IsFlying = true;
        HasLanded = false;
        landedOnObject = null;
    }

    public void AddExplosionForce(float force, Vector3 explosionPosition, float explosionRadius)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        rb.AddExplosionForce(force, explosionPosition, explosionRadius, 2.0f, ForceMode.Impulse);
        StartFlying();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!IsFlying || HasLanded)
            return;

        if (rb.linearVelocity.magnitude > velocityThreshold ||
            rb.angularVelocity.magnitude > velocityThreshold)
            return;

        foreach (ContactPoint contact in collision.contacts)
        {
            // Must be resting on the object
            if (Vector3.Dot(contact.normal, Vector3.up) <= normalThreshold)
                continue;

            HasLanded = true;
            IsFlying = false;

            landedOnObject = collision.gameObject;

            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Landed on enemy");
                OnLandedOnEnemy?.Invoke(landedOnObject);
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Landed on player");
                OnLandedOnPlayer?.Invoke(landedOnObject);
            }
            else
            {
                Debug.Log("Landed on object");
                OnLanded?.Invoke(landedOnObject);
            }

            break;
        }
    }


}
