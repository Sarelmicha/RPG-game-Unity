using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagment
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            HEAD_PORTAL,
            TAIL_PORTAL
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier portalType;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;



        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());

            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set");
                yield break;
            }
            // Don't Destory the game object until Coroutine is done execute all
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            //Remove control of the player


            yield return fader.FadeOut(fadeOutTime);

            //First save is for saving the player data (weapon etc)
            wrapper.Save();
            print("File saved for the first time in portal.");


            //Load the new level
           yield return SceneManager.LoadSceneAsync(sceneToLoad);

            print("Level Loaded");
            print("after load index is " + SceneManager.GetActiveScene().buildIndex);

            //Remove control of the player
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;


            wrapper.Load();
            print("File Loaded");

            Portal otherPortal = GetOtherPortal();   
            UpdatePlayer(otherPortal);

            //Second save is to save the current level with new player position
            wrapper.Save();
            print("File saved for the second time in portal.");

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            print("after fade in...");

            // Restore control
            newPlayerController.enabled = true;

            //Now destory the game object
            Destroy(gameObject);

        }

        Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal != this)
                {
                    if (portalType == DestinationIdentifier.HEAD_PORTAL &&
                        portal.portalType == DestinationIdentifier.TAIL_PORTAL ||
                        portalType == DestinationIdentifier.TAIL_PORTAL &&
                        portal.portalType == DestinationIdentifier.HEAD_PORTAL)
                    {
                        print("here man1!");
                        return portal;
                    }
                }
            }

            print("here man2!");
            return null;

        }

        void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;

        }
    }
}
