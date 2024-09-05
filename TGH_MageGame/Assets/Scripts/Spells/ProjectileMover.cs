using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    [SerializeField] private Spell spellCasted;

    private void Start()
    {
        Destroy(this.gameObject, spellCasted.LifeSpan);
        }

    private void Update()
    {
        Move();
        Destroy(gameObject, spellCasted.LifeSpan);
    }

    private void Move()
    {  
        // MOVE FORWARD THEN DESTORY AFTER SO LONG
        transform.position += transform.forward * spellCasted.MoveSpeed * Time.deltaTime;
    }
}