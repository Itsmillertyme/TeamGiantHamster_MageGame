using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryEnemyShoot : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float directionChangeInterval = 1f;
    [SerializeField] private float moveDistance = 2f;
    private float timeSinceLastChange = 0f;

    private Vector3 moveDirection;
    private Vector3 targetPosition;

    private void Start()
    {
        // Initialize the first movement direction
        ChangeDirection();
    }

    private void Update()
    {
        timeSinceLastChange += Time.deltaTime;

        // Change direction at set intervals
        if (timeSinceLastChange >= directionChangeInterval)
        {
            ChangeDirection();
            timeSinceLastChange = 0f;
        }

        // Smoothly move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void ChangeDirection()
    {
        // Randomly choose a direction (forward, backward)
        int direction = Random.Range(1, 3);

        switch (direction)
        {
            case 1:
                moveDirection = Vector3.left;
                break;
            case 2:
                moveDirection = Vector3.right;
                break;
        }

        // Calculate the new target position based on the movement direction
        targetPosition = transform.position + (moveDirection * moveDistance);
    }
}