using System;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

#if UNITY_EDITOR
        //This code run when compiled and not in runtime (Only in editor)
        private void Update() {
            //Check if we are during run time
            if (Application.IsPlaying(gameObject)) return;
            //if the path is empty so we are in a prefab, return and do not do the logic
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            //Get the serilzition of the object of type SavableEntity
            SerializedObject serializedObject = new SerializedObject(this);
            //Get the property Unique Id from the serialized Object
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            //If the Uid is empty
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                //Set the value of the uid to a unique id
                property.stringValue = System.Guid.NewGuid().ToString();
                //Tell unity that a change has been made in this object
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }
#endif

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;

            if (globalLookup[candidate] == this) return true;

            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}