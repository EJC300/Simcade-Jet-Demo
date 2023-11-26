using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class MathStuff
{
    public static float ConvertToKPH()
    {
        return 3.6f;
    }

    public static float ConvertToKilometers()
    {
        return 0.001f;
    }

    public static float ScaleFactor(float scale)
    {
        return scale;
    }

    public static float ToKnots()
    {
        return 0.539957f;
    }

    public static float DivideBy1000(float divisor)
    {
        return divisor / 10000;
    }
    public static float MultiplyByScalar(float scalar, float value)
    {
        return scalar * value;
    }
    public static float CalculateGEffect(float pitch, Vector3 velocity, Transform trn)
    {
        Vector3 localVelo = trn.InverseTransformDirection(velocity);

        

        if (Mathf.Abs(pitch * Mathf.Deg2Rad) < Mathf.Epsilon)
        {
            return trn.up.y;
        }
        float radius = localVelo.z /  ((pitch));
        float verticalForce = (localVelo.z * localVelo.z) / radius;

        float gForce = -verticalForce / 9.8f;
        gForce += trn.up.magnitude;
        return gForce;
    }

    public static float CalculateThrottlePercentage(float throttleAmount, float MaxThrust)
    {
        return Mathf.RoundToInt((MaxThrust / throttleAmount));
    }

    public static float Remap(float a, float b, float dt)
    {
        return Mathf.Lerp(a, b, dt);
    }
    public static Vector3 Remap(Vector3 a, Vector3 b, float dt)
    {
        float x = Remap(a.x,b.x,dt);
        float y = Remap(a.y, b.y, dt); 
        float z = Remap(a.z, b.z, dt);

        return new Vector3(x, y, z);
      
    }
}
