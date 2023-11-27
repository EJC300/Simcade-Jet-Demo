using Aircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private JetController jet;
    // Start is called before the first frame update
    void Start()
    {
        if(jet == null)
        {
            jet = GetComponent<JetController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        jet.AdjustThrottle(PlayerInput.RawThrottle());
        jet.ControlJetOrientation(PlayerInput.RawPitch(),PlayerInput.RawYaw(),PlayerInput.RawRoll());
        jet.Extendables(PlayerInput.FlapsAndGearToggle(), PlayerInput.FlapsAndGearToggle(), PlayerInput.SpeedBreaksToggle());
    }
}
