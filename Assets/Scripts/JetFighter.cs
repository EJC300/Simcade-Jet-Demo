using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;


/*
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
        if(extended)
        {
            totalDrag = Mathf.MoveTowards(totalDrag, extendedDragFull, extendTime);
        }
        else if(!extended)
        {
            totalDrag = Mathf.MoveTowards(totalDrag, 0, extendTime);
        }
        isExtended = !Mathf.Approximately(0, totalDrag);
    }
}
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
            float wheelUp = Vector3.Dot(totalForce * trn.up, landingGear.up );
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
       if(isGrounded)
       {
            var wheelHitNormalDirection = (landingGear.position - wheelHit.normal).normalized;
            var angleBetweenNormal = Vector3.Angle(wheelHitNormalDirection, landingGear.position);
            Quaternion moveTo = Quaternion.AngleAxis(angleBetweenNormal,-Vector3.right);
            return moveTo;
       }
        return Quaternion.identity;
    }
}

public class JetFighter : MonoBehaviour
{

    /*
     * This demo does jet fighter behavior without the physics engine or real physics. 
     * The goals of this demo:
     * Pitch - smooth pitch that adjusts based on speed
     * Roll - smooth roll
     * Turn Rate - Glimit : G forces limit pitch rate using some kind of max glimited turn
     * Thrust & Gravity : Thrust controlled by throttle with gravity pulling the jet down
     * Drag : basic drag slows down the plane
     * Angle of Attack : Angle of attack will increase an inducedDrag 
     * G Limited Turns that limit pitch rate based on g-Force
     * Stall : When it hits below stall speed the aircraft points its nose down unless its speed surpasses stall speed
     * HUD : Seperate HUD class that tracks pitch rate,throttle,velocity vector, g force and speed
     


    
     * Checklist:
     Aircraft-
     1 No Physics Engine - done
     2 Throttle - done
     3 Engine Thrust plus AfterBurner - done
     4 Linear Drag(Forward Drag) - done
     5 Linear Forces - done
     6 Gravity System - done
     7 Drag based Extensions ( non physical) like flaps,landing gear and speedbrakes - done needs work
     8 Rotations with pitch roll and yaw - done -mostly
     9 Angle of Attack using this api call (Vector3.Angle(transform.forward,velocity))-done
     10 AOA effects inducedDrag - done
     11 when stall condition is met looktowards velocity which will be gravity - done
     12 GLimiter - done it works sorta
     13 Landing Gear ground movement - oh dear god - done mostly, and works better than I thought!
     14 HUD - work on this before landing gear - done
     15 Refactor- Put aircraft stats in a seperate class , use getter and setters plus properties
     Simulation/World-
     1 Origin Shift to deal with floating point errors
     2 Use a terrain generated from height maps using a procedural mesh of course make spot for a runway in the heightmap
     3 Setup a simple start & exit functionality complete with a howto use guide.
     Bonus-
     1 Polish the demo by adding an external chase camera
     2 Add some afterburner effect, turning wing tip trail effect
     3 "Crash" effect by bouncing off the terrain and objects
     4 Add some jet noises
     Fixes-
     Fix the dang aerial drag and induced drag effects
     Add Yaw
     *

    //When it is done I am going to refactor all this
    public GForceLookUpTable limits;
    public ExtendableDragPart flaps;
    public ExtendableDragPart speedBrakes;
    public float targetPitch;
    public float targetYaw;
    public float targetRoll;   
    //public ExtendableDragPart landingGear
    public float throttle,indicatedThrottle,throttleInput,ThrottleSpeed;
    public float thrust = 570000;
    public float afterBurner = 1800000;
    public float dragCoef = 0.7f;
    public float inducedDragCoef = 0.45f;
    public float lowSpeed;
    public float maxG = 7f;
    public float minG = 3f;
    public float highGTurnRate = 0.5f;
    private Vector3 dragForce, inducedDragForce;
    private float glimit = 1;
    public float mass = 860000;
    public float accelerationResponse;
    
    private Vector3 acceleration;
    public float aoa;
   
    private float drag;
    private Vector3 forwardForce;
    public Vector3 velocity;
    public Vector3 targetVelocity;
    public Vector3 velocityDireciton;
    private bool gearFlaps = false;
    private bool brakes = false;
    public float speed;
    public float stallSpeed;
    private float g;
    public float Gforce;
    private float pitch;
    public float pitchSpeed;
    private float roll;
    public float rollSpeed;
    private float yaw;
    private int index = 0;
    public float   yawSpeed;
    private bool isGrounded;
    public List<LandingGear> gears = new List<LandingGear>();
    private void ThrottleInput()
    {
        if(PlayerInput.RawThrottle() > 0)
        {
            throttleInput += 0.75f * Time.deltaTime;
        }
        else if(PlayerInput.RawThrottle()<0)
        {
            throttleInput -= 0.75f * Time.deltaTime;
        }
        throttleInput = Mathf.Clamp(throttleInput, 0.0f, 2.0f);
        if (throttleInput > 0.0)
        {
            throttle = Mathf.MoveTowards(throttle,  thrust * throttleInput, ThrottleSpeed );
        }
       else if(throttleInput > 1.0 && throttleInput < 2.0)
        {
            throttle = Mathf.MoveTowards(throttle,  afterBurner * throttleInput, ThrottleSpeed * 20 );
        }
        else
        {
            throttle = Mathf.MoveTowards(throttle, 0, ThrottleSpeed);
        }
        indicatedThrottle = throttleInput;
    }

    private float TargetPitch()
    {
        return PlayerInput.RawPitch() * targetPitch;
    }
    private float TargetRoll()
    {
        return PlayerInput.RawRoll() * targetRoll;
    }
    private void Extendables()
    {
        
        if (!gearFlaps && PlayerInput.FlapsAndGearToggle())
        {

            gearFlaps = true;
        }
       else if (gearFlaps && PlayerInput.FlapsAndGearToggle())
        {
            gearFlaps = false;
        }
        flaps.UpdatePart(gearFlaps);
        foreach (LandingGear g in gears)
        {
            g.UpdatePart(gearFlaps);
        }
        if (!brakes && PlayerInput.SpeedBreaksToggle())
        {

            brakes = true;
        }
        else if (brakes && PlayerInput.SpeedBreaksToggle())
        {
            brakes = false;
        }
       speedBrakes.UpdatePart(brakes);
    }
   
    private float CalculateAOA(Vector3 currentVelocity)
    {
        return Vector3.Angle(transform.forward, currentVelocity);

    }
    private Vector3 Gravity()
    {
        return GameWorld.instance.Gravity * Vector3.down * mass;
    }
    private void CalculateEngineThrust()
    {
       
        forwardForce = (transform.forward * throttle);
    }
    private void RotationFlight()
    {
        g = MathStuff.CalculateGEffect(pitch, velocity, transform);
        
        Gforce = SmoothDamp.Move(Gforce, g, 2f, Time.deltaTime);


        // float glerp = g > 0 ? Mathf.InverseLerp(maxG, maxG + maxG * 0.1f,g) : Mathf.InverseLerp(-minG, -minG - minG * 0.1f, g);
        // float glimit = Mathf.Lerp(0f, 1f, 1f - glerp);
       
        float gF = Mathf.Abs(Gforce);

        //TODO use a g-force lookup table- Example g == abs > 2 abs < 3 glimit = 0.5,glimit = 0.3 abs > 3 abs < 4 glimit = 0.2 abs > 6 < 0.05
        //Calculate - Done 
      

       
       
            if(gF > 2 && gF < 3)
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
            else if(gF < 2)
            {
                index = 0;
            }
            
        

       
         glimit = limits.limit[index];




      
            pitch = SmoothDamp.Move(pitch, TargetPitch() * glimit, pitchSpeed, Time.deltaTime);
            roll =  SmoothDamp.Move(roll, TargetRoll(), rollSpeed, Time.deltaTime * speed * 0.5f);
        

        Quaternion gearRotation = LandingGearRot();
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);
        Quaternion rollRotation = Quaternion.AngleAxis(roll, Vector3.forward);
        if (speed < lowSpeed && !isGrounded)
        {
            Quaternion stallRotation = Quaternion.AngleAxis(stallSpeed, Vector3.right);
            transform.localRotation = stallRotation * transform.localRotation;
        }
        else
        {
            transform.localRotation *= pitchRotation  * rollRotation * gearRotation;
        }
    }

    /// <summary>
    ///Landing Gear logic overview 
    /// The landing gear is going to be interesting it is going to be a raycast physics thing that creates a limited spring effect on the acceleration vector
    /// It in theory would be highly unstable so it will need a maximum height that is clamped between maximum and mininum height
    /// The suspension system is much more simple than my raycast rigidbody experiments.
    /// 
    ///
    /// </summary>

    private void CalculateDragForce()
    {
        //TODO InducedDrag based on AOA Needs to be improved should instead of reducing extended part drag we need to go to previous drag before  extended part
        float velocitySquared = (speed * speed);
        float flapsDrag =   flaps.totalDrag;//????
        float brakesDrag =  speedBrakes.totalDrag;
        //Needs fixing should have a total drag local variable that increments 
        float totalDrag = 0;
        drag = velocitySquared * dragCoef;
        if (speedBrakes.isExtended)
        {
            totalDrag += drag * brakesDrag;
           
        }
        else
        {

            totalDrag += drag;

        }
        if (flaps.isExtended)
        {

            totalDrag += drag * flapsDrag;
        }
        else
        {
            totalDrag += drag;

        }
        Debug.Log(drag);

        float inducedDrag = 0.5f * drag * inducedDragCoef * aoa ;
        dragForce = -transform.forward  * totalDrag;
        inducedDragForce = -transform.forward * inducedDrag;
        dragForce = dragForce + inducedDragForce;
        
    }

    private Vector3 LandingGearForce()
    {
        foreach(LandingGear g in gears)
        {
            isGrounded = g.isGrounded;
            return g.WheelForce(acceleration, transform, 1.0f, 0.5f);
        }
        return Vector3.zero;
    }
    private Quaternion LandingGearRot()
    {
        foreach (LandingGear g in gears)
        {
            return  g.LookToWheelRotation();
        }
        return Quaternion.identity;
    }
    private void CalculateAcceleration()
    {
        var gravityForce = Gravity();

        //Landing gears deployed add their respective force to it of course when grounded
        var gearForce = LandingGearForce();
        acceleration = (forwardForce + dragForce + gearForce + gravityForce) / mass;

        speed += Vector3.Dot(transform.forward, acceleration) * Time.deltaTime;

        //TODO Of course we have a way for the velocity to always point down when high angle of attack happens - done!
        var targetForwardVelocity = speed * transform.forward;
        velocityDireciton = speed * transform.forward;
        //Calculate AOA for targetVelocity
        aoa = CalculateAOA(velocity);
        //Do something that works essentialy lerp aoa to stall speed based on speed I have no idea why it works.
        var stallAOA = MathStuff.Remap(aoa, stallSpeed, speed);
        targetVelocity = Vector3.RotateTowards(targetForwardVelocity, Vector3.down, stallAOA * Mathf.Deg2Rad, 0);

        velocity = SmoothDamp.Move(velocity, targetVelocity, accelerationResponse, Time.deltaTime);
       
        transform.position += velocity * GameWorld.instance.WorldScale * Time.deltaTime;
        
    }
    private void LinearFlight()
    {
        CalculateEngineThrust();
       
        CalculateAcceleration();
        CalculateDragForce();
    }
    void PlayerControl()
    {
        ThrottleInput();
        Extendables();
    }
    private void Start()
    {
        limits = new GForceLookUpTable();
    }
    private void Update()
    {
        PlayerControl();
     
    }
    private void FixedUpdate()
    {
        LinearFlight();
        RotationFlight();
        
    }
}
  */
