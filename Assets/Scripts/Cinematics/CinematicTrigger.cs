using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{

    public class CinematicTrigger : MonoBehaviour
    {

        //Cached
        bool allreadyTriggerd = false;

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
