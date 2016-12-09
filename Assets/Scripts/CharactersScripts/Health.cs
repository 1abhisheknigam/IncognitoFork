﻿using UnityEngine;

/// <summary>
/// Keeps track of the player's health.
/// </summary>
public class Health : MonoBehaviour {
    
    /// <summary> The maximum health of the player. </summary>
    public float maxHealth;
    /// <summary> The current health of the player. </summary>
    public float health;
    /// <summary> The fraction of the player's health that remains. </summary>
    public float healthFraction {
        get { return health / maxHealth; }
    }

	/// <summary> The particles emitted when the player takes damage. </summary>
	public GameObject particle;

    /// <summary> Whether the player is dead. </summary>
    public bool dead {
        get { return health <= 0; }
    }

    /// <summary> Whether the player is invincible to damage. </summary>
    public bool invincible = false;

    /// <summary> The sound played when the player takes damage. </summary>
    [SerializeField]
    [Tooltip("The sound played when the player takes damage.")]
    private AudioClip damageSound;

    /// <summary> The audio source on the player. </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Initializes the object.
    /// </summary>
    private void Start() {
        health = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Deals damage to the player.
    /// </summary>
    /// <param name="damage">The amount of damage to deal.</param>
    /// <param name="playSound">Whether to play the damage sound.</param>
    public void Damage(float damage, bool playSound = true) {
        if (!invincible) {
            if (health > 0) {
                Logger.instance.LogDamage();
                if (playSound) {
                    audioSource.PlayOneShot(damageSound);
                }
				particle.GetComponent<EllipsoidParticleEmitter>().Emit(30);
                health -= damage;
                if (health <= 0) {
                    Die();
                }
            }
        }
    }

    /// <summary>
    /// Instantly kills the player.
    /// </summary>
    public void OHKO() {
        Damage(maxHealth, false);
    }

    /// <summary>
    /// Kills the player when health reaches 0.
    /// </summary>
    private void Die() {
        GetComponent<RigidbodyController>().SetRagdollActive(true);
        GetComponent<Interact>().enabled = false;
        transform.FindChild("Entity").gameObject.SetActive(false);
        GameOverScreen.instance.Show();
    }
}