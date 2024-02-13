using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class InputManager : MonoBehaviour
{
    // TODO: ADD SUPPORT FOR ROTATION AND SCALE

    [SerializeField] ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    Camera m_ArCam;

    private Vector3 posVector3 = new Vector3(0,0,0);
    private float scale = 1;
    private float rotation = 0;

    private bool raycastFlag = false;

    public delegate void RayCastUpdateHandler();
    public event RayCastUpdateHandler onUpdate;


    // Start is called before the first frame update
    void Start()
    {
        m_ArCam = GameObject.Find("AR Camera").GetComponent<Camera>(); //finds ar camera object
    }

    // Update is called once per frame
    void Update()
    {
        // Base Case: Check if there are no touches; if true, exit the function.
        if (Input.touchCount == 0) return;

        // Declare variables
        RaycastHit hit;
        Ray ray = m_ArCam.ScreenPointToRay(Input.GetTouch(0).position);

        // Check if there is a collision with a plane using ARRaycastManager
        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            // Check if touch has just begun and no object is spawned yet
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Check if there's a raycast hit
                if (Physics.Raycast(ray, out hit))
                {
                    // put collision point into posVector3
                    posVector3 = hit.point;
                }
            }
        }

        raycastFlag = true; //sets the flag to show other scripts that the user has cast a ray

        onUpdate.Invoke(); // calls every function that relies on the raycast updates
    }

    public Vector3 GetPos() {
        return posVector3;
    }

    public float GetScale() {
        return scale;
    }
    public float GetRotation() {
        return rotation;
    }

    public bool GetRaycastFlag()  // used when you want to check if the user has raycasted once
    {
        return raycastFlag;
    }
}
