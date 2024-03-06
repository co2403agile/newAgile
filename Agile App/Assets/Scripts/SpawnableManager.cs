using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/* SpawnableManager: Handles spawning and moving objects on touch input in AR environment */
/* Written by James */
public class SpawnableManager : MonoBehaviour
{
    [SerializeField] ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    [SerializeField] GameObject m_SpawnablePrefab; // The prefab to be spawned

    Camera m_ArCam; // Reference to the AR Camera
    GameObject m_SpawnedObject; // The currently spawned object

    /* Start: called before the first frame update */
    void Start()
    {
        m_SpawnedObject = null;
        m_ArCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    /* Update: called once per frame */
    void Update()
    {
        /* Check if there are no touches; if true, exit the function. */
        if (Input.touchCount == 0) return;

        /* Declare variables */
        RaycastHit hit;
        Ray ray = m_ArCam.ScreenPointToRay(Input.GetTouch(0).position);

        /* Check if there is a collision with a plane using ARRaycastManager */
        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            /* Check if touch has just begun and no object is spawned yet */
            if (Input.GetTouch(0).phase == TouchPhase.Began && m_SpawnedObject == null)
            {
                /* Check if there's a raycast hit with a game object tagged as "Spawnable" */
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        /* Set the spawned object to the hit game object */
                        m_SpawnedObject = hit.collider.gameObject;
                    }
                    else
                    {
                        /* Get current object spawned */
                        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Spawnable"); 

                        /* Check if there's already an object spawned */
                        if (objectsWithTag.Length > 0) 
                        {
                            /* Move object to touch position */
                            objectsWithTag[0].transform.position = m_Hits[0].pose.position;
                        }
                        else
                        {
                            /* Spawn a prefab at the hit pose position if not hitting a "Spawnable" object */
                            SpawnPrefab(m_Hits[0].pose.position);
                        }
                    }
                }
            }

            /* Check if touch is moved and an object is already spawned */
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && m_SpawnedObject != null)
            {
                /* Move the spawned object to the ARRaycastManager hit pose position */
                m_SpawnedObject.transform.position = m_Hits[0].pose.position;
            }

            /* Check if touch has ended, reset the spawned object */
            if (Input.GetTouch(0).phase == TouchPhase.Ended) m_SpawnedObject = null;
        }
    }

    /* SpawnPrefab: Spawns the prefab at the given position and assigns it to m_SpawnedObject */
    private void SpawnPrefab(Vector3 spawnPosition)
    {
        m_SpawnedObject = Instantiate(m_SpawnablePrefab, spawnPosition, Quaternion.identity);
    }
}
