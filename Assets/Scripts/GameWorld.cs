using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    /*
     * This is a basic world settings that remain unchanged but are set the designer.
     * I use a hacky static accessor thing to get the variables. Eventually In my jet fighter project I would use JSON to edit the world settings.
     * The GameWorld for this project since I am not dealing with physics has a few variable settings
     * WorldScale = Fast or slow game objects traverse the level
     * Gravity = How strong the force down is.
     * Center Scale = how far away the player can be from the center until the the the world shifts back to the camera position
     */

    [Range(0,1f)]
    [SerializeField] private float worldScale = 1f;
    [Range(0,100)]
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float centerScale = 1f;

    //World Scale scales the speed of the objects NOT the indicated speed of the objects
    //In theory the a jet will or an ai plane will have two values the actual simulation Velocity and the indicated Velocity.
    //The indicated velocity displays the velocity untouched by World Scale. Since this is only a player operated demo I don't need to think about indicated Velocity on bots.
    public float WorldScale
    {
        get { return worldScale; }
    }
    public float Gravity
    {
        get { return gravity; }
    }
    public float CenterSclae
    {
        get { return centerScale; }
    }
    public static GameWorld instance;

    private void Start()
    {
        if (instance == null)
        {


            instance = this;
        }
    }

}
