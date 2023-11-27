using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aircraft
{
    public class JetController : MonoBehaviour
    {
        [SerializeField] private Jet jet;

        private Jet Jet { get { return jet; } }

        #region AircraftControl
        private float throttle, throttleInput;
        public float indicatedThrottle() { return throttleInput; }

        private float pitch, roll, yaw;

        private bool flapsToggle, gearToggle, speedBrakeToggle, brakesToggle;
        public void AdjustThrottle(float input)
        {

            if (input > 0)
            {
                throttleInput += 0.75f * Time.deltaTime;
            }
            else if (input < 0)
            {
                throttleInput -= 0.65f * Time.deltaTime;
            }

            throttleInput = Mathf.Clamp(throttleInput, 0.0f, 2.0f);
            if (throttle < 1.0f)
            {
                throttle = SmoothDamp.Move(throttle, Jet.Thrust * throttleInput, Jet.ThrottleSpeed, Time.deltaTime);
               
            }
           else if (throttle > 1.0f)
            {
                throttle = SmoothDamp.Move(throttle, Jet.AfterBurner * throttleInput, Jet.ThrottleSpeed, Time.deltaTime);
            }

        }
        public void Extendables(bool flaps, bool gears, bool speedBrakes)
        {
            if (!flaps && flapsToggle)
            {
                flapsToggle = true;
            }
            else if (flaps && flapsToggle)
            {
                flapsToggle = false;
            }
            if (!gearToggle && gears)
            {
                gearToggle = true;
            }
           else if (gearToggle && gears)
            {
                gearToggle = false;
            }
            if (!speedBrakeToggle && speedBrakes)
            {
                speedBrakeToggle = true;
            }
           else if (speedBrakeToggle && speedBrakes)
            {
                speedBrakeToggle = false;
            }
            Jet.Flaps.UpdatePart(speedBrakeToggle);
            Jet.LandingGear.UpdatePart(gearToggle);
            Jet.SpeedBrakes.UpdatePart(speedBrakeToggle);
        }

        public void ControlJetOrientation(float x, float y, float z)
        {
            Gforce = SmoothDamp.Move(Gforce, MathStuff.CalculateGEffect(pitch, velocity, transform), 4, Time.deltaTime);
            var gF = Mathf.Abs(Gforce);
            if (gF > 2 && gF < 3)
            {
                index = 1;
            }
            else if (gF > 4 && gF < 5)
            {
                index = 2;
            }
            else if (gF > 5)
            {
                index = 3;
            }
            else if (gF < 2)
            {
                index = 0;
            }
            var glimit = Jet.Glimits.limit[index];
            var targetPitch = x * Jet.TargetPitch;
            var targetYaw = y * Jet.TargetYaw;
            var targetRoll = z * Jet.TargetRoll;

            pitch = SmoothDamp.Move(pitch, targetPitch * glimit, Jet.PitchSpeed, Time.deltaTime);
            yaw = SmoothDamp.Move(yaw, targetYaw, Jet.YawSpeed, Time.deltaTime);
            roll = SmoothDamp.Move(roll, targetRoll, Jet.RollSpeed, Time.deltaTime);
        }

        #endregion

        #region ForwardFlight 
        private float aoa;
        private bool isGrounded;
        private float speed;
        
        public float Speed { get { return speed; } }
        public float AOA { get { return aoa; } }

        private Vector3 acceleration, targetVelocity, velocity;
        
        private Vector3 inducedDragForce, dragForce, totalDragForce;
        private float Gforce;
        private int index;
        public float GForce { get { return Gforce; } }
        public Vector3 Velocity { get { return velocity; } }
        public Vector3 VelocityDirection { get { return velocity; } }


        private void AircraftDrag()
        {
            aoa = Vector3.Angle(transform.forward, velocity);
            var totalDrag = 0.0f;
            var drag = (speed * speed) * Jet.DragCoef;
            var inducedDrag = 0.5f * (speed * speed) * Jet.InducedDrag * aoa;
            dragForce = -transform.forward * drag;
            if (Jet.Flaps.isExtended)
            {
                totalDrag += Jet.Flaps.totalDrag * drag;
            }
            else
            {
                totalDrag = drag;
            }
            if (Jet.SpeedBrakes.isExtended)
            {
                totalDrag += Jet.SpeedBrakes.totalDrag * drag;
            }
            else
            {
                totalDrag = drag;
            }
            if (Jet.LandingGear.isExtended)
            {
                totalDrag += Jet.LandingGear.totalDrag * drag;
            }
            else
            {
                totalDrag = drag;
            }
            inducedDragForce = -transform.forward * inducedDrag;
            dragForce = -transform.forward * totalDrag;
            totalDragForce = dragForce + inducedDragForce;
        }
        private void AircraftForce()
        {
            var gravityForce = Vector3.down * GameWorld.instance.Gravity * jet.Mass;
            var thrust = transform.forward * throttle;
            var stallAOA = MathStuff.Remap(aoa, jet.MinStallSpeed, speed);
            speed += Vector3.Dot(transform.forward, acceleration) * Time.deltaTime;
            acceleration = (thrust + totalDragForce + gravityForce)/jet.Mass;
            targetVelocity = Vector3.RotateTowards(acceleration, Vector3.down, stallAOA * Mathf.Deg2Rad, 0.0f);

            if (isGrounded)
            {
                targetVelocity = Jet.LandingGear.LookToWheelRotation() * targetVelocity;

            }
      

            var velocityDirection = transform.forward * speed;
            velocity = SmoothDamp.Move(velocityDirection, targetVelocity, Jet.MovementResponse, Time.deltaTime);
          
            transform.position += velocity * GameWorld.instance.WorldScale * Time.deltaTime;
        }
        #endregion

        #region RotationFlight 



        private void FlightRotation()
        {
           

       




         
            var pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);
            var yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);
            var rollRotation = Quaternion.AngleAxis(roll, Vector3.forward);
            var stalling = (speed < Jet.MinStallSpeed && !isGrounded);
            if (stalling)
            {
                var stallRotation = Quaternion.AngleAxis(-0.5f,Vector3.right);
                transform.localRotation = stallRotation * transform.localRotation;
            }
            else
            {
                transform.localRotation *= pitchRotation * yawRotation * rollRotation;
            }
            if (isGrounded)
            {
                transform.localRotation *= Jet.LandingGear.LookToWheelRotation();
            }
        }

        #endregion

        #region UpdateState
        private void UpdateState()
        {
    
            AircraftDrag();
            AircraftForce();
            FlightRotation();



        }

        #endregion

        #region FixedUpdate
        private void FixedUpdate()
        {
            UpdateState();
        }

        #endregion


    }
}
