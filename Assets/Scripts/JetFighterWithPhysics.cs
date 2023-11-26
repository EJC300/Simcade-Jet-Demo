using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class JetFighterWithPhysics : MonoBehaviour
{
    public Rigidbody rb;
    public float realThrottle, throttle, targetThrottle, indicatedThrottle;
    public float throttleSpeedDown, throttleSpeedUp;
    public float thrust;

    private Vector3 linearForce;
    private Vector3 angularForce;

    void AdjustThrottle()
    {

        if (PlayerInput.RawThrottle() > 0)
        {
            throttle += throttleSpeedUp;

        }
        else if (PlayerInput.RawThrottle() < 0)
        {
            throttle -= throttleSpeedDown;
        }
        throttle = Mathf.Clamp(throttle, 0, 100);
        targetThrottle = Mathf.MoveTowards(targetThrottle, throttle / 100, 1 * Time.deltaTime);
        realThrottle = targetThrottle * thrust;



    }




    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
