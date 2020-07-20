using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] float projectileSpeed = 1f;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect;
        [SerializeField] GameObject[] destoryOnHit = null;
        [SerializeField] UnityEvent onHit;


        Health target = null;
        GameObject instigator = null;
        float damage = 0;


        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)
            {
                return;
            }

            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }

            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);

        }

        public void SetTarget(Health target,GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

        

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null)
            {
                return target.transform.position;
            }
           
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider collider)
        {
            
            if (collider.GetComponent<Health>() != target)
            {
                return;
            }

            if (target.IsDead())
            {
                return;
            }

        
            target.TakeDamage(instigator, damage);

            projectileSpeed = 0;

            onHit.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestory in destoryOnHit)
            {
                Destroy(toDestory);
            }

            Destroy(gameObject,lifeAfterImpact);
        }
    }
}
