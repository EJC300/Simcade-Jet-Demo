using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfintePlane : MonoBehaviour
{
    [SerializeField] private GameObject groundPlane;

    [SerializeField] private float spawnDistance;

    [SerializeField] private float offset;

    [SerializeField] private Transform player;

    private void LateUpdate()
    {
        if(player.transform.position.magnitude > spawnDistance)
        {
            Quaternion playerRotation = player.transform.rotation;
            playerRotation.x = 0;
            playerRotation.z = 0;
            groundPlane.transform.position += player.position;
            groundPlane.transform.rotation = playerRotation;
        }
        
    }
}
