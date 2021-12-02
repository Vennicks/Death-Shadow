using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public Transform player;
    [SerializeField] public float smoothSpeed = 0.125f;
    [SerializeField] public Vector3 offset;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); ;
        transform.position = smoothedPosition;
    }
}
