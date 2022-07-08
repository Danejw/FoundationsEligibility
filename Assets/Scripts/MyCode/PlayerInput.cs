using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private bool debug;

    [SerializeField] private Transform cursor;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Vector3 mouseWorldPosition;
    [SerializeField] private RaycastHit hit;



    [SerializeField] private GameObject interactingObject;
    [SerializeField]
    public GameObject InteractingObject
        {
            get
            {
                return interactingObject;
            }
            set
            {
                if (value == null)
                {
                    if (debug) Debug.Log("No longer interacting with " + interactingObject.name);
                }

                interactingObject = value;

                if (value != null)
                {
                    if (debug) Debug.Log("Interacting with " + value.name);
                }

            }
        }

    private void Update()
    {
        // Screen to 3D
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            mouseWorldPosition = hit.point;
            cursor.position = mouseWorldPosition;

            // Sets the cursor active
            cursor.gameObject.SetActive(true);

            //Check for mouse click 
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.tag == "Tic")
                {
                    hit.transform.GetComponent<TicTacToeTrigger>().TriggerHit();
                }
            }

            // Caches the object being interacted with and send an event with that data
            if (interactingObject != hit.transform.gameObject)
                InteractingObject = hit.transform.gameObject;

            if (debug) Debug.Log(hit.transform.name);
        }
        else
        {
            cursor.gameObject.SetActive(false);

            if (interactingObject != null)
                InteractingObject = null;
        }
    }
}
