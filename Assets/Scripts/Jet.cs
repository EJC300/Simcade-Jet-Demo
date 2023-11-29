using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aircraft
{
    [Serializable]
    public class LandingGear : ExtendableDragPart
    {

        public float isGroundedDrag = 1.3f;
        public float stiffness;
        public float damper;
        public Transform landingGear;
        public bool isGrounded;
        private RaycastHit wheelHit;
        public Vector3 WheelForce(Vector3 acceleration, Transform trn, float maximumHeight, float minumHeight)
        {


            if (Physics.Raycast(landingGear.position, -landingGear.up, out wheelHit, maximumHeight + minumHeight) && isExtended)
            {
                var currentAccel = acceleration;
                isGrounded = true;
                float offset = (wheelHit.distance / (maximumHeight + minumHeight)) * stiffness;
                var currentForce = (landingGear.position - wheelHit.point);
                var damp = damper * -(currentForce.y);
                var totalForce = damp + offset;
                var drag = Vector3.Dot(landingGear.position, currentAccel) * landingGear.forward;
                float wheelUp = Vector3.Dot(totalForce * trn.up, landingGear.up);
                currentAccel.y += wheelUp;
                return currentForce - drag;
            }
            else
            {
                isGrounded = false;
            }

            return Vector3.zero;
        }
        public Quaternion LookToWheelRotation()
        {
            if (isGrounded)
            {
                var wheelHitNormalDirection = (landingGear.position - wheelHit.normal).normalized;
                var angleBetweenNormal = Vector3.Angle(wheelHitNormalDirection, landingGear.position);
                Quaternion moveTo = Quaternion.AngleAxis(angleBetweenNormal, -Vector3.right);
                return moveTo;
            }
            return Quaternion.identity;
        }
    }
    public class GForceLookUpTable
    {
        public float[] limit =
        {
        1.0f,
        0.5f, //G-force > 2 < 3
        0.35f,//G-force > 3 < 4
        0.02f, //G-force > 3.5 < 5
       
       
        };
    }
    [Serializable]
    public class ExtendableDragPart
    {
        //How long it takes the part to extend
        public float extendTime;
        //the full drag coef for the fully extended part
        public float extendedDragFull;
        //total incremental drag exerted
        public float totalDrag;

        public bool isExtended;


        public void UpdatePart(bool extended)
        {
            if (extended)
            {
                totalDrag = Mathf.MoveTowards(totalDrag, extendedDragFull, extendTime);
            }
            else if (!extended)
            {
                totalDrag = Mathf.MoveTowards(totalDrag, 0, extendTime);
            }
            isExtended = !Mathf.Approximately(0, totalDrag);
        }
    }

    [Serializable]
    public class Jet
    {

        /*
         * Checklist:
         Aircraft-
         1 No Physics Engine - done
         2 Throttle - done
         3 Engine Thrust plus AfterBurner - done
         4 Linear Drag(Forward Drag) - done
         5 Linear Forces - done
         6 Gravity System - done
         7 Drag based Extensions ( non physical) like flaps,landing gear and speedbrakes - done needs work-done
         8 Rotations with pitch roll and yaw - done -mostly
         9 Angle of Attack using this api call (Vector3.Angle(transform.forward,velocity))-done
         10 AOA effects inducedDrag - done
         11 when stall condition is met looktowards velocity which will be gravity - done
         12 GLimiter - done it works sorta
         13 Landing Gear ground movement - oh dear god - done mostly, and works better than I thought!-scrapped
         14 HUD - work on this before landing gear - done
         15 Refactor- Put aircraft stats in a seperate class , use getter and setters plus properties-done
         Simulation/World-
         1 Origin Shift to deal with floating point errors - done
         2 Generate simple hilly terrain to fly over - scrapped instead create infinite plane objects to fly over forever
         3 Setup a simple start & exit functionality complete with a howto use guide.
         Bonus-
         1 Polish the demo by adding an external chase camera
         2 Add some afterburner effect, turning wing tip trail effect
         3 "Crash" effect by bouncing off the terrain and objects - done
         4 Add some jet noises
         Fixes-
         Fix the dang aerial drag and induced drag effects
         Add Yaw
         */
        #region ExtendableDragPart
        [SerializeField] private ExtendableDragPart flaps;
        [SerializeField] private ExtendableDragPart speedBrakes;
        [SerializeField] private LandingGear landingGear;

        public LandingGear LandingGear { get { return landingGear; } }
        public ExtendableDragPart Flaps { get { return flaps; } }
        public ExtendableDragPart SpeedBrakes { get { return speedBrakes; } }



        #endregion

        #region Pitch,Yaw,Roll 
        [SerializeField] private float targetPitch, targetYaw, targetRoll;
        [SerializeField] private float pitchSpeed, yawSpeed, rollSpeed;
        public float TargetPitch { get { return targetPitch; } }
        public float TargetYaw { get { return targetYaw; } }
        public float TargetRoll { get { return targetRoll; } }

        public float PitchSpeed { get { return pitchSpeed; } }
        public float YawSpeed { get { return yawSpeed; } }
        public float RollSpeed { get { return rollSpeed; } }
        #endregion

        #region Thrust,Drag,InducedDrag etc variables
        [SerializeField] private float throttleSpeed,thrust, afterBurner, dragCoef, inducedDragCoef;
        [SerializeField] private float mass;
        [SerializeField] private float movementResponse;
        [SerializeField] private float minStallSpeed;
        private GForceLookUpTable gLimits = new GForceLookUpTable();

        public GForceLookUpTable Glimits { get { return gLimits; } }    
        public float ThrottleSpeed { get { return throttleSpeed; } }
        public float Thrust { get { return thrust; } }
        public float DragCoef { get { return dragCoef; } }
        public float InducedDrag { get { return inducedDragCoef; } }

        public float AfterBurner { get { return afterBurner; } }
        public float MovementResponse { get { return movementResponse; } }
        public float MinStallSpeed { get { return minStallSpeed; } }

        public float Mass { get { return mass; } }  
        #endregion

    }

}