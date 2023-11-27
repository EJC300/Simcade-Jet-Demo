using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore;
using Aircraft;
namespace Player
{
    public class HUD : MonoBehaviour
    {

        [SerializeField] private JetController jet;
        [SerializeField] private Text Throttle;
        [SerializeField] private Text G;
        [SerializeField] private Text Speed;
        [SerializeField] private Text AOA;
        [SerializeField] private Transform VelocityVector;
    

       
        void Update()
        {
            if (jet == null)
            {
                return;
            }

            G.text = $"{jet.GForce:0.0:}G";
            Throttle.text = $"THR:{jet.indicatedThrottle()*100:0.0}%";

            if ((jet.Speed * MathStuff.ConvertToKPH() > 1200))
            {
                Speed.text = $"SPD:{(jet.Speed * MathStuff.ConvertToKPH()) * 0.000809848f:0.0}Mach";
            }
            else
            {
                Speed.text = $"SPD:{jet.Speed * MathStuff.ConvertToKPH():0.0}KMH";
            }
            AOA.text = $"{jet.AOA:0.0f}Pitch";
            var velocityPos = Camera.main.WorldToScreenPoint(jet.transform.position + jet.Velocity * 3500f);

            velocityPos.z = Mathf.Clamp(velocityPos.z, -200f, 200f);


            VelocityVector.position = SmoothDamp.Move(VelocityVector.position, velocityPos, 5, Time.deltaTime);

        }
    }
}