using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {

        [SerializeField] Weapon weapon = null;

        private void OnTriggerEnter(Collider other)
        {
            print("Triggerd bitch");
            if (other.gameObject.tag == "Player")
            {
           
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}
