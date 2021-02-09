using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{   

    // serializing 2 vars for the ball prefab and the rigidbody
    // pivot point

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;

    // serializing a delay for time between finger release
    // and ball detach

    [SerializeField] private float detachDelay = 0.1f;

    [SerializeField] private float respawnDelay = 2.0f;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint;
    
    private Camera mainCamera;
    private bool isDragging;

    void Start()
    {
        mainCamera = Camera.main;
        
        EnhancedTouchSupport.Enable();

        SpawnNewBall();
    }
    void Update() 
    {
        // ensures we're not doing anything if there's
        // no reference to the ball

        if(currentBallRigidbody == null) 
        {
            return;
        }
        else
        {       
            currentBallRigidbody.isKinematic = false;

                if(Touch.activeTouches.Count == 0)
                { 
                    if(isDragging)
                    {
                        LaunchBall();
                    }
                    
                    isDragging = false;
                    
                    return; 
                }

            // setting our ball to not interact with physics
            // set dragging bool to true

            currentBallRigidbody.isKinematic = true;
            isDragging = true;

            // get position of our first finger touch

            Touch currentTouch = Touch.activeTouches[0];

            // taking our camera's xy coords and converting them to
            // world point, then setting our rigid body pos to this point

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(currentTouch.screenPosition);

            currentBallRigidbody.position = worldPosition;
        }

    }

    private void SpawnNewBall()
    {
        // creating an instance of the ball prefab, spawning
        // at the pivot position
        
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        
        // redefining the currentball vars to the new ball instance

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        // connecting the spring joint to our pivot

        currentBallSpringJoint.connectedBody = pivot;
    }

    // reapplies physics
    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;
        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }

}
