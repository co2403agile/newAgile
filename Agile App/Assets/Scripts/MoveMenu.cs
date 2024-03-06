using UnityEngine;
using UnityEngine.XR.ARFoundation;

/* Script to show and hide hamburger menu */
/* Written by James */
public class MoveMenu : MonoBehaviour
{
    /* The menu panel to be moved */
    public GameObject menuPanel;

    /* The original position of the menu */
    public GameObject menuOriginPos; 
    
    /* The active position of the menu */
    public GameObject menuActivePos; 

    /* Used to disable raycasting when menu is open */
    [SerializeField] ARRaycastManager raycastManager;

    /* Flag indicating whether to move the panel, false by default */
    public bool Move_Menu_Panel = false; 
    
    enum menuDirection
    {
        Extend, Retract
    }
    /* Direction of movement, retracting by default */
    private menuDirection Move_Menu_Direction = menuDirection.Retract;

    /* Speed of movement */
    public float moveSpeed = 2;

    /* Tolerance for movement completion */
    public float MoveTolerance = 0.1f;

    /* Start: called before the first frame update */
    void Start()
    {
        /* Sets the menu panel to its original position */
        menuPanel.transform.position = menuOriginPos.transform.position;
    }

    /* Update: called once per frame */
    void Update()
    {
        /* Should we move the menu panel? */
        if (Move_Menu_Panel)
        {
            if (Move_Menu_Direction == menuDirection.Extend)
            {
                Debug.Log("MoveMenu::Update -> Extending");
                /* Move the menu panel towards the active position */
                menuPanel.transform.position = Vector3.Lerp(menuPanel.transform.position, menuActivePos.transform.position, moveSpeed * Time.deltaTime);

                /* Check if movement is completed */
                if (menuPanel.transform.localPosition.x > menuActivePos.transform.localPosition.x * 1 - MoveTolerance)
                {
                    Move_Menu_Panel = false;
                }
            }
            else
            {
                /* Move the menu panel towards the original position */
                menuPanel.transform.position = Vector3.Lerp(menuPanel.transform.position, menuOriginPos.transform.position, moveSpeed * Time.deltaTime);
                Debug.Log("MoveMenu::Update -> Retracting");

                /* Check if movement is completed */
                if (menuPanel.transform.localPosition.x < menuOriginPos.transform.localPosition.x * 1 + MoveTolerance)
                {
                    Move_Menu_Panel = false;
                }
            }
        }
    }

    /* MovePanel: toggles menu move direction */
    public void MovePanel()
    {
        Debug.Log("MoveMenu::MovePanel -> Changing Direction");

        /* Change the direction of movement based on current direction */
        if (Move_Menu_Direction == menuDirection.Extend)
        {
            /* Set direction to retracting */
            Move_Menu_Direction = menuDirection.Retract;

            /* Enable raycasting */
            raycastManager.enabled = true; 
        }
        else
        {
            /* Set direction to extending */
            Move_Menu_Direction = menuDirection.Extend;
            
            /* Disable raycasting */
            raycastManager.enabled = false;
        }

        /* Set the flag to start moving the panel */
        Move_Menu_Panel = true; 
    }
}
