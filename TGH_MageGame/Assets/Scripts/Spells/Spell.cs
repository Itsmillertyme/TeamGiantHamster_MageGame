using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

[CreateAssetMenu]

public class Spell : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private string description;
    [SerializeField] private string loreText;

    [SerializeField] private int currentLevel;
    [SerializeField] private int maxLevel;
    [SerializeField] private int projectileCount;
    [SerializeField] private int manaCost;
    [SerializeField] private int damage; // TOTAL COMBINED DAMAGE
    [SerializeField] private int lifeSpan;

    [SerializeField] private float damageDuration;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float impactForce;
    [SerializeField] private float range;

    [SerializeField] private GameObject shotSpawn;

    [SerializeField] private AudioClip spawnSFX;
    [SerializeField] private AudioClip hitSFX;

    [SerializeField] private Animation castAnimation;

    [SerializeField] private ParticleSystem spawnFX;
    [SerializeField] private ParticleSystem hitFX;

    [SerializeField] Image icon;

    [SerializeField] private enum MoveMethod { Linear, Curvilinear, Fixed }
    [SerializeField] private enum Element { Air, Earth, Fire, Water }

    [SerializeField] GameObject prefab;

    [SerializeField] bool isUnlocked;

    // FOR RESETTING SO
    public void ResetBaseStats()
    {
        // fill later
    }

    public void Cast()
    {
        GameObject projectile = Instantiate(prefab, shotSpawn.transform.position, shotSpawn.transform.rotation);
        
    }
}