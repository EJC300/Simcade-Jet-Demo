using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OriginShifter : MonoBehaviour
{

    // Update is called once per frame
    void LateUpdate()
    {

        if (Camera.main.transform.position.magnitude > GameWorld.instance.CenterSclae)
        {
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
                g.transform.position -= Camera.main.transform.position;

            

        }

        //Add Particles line and trail renderes on polish steps

    
    }
}
