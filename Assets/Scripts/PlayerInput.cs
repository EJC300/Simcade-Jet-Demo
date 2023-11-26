using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInput
{
  /*
   * Another shitty static accessor class accept this one controls player input using the input axis from the old input system
   * 
   * 
   */
    public static bool FlapsAndGearToggle()
    {
        return Input.GetKeyDown(KeyCode.G);
    }
    public static bool SpeedBreaksToggle()
    {
        return Input.GetKeyDown(KeyCode.B);
    }
    public static float RawThrottle()
    {
        return Input.GetAxis("Throttle");
    }

    public static float RawPitch()
    {
        return Input.GetAxis("Vertical");
    }
    public static float RawRoll()
    {
        return Input.GetAxis("Horizontal");
    }
   /* public static float RawYaw()
    {
        return Input.GetAxis("Vertical");
    }
   */
}
