using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using UnityEngine.EventSystems;

using System;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float maxNavPathLength = 40f;

        private void Awake()
        {
            health = GetComponent<Health>();   
        }

        private void Update()
        {
            if (IntercatWithUI())
            {
                SetCursor(CursorType.UI);
                return;
            }

            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent())
            {  
                return;
            }
     
            if (InteractWithMovement())
            {
                SetCursor(CursorType.Movement);
                return;
            }


            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits =  Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }


        private bool IntercatWithUI()
        {
            //IsPointerOverGameObject returns true if the cursor on UI Object
            return EventSystem.current.IsPointerOverGameObject();
        }


        private bool InteractWithMovement()
        {

            //RaycastHit hit;
            //Check if the ray has hit somthing
            //bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target,1f);
                }
              
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            //Raycast to terrain
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (!hasHit)
            {
                return false;
            }

            //Find nearest navmesh point
           NavMeshHit navMeshHit;
           bool hasCastToNavMesh =  NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!hasCastToNavMesh)
            {
                return false;
            }

            target = navMeshHit.position;

            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath)
            {
                return false;
            }
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            if (GetPathLength(path) > maxNavPathLength)
            {
                return false;
            }

            return true;
        
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;

            if (path.corners.Length < 2)
            {
                return total;
            }

            for (int i = 0; i < path.corners.Length - 1 ; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping cursorMapping in cursorMappings)
            {
                if (cursorMapping.type == type)
                {
                    return cursorMapping;
                }
            }

            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {

            //Shoot a ray from the main camera 
            Ray lastRay =  Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(lastRay.origin, lastRay.direction * 100);
            return lastRay;
        }
    }
}

