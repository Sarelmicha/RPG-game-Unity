using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;

        // Update is called once per frame
        void LateUpdate()
        {
            var euler = target.rotation.eulerAngles;   //get target's rotation
            var rot = Quaternion.Euler(0, euler.y,0 ); //transpose values
            transform.rotation = rot;                  //set my rotation

        }
    }
}
