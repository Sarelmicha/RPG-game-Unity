using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using UnityEngine.EventSystems;

using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        enum CursorType
        {
            None,
            Movement,
            Combat,
            UI
        }

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

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
                SetCursor(CursorType.Combat);
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
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IntercatWithUI()
        {
            //IsPointerOverGameObject returns true if the cursor on UI Object
            return EventSystem.current.IsPointerOverGameObject();
        }

       

      

        private bool InteractWithMovement()
        {
            
            RaycastHit hit;
            //Check if the ray has hit somthing
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point,1f);
                }
              
                return true;
            }
            return false;
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

