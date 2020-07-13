using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

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

            yield return fader.FadeOut(fadeOutTime);

            wrapper.Save();
            print("File saved.");

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            print("Level Loaded");

            wrapper.Load();
            print("File Loaded");

            Portal otherPortal = GetOtherPortal();   
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

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
