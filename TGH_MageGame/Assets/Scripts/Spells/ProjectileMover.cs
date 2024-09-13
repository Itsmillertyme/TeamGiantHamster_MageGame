using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    private float lifeSpan;
    private float moveSpeed;
    private int damage;
    private Vector3 targetPosition;

    // 
    private Vector3 moveDirection;

    private void Start()
    {
        moveDirection = (targetPosition - transform.position).normalized;
        Destroy(gameObject, lifeSpan);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {  
        // MOVE TOWARD TARGET
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // ROTATE OBJECT TOWARDS DIRECTION TO MOVE
        if (moveDirection != Vector3.zero)  // Ensure there is a valid direction
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // ADD 90 ON Y-AXIS TO FIX PROJECTILE ROTATION
            transform.rotation = targetRotation * Quaternion.Euler(0, 90, 0);
        }
    }

    public void SetAttributes(int damage, float lifeSpan, float moveSpeed, Vector3 targetPosition)
    { 
        this.damage = damage;
        this.lifeSpan = lifeSpan;
        this.moveSpeed = moveSpeed;
        this.targetPosition = targetPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyHealth>().RemoveFromHealth(damage);
            Debug.Log("Collision with enemy");
        }

        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision with player");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Collision with environment");
        }
    }
}