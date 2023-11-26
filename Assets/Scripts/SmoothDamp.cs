using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SmoothDamp 
{
    public static float Move(float start,float end,float speed,float dt)
    {
       
        return Mathf.Lerp(start, end, 1 - Mathf.Exp(-speed * dt));
    }
    public static Vector3 Move(Vector3 start,Vector3 end,float speed,float dt)
    {
        return Vector3.Lerp(start, end, 1 - Mathf.Exp(-speed * dt));
    }

    public static Quaternion Rotate(Quaternion start, Quaternion end, float speed, float dt)
    {
        return Quaternion.Slerp(start, end, 1 - Mathf.Exp(-speed * dt));
    }
}
