using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


//script to show and hide hamburger menu
public class MoveMenu : MonoBehaviour
{
    //objects
    public GameObject menuPanel;
    public GameObject menuOriginPos;
    public GameObject menuActivePos;

    // used to disable raycasting when menu is open
    [SerializeField] ARRaycastManager raycastManager;

    //move variables
    public bool Move_Menu_Panel = false; //are we moving the panel, false by default
    enum menuDirection
    {
        Extend, Retract
    }
    private menuDirection Move_Menu_Direction = menuDirection.Retract; //direction, retracting by default

    public float moveSpeed = 2; // speed
    public float MoveTolerance = 0.1f;
    void Start()
    {
        //sets to origin
        menuPanel.transform.position = menuOriginPos.transform.position;
    }

    void Update()
    {
        //should we move the menu
        if (Move_Menu_Panel)
        {
            if (Move_Menu_Direction == menuDirection.Extend)
            {
                Debug.Log("MoveMenu::Update -> Extending");
                menuPanel.transform.position = Vector3.Lerp(menuPanel.transform.position, menuActivePos.transform.position, moveSpeed * Time.deltaTime);


                if (menuPanel.transform.localPosition.x > menuActivePos.transform.localPosition.x * 1 - MoveTolerance)
                {
                    Move_Menu_Panel = false;
                }
            }
            else
            {
                menuPanel.transform.position = Vector3.Lerp(menuPanel.transform.position, menuOriginPos.transform.position, moveSpeed * Time.deltaTime);
                Debug.Log("MoveMenu::Update -> Retracting");


                if (menuPanel.transform.localPosition.x < menuOriginPos.transform.localPosition.x * 1+MoveTolerance)
                {
                    Move_Menu_Panel = false;
                }
            }
        }
    }

    public void MovePanel()
    {
        Debug.Log("MoveMenu::MovePanel -> Changing Direction");

        if (Move_Menu_Direction == menuDirection.Extend)  {
            Move_Menu_Direction = menuDirection.Retract;
            raycastManager.enabled = true;
        }

        else  {
            Move_Menu_Direction = menuDirection.Extend;
            raycastManager.enabled = false;
        }

        Move_Menu_Panel = true;
    }



}
