using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    [SerializeField] private ParticleSystem rain;

    public Transform target;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rainOffset;

    private void Start()
    {
        rainOffset = rain.transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, damping);

        rain.transform.position = transform.position + rainOffset;
    }
}
