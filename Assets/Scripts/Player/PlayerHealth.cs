﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
	public int startingHealth = 100;
	public int currentHealth;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

	Slider healthSlider;
	Image damageImage;
	Text healthText;
    Animator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;
    bool isDead;
    bool damaged;


    void Awake ()
	{
		GameObject slider = GameObject.Find ("HealthSlider");
		if (slider) {
			healthSlider = slider.gameObject.GetComponent<Slider> ();
		}
		
		GameObject image = GameObject.Find ("DamageImage");
		if (image) {
			damageImage = image.GetComponent<Image> ();
		}

		healthText = GameObject.Find ("HealthText").GetComponent<Text>();

		anim = GetComponent <Animator> ();
		playerAudio = GetComponentInChildren <AudioSource> ();
		playerMovement = GetComponentInChildren <PlayerMovement> ();
		playerShooting = GetComponentInChildren <PlayerShooting> ();
		currentHealth = startingHealth;
    }

	void Start()
	{
	}


    void Update ()
    {
		if (networkView.isMine) 
		{
			healthText.text = currentHealth + "/" + startingHealth;
		}
        if(damaged && networkView.isMine)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting) 
        {
            int syncHealth = currentHealth;
            int syncMaxHealth = startingHealth;

            stream.Serialize(ref syncHealth);
            stream.Serialize(ref syncMaxHealth);
        } 
        else 
        {
            int syncHealth = 0;
            int syncMaxHealth = 0;
            stream.Serialize(ref syncHealth);
            stream.Serialize(ref syncMaxHealth);
            currentHealth = syncHealth;
            startingHealth = syncMaxHealth;
        }
    }

    public void TakeDamage (int amount)
    {
        damaged = true;

        currentHealth -= amount;

		if (networkView.isMine) 
		{
			healthSlider.value = currentHealth;
		}
        playerAudio.Play ();

        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
    }


    void Death ()
    {
        isDead = true;

        playerShooting.DisableEffects ();

        anim.SetTrigger ("Die");

        playerAudio.clip = deathClip;
        playerAudio.Play ();

        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }


//    public void RestartLevel ()
//    {
//        Application.LoadLevel (Application.loadedLevel);
//    }
}
