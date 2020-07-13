using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{

    public class CinematicTrigger : MonoBehaviour
    {

        //Cached
        //Need to be change to false, it is true only for test mode
        bool allreadyTriggerd = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!allreadyTriggerd && other.tag == "Player")
            { 
                    GetComponent<PlayableDirector>().Play();
                    allreadyTriggerd = true;   
            }
            
       
        }

    }
}
