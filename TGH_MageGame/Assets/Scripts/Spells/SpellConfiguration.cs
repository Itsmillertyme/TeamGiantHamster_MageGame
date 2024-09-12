using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

[CreateAssetMenu]

public class Spell : ScriptableObject
{
    [Header("Information")]
    [SerializeField] private new string name;
    [SerializeField] private string description;
    [SerializeField] private string loreText;
    [Header("Levels")]
    [SerializeField] private int currentLevel;
    [SerializeField] private int maxLevel;
    [Header("Spell Attributes")]
    [SerializeField] private int projectileCount;
    [SerializeField] private int manaCost;
    [SerializeField] private int damage; // TOTAL COMBINED DAMAGE
    [SerializeField] private float lifeSpan;
    [SerializeField] private float castDelayTime;
    //[SerializeField] private float damageDuration;
    [SerializeField] private float moveSpeed;
    //[SerializeField] private float impactForce;
    //[SerializeField] private float range;
    [Header("Prefab")]
    [SerializeField] private GameObject spawnObjectLvl1;
    [SerializeField] private GameObject spawnObjectLvl2;
    [SerializeField] private GameObject spawnObjectLvl3;
    //[Header("SFX")]
    //[SerializeField] private AudioClip spawnSFX;
    //[SerializeField] private AudioClip hitSFX;
    [Header("Animation")]
    [SerializeField] private AnimationClip castAnimation;
    //[Header("FX")]
    //[SerializeField] private ParticleSystem spawnFX;
    //[SerializeField] private ParticleSystem hitFX;
    //[Header("UI Icon")]
    //[SerializeField] private Image icon;
    [SerializeField] private enum MoveMethod { Linear, Curvilinear, Fixed }
    [SerializeField] private enum Element { Air, Earth, Fire, Water }
    [Header("Unlock Status")]
    [SerializeField] private bool isUnlocked;
    [Header("Debugging")]
    [SerializeField] private Vector3 targetPosition;

    // GETTER
    public int ManaCost => manaCost;
    public int Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float LifeSpan => lifeSpan;
    public float CastDelayTime => castDelayTime;
    public string Name => name;
    public AnimationClip CastAnimation => castAnimation;
    public Vector3 TargetPosition => targetPosition;

    public bool IsUnlocked => isUnlocked;

    public void Cast(Transform spawnPosition)
    {
        GameObject projectile;
        switch (currentLevel)
        {
            case 1:
                projectile = Instantiate(spawnObjectLvl1, spawnPosition.transform.position, spawnPosition.transform.rotation);
                projectile.GetComponent<ProjectileMover>().SetAttributes(Damage, LifeSpan, MoveSpeed, TargetPosition);
                break;
            case 2:
                projectile = Instantiate(spawnObjectLvl2, spawnPosition.transform.position, spawnPosition.transform.rotation);
                break;
            case 3:
                Instantiate(spawnObjectLvl3, spawnPosition.transform.position, spawnPosition.transform.rotation);
                break;
            default: 
                Instantiate(spawnObjectLvl1, spawnPosition.transform.position, spawnPosition.transform.rotation);
                break;
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}