using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtThat : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        transform.LookAt(target);
    }
}
