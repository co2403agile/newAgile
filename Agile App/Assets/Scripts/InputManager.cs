using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class InputManager : MonoBehaviour
{
    // TODO: ADD SUPPORT FOR ROTATION AND SCALE

    /* Reference to the ARRaycastManager for raycasting */
    [SerializeField] ARRaycastManager m_RaycastManager; 

    /* List to store raycast hits */
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    /* Reference to the AR Camera */
    Camera m_ArCam; 

    /* Position vector for raycast hit */
    private Vector3 posVector3 = new Vector3(0, 0, 0); 

    /* Scale of the object (not implemented yet) */
    private float scale = 1; 

    /* Rotation of the object (not implemented yet) */
    private float rotation = 0;

    /* Flag to indicate if a raycast has been performed */
    private bool raycastFlag = false; 

    /* Delegate for raycast update event */
    public delegate void RayCastUpdateHandler();

    /* Event triggered when raycast updates */
    public event RayCastUpdateHandler onUpdate;

    /* Start: called before the first frame update */
    void Start()
    {
        /* Find AR Camera object */
        m_ArCam = GameObject.Find("AR Camera").GetComponent<Camera>(); 
    }

    /* Update: called once per frame */
    void Update()
    {
        /* Base Case: Check if there are no touches; if true, exit the function. */
        if (Input.touchCount == 0) return;

        /* Declare variables */
        RaycastHit hit;
        Ray ray = m_ArCam.ScreenPointToRay(Input.GetTouch(0).position);

        /* Check if there is a collision with a plane using ARRaycastManager */
        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            /* Check if touch has just begun and no object is spawned yet */
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                /* Check if there's a raycast hit */
                if (Physics.Raycast(ray, out hit))
                {
                    /* Put collision point into posVector3 */
                    posVector3 = hit.point;
                }
            }
        }

        /* Set the flag to indicate that a raycast has been performed */
        raycastFlag = true; 

        /* Call every function that relies on the raycast updates */
        onUpdate.Invoke();
    }

    /* GetPos: Get the position vector */
    public Vector3 GetPos()
    {
        return posVector3;
    }

    /* GetScale: Get the scale factor (not implemented yet) */
    public float GetScale()
    {
        return scale;
    }

    /* GetRotation: Get the rotation angle (not implemented yet) */
    public float GetRotation()
    {
        return rotation;
    }

    /* GetRaycastFlag: Get the raycast flag, used to check if the user has raycasted once */
    public bool GetRaycastFlag()
    {
        return raycastFlag;
    }
}
