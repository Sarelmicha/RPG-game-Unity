using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        // Config
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspictionTime = 3f;

        // Cached
        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        // State 
        Vector3 guardPositon;
        float timeSinceLastSawPlayer = Mathf.Infinity;
     

        private void Start()
        {

            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            mover = GetComponent<Mover>();

            guardPositon = transform.position;
        }

        private void Update()
        {
            if (health.IsDead())
            {
                return;
            }

            GameObject player = GameObject.FindWithTag("Player");
            if (InAttackRange() && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour(player);
            }
            else if (timeSinceLastSawPlayer < suspictionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                GuardBehaviour();
            }

            timeSinceLastSawPlayer += Time.deltaTime; ;
        }

        private void GuardBehaviour()
        {
            fighter.Cancel();
            mover.StartMoveAction(guardPositon);
        }

        private void SuspicionBehaviour()
        {
            transform.LookAt(player.transform);
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour(GameObject player)
        {
            fighter.Attack(player);
        }

        private bool InAttackRange()
        {
       
            return Vector3.Distance(player.transform.position, transform.position)
                <= chaseDistance;
        }

        // Called by Unity 
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
