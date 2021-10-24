using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

	static SoundManagerScript instance;

    public static AudioClip defenderAttack, soldierAttack, spitterAttack, defenderDeath, soldierDeath, spitterDeath, hiveDestruction, projectileHit;
    public AudioClip[] levelTracks;
    public AudioSource musicSrc;
	public AudioSource fxSrc;


	// Start is called before the first frame update
	void Awake()
    {
		instance = this;

        defenderAttack = Resources.Load<AudioClip>("Audio/FX/Defender_Attack");
        soldierAttack = Resources.Load<AudioClip>("Audio/FX/Soldier_Attack");
        spitterAttack = Resources.Load<AudioClip>("Audio/FX/Spitter_Attack");
        projectileHit = Resources.Load<AudioClip>("Audio/FX/Projectile_Hit");
        defenderDeath = Resources.Load<AudioClip>("Audio/FX/Defender_Death");
        soldierDeath = Resources.Load<AudioClip>("Audio/FX/Soldier_Death");
        spitterDeath = Resources.Load<AudioClip>("Audio/FX/Spitter_Death");
        hiveDestruction = Resources.Load<AudioClip>("Audio/FX/Hive_Destruction");


		musicSrc.volume = Options.VolumeMusic;
		fxSrc.volume = Options.VolumeFX;
	}


    // Update is called once per frame
    void Update()
    {
        if (!musicSrc.isPlaying)
        {
			musicSrc.clip = levelTracks[Random.Range(0, levelTracks.Length)];
			musicSrc.Play();
        }
    }


    public static void PlaySound (string clip)
    {
        switch (clip) {
            case "Defender_Attack":
				instance.fxSrc.PlayOneShot(defenderAttack);
				break;
            case "Soldier_Attack":
				instance.fxSrc.PlayOneShot(soldierAttack);
				break;
            case "Spitter_Attack":
				instance.fxSrc.PlayOneShot(spitterAttack);
				break;
            case "Projectile_Hit":
				instance.fxSrc.PlayOneShot(projectileHit);
				break;
            case "Defender_Death":
				instance.fxSrc.PlayOneShot(defenderDeath);
				break;
            case "Soldier_Death":
				instance.fxSrc.PlayOneShot(soldierDeath);
				break;
            case "Spitter_Death":
				instance.fxSrc.PlayOneShot(spitterDeath);
                break;
            case "Hive_Destruction":
				instance.fxSrc.PlayOneShot(hiveDestruction);
                break;
        }
    }


	void OnDestroy() {
		instance = null;
	}


	public static void SetMusicVolume(float t) => instance.musicSrc.volume = t;
	public static void SetFXVolume(float t) => instance.fxSrc.volume = t;
}
