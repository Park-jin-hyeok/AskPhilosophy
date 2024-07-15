using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBob : MonoBehaviour
{
    public float maxBobbingSpeed = 5f;
    public float maxBobbingAmount = 0.5f;
    public float maxMovingSpeed = 2f;
    public KeyCode triggerKey = KeyCode.Space; // Trigger key for gravity

    private Vector3 startPosition;
    private Rigidbody rb;
    private bool isGravityEnabled = false;
    private float currentBobbingSpeed;
    private float currentMovingSpeed;
    private float bobbingPhase;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable gravity at start

        // Initialize random bobbing and moving speeds
        currentBobbingSpeed = Random.Range(0.5f * maxBobbingSpeed, maxBobbingSpeed);
        currentMovingSpeed = Random.Range(0.5f * maxMovingSpeed, maxMovingSpeed);
        bobbingPhase = Random.Range(0f, 2f * Mathf.PI); // Random phase for bobbing
    }

    void Update()
    {
        if (!isGravityEnabled)
        {
            // Random bobbing up and down
            float bobbing = Mathf.Sin(Time.time * currentBobbingSpeed + bobbingPhase) * maxBobbingAmount;
            transform.position = startPosition + new Vector3(0, bobbing, 0);

            // Random moving around a bit
            transform.position += new Vector3(Mathf.Sin(Time.time * currentMovingSpeed) * 0.1f, 0, 0);
        }

        // Check for the trigger
        if (Input.GetKeyDown(triggerKey))
        {
            isGravityEnabled = true;
            rb.useGravity = true; // Enable gravity
        }
    }
}


