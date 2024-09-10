using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    [SerializeField] private Spell spellCasted;
    [SerializeField] private MousePositionTracking mousePositionTracker;
    private Vector3 moveDirection;

    private void Start()
    {
        Vector3 targetPosition = mousePositionTracker.CurrentPosition;
        moveDirection = (targetPosition - transform.position).normalized;
        Destroy(this.gameObject, spellCasted.LifeSpan);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {  
        // MOVE TOWARD TARGET
        transform.position += moveDirection * spellCasted.MoveSpeed * Time.deltaTime;
    }
}