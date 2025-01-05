using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandShake : MonoBehaviour
{
    public float slideDistance = 20f; // Distance to slide up and down
    public float slideDuration = 1f; // Time to complete one slide up and down

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float elapsedTime = 0f;
    private bool movingUp = true;

    void Start()
    {
        startPosition = transform.localPosition;
        targetPosition = startPosition + new Vector3(0, slideDistance, 0);
    }

    void Update()
    {
        // Use unscaled delta time to avoid being affected by Time.timeScale
        elapsedTime += Time.unscaledDeltaTime;

        // Calculate the progress
        float progress = elapsedTime / slideDuration;

        // Smoothly move between positions
        transform.localPosition = Vector3.Lerp(movingUp ? startPosition : targetPosition, movingUp ? targetPosition : startPosition, progress);

        // Switch direction when the duration is reached
        if (elapsedTime >= slideDuration)
        {
            elapsedTime = 0f;
            movingUp = !movingUp;
        }
    }
}
