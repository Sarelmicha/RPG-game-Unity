using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagment
{

    public class SavingWrapper : MonoBehaviour
    {

        const string DEFAULT_SAVE_FILE = "save";
        [SerializeField] float fadeInTime = 0.2f;

        private void Start()
        {
            print("Wake wake saving wrapper!!");

            int numOfWrappers = FindObjectsOfType<SavingWrapper>().Length;

            print("number of wrappers is" + numOfWrappers);

            print("my name is " + gameObject.name);
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {

            print("(Saving Wrapper)Inside Load Last Scene!!!");
            
            yield return GetComponent<SavingSystem>().LoadLastScene(DEFAULT_SAVE_FILE);

            print("(Saving Wrapper) after load last scene of SavingSystem!");
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
       }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(DEFAULT_SAVE_FILE);
        }
        public void Save()
        {
            GetComponent<SavingSystem>().Save(DEFAULT_SAVE_FILE);

        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(DEFAULT_SAVE_FILE);
        }
    }
}
