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
    [SerializeField] private int lifeSpan;

    [SerializeField] private float damageDuration;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float impactForce;
    [SerializeField] private float range;

    [Header("Prefab")]
    [SerializeField] private GameObject spawnObject;
    [Header("SFX")]
    [SerializeField] private AudioClip spawnSFX;
    [SerializeField] private AudioClip hitSFX;
    [Header("Animation")]
    [SerializeField] private AnimationClip castAnimation;
    [Header("FX")]
    [SerializeField] private ParticleSystem spawnFX;
    [SerializeField] private ParticleSystem hitFX;
    [Header("UI Icon")]
    [SerializeField] private Image icon;
    [SerializeField] private enum MoveMethod { Linear, Curvilinear, Fixed }
    [SerializeField] private enum Element { Air, Earth, Fire, Water }

    [Header("Unlock Status")]
    [SerializeField] private bool isUnlocked;

    // GETTER
    public float MoveSpeed => moveSpeed;
    public int LifeSpan => lifeSpan;

    // FOR RESETTING SO
    public void ResetBaseStats()
    {
        // fill later
    }

    public void Cast(Transform spawnPosition)
    {
        GameObject projectile = Instantiate(spawnObject, spawnPosition.transform.position, spawnPosition.transform.rotation);
        
    }
}