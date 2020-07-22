
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Utils;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        // Config
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspictionTime = 3f;      
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float aggroCooldownTime = 10f;
        [SerializeField] float shoutDistance = 5f;
       
        float waypointDwellTime = 3f;



        // Cached
        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        // State 
        LazyValue<Vector3> guardPositon;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;

        int currentWaypointIndex = 0;
       


        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            guardPositon = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            guardPositon.ForceInit();
        }


        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if (health.IsDead())
            {
                return;
            }

            GameObject player = GameObject.FindWithTag("Player");

            if (IsAggrevated() && fighter.CanAttack(player))
            {
           
                AttackBehaviour(player);
            }
            else if (timeSinceLastSawPlayer < suspictionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            //Set the timer
            timeSinceAggrevated = 0;
            
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPositon.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedWaypoint = 0;
                    RaffleWaypointDwellTime();
                    CyclicWaypoint();
                }

                nextPosition = GetCurrentWaypoint(); 
            }

            if (timeSinceArrivedWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition,patrolSpeedFraction);         
            }
        }

        private void RaffleWaypointDwellTime()
        {
            waypointDwellTime = UnityEngine.Random.Range(0, 1);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CyclicWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            transform.LookAt(player.transform);
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour(GameObject player)
        {
            AggrevateNearByEnemies();
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private void AggrevateNearByEnemies()
        {
            RaycastHit[] hits =  Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AIController enemy = hit.collider.GetComponent<AIController>();
                if (enemy != null)
                {
                    enemy.Aggrevate();
                }
            }
        }

        private bool IsAggrevated()
        {
            return Vector3.Distance(player.transform.position, transform.position)
                <= chaseDistance || timeSinceAggrevated < aggroCooldownTime ;
        }

        // Called by Unity 
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
