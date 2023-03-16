using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceTestMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 5f; 
    public float tilesLenght = 4f; 
    public LayerMask obstacleMask; 

    private Vector3 destination; 
    private Vector3 previousTile;
    private bool isMoving = false;

    [Header("Rotation")]
    public float rotationSpeed = 360f; 
    public float rotationThreshold = 0.01f; 

    private bool isRotating = false; 
    private Quaternion targetRotation; 

    void Update()
    {
        MovementKeysListener();
        RotationKeyListener();
    }

    void MovementKeysListener()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        if ((horizontalInput != 0 || verticalInput != 0) && !isMoving)
        {
            // Calculate the position of the tile the player is moving towards
            Vector3 direction = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;
            destination = transform.position + direction*tilesLenght;
            destination.y = transform.position.y;
            previousTile = transform.position;
            if (CanMove())
            {
                isMoving = true;
            }
        }

        // Move the player towards the destination
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            if (transform.position == destination)
            {
                isMoving = false; // Stop moving when the destination is reached
            }
            if(!CanMove())
            {
                destination = previousTile; // Back to previous tile if collide with something
            }
        }
    }

    // Check if the player can move to a given destination
    bool CanMove()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.2f, obstacleMask);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Wall")
            {
                return false;
            }
        }
        return true;
    }

    void RotationKeyListener()
    {
        // Get target rotation
        if (Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            isRotating = true;
            targetRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            isRotating = true;
            targetRotation = transform.rotation * Quaternion.Euler(0, -90, 0);
        }

        // Rotate towards target rotation
        if(isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if(Quaternion.Angle(transform.rotation, targetRotation) < rotationThreshold)
            {
                isRotating = false;
                transform.rotation = targetRotation;
            }
        }
    }

}
