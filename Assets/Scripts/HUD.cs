using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore;
public class HUD : MonoBehaviour
{
   
    public JetFighter player;
    public Text Throttle;
    public Text G;
    public Text Speed;
    public Text AOA;
    public Transform VelocityVector;
    void Start()
    {
        if(player == null)
        {
            player = GetComponent<JetFighter>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }
        
            G.text = $"{player.Gforce:0.0:}G";
            Throttle.text = $"THR:{player.indicatedThrottle * 100:0.0}%";

        if ((player.speed * MathStuff.ConvertToKPH() > 1200))
        {
            Speed.text = $"SPD:{(player.speed * MathStuff.ConvertToKPH()) * 0.000809848f:0.0}Mach";
        }
        else
        {
            Speed.text = $"SPD:{player.speed * MathStuff.ConvertToKPH():0.0}KMH";
        }
            AOA.text = $"{player.aoa:0.0f}Pitch";
            var velocityPos = Camera.main.WorldToScreenPoint(player.transform.position + player.velocity * 3500f);
           
           velocityPos.z = Mathf.Clamp(velocityPos.z, -200f, 200f);


        VelocityVector.position = SmoothDamp.Move(VelocityVector.position, velocityPos,5, Time.deltaTime);
        
    }
}
