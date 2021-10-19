using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public static AudioClip defenderAttack, soldierAttack, spitterAttack, defenderDeath, soldierDeath, spitterDeath, hiveDestruction, projectileHit;
    public AudioClip[] levelTracks;
    public static AudioSource levelAudio;
    static AudioSource audioSrc;


    // Start is called before the first frame update
    void Awake()
    {
        defenderAttack = Resources.Load<AudioClip>("Audio/FX/Defender_Attack");
        soldierAttack = Resources.Load<AudioClip>("Audio/FX/Soldier_Attack");
        spitterAttack = Resources.Load<AudioClip>("Audio/FX/Spitter_Attack");
        projectileHit = Resources.Load<AudioClip>("Audio/FX/Projectile_Hit");
        defenderDeath = Resources.Load<AudioClip>("Audio/FX/Defender_Death");
        soldierDeath = Resources.Load<AudioClip>("Audio/FX/Soldier_Death");
        spitterDeath = Resources.Load<AudioClip>("Audio/FX/Spitter_Death");
        hiveDestruction = Resources.Load<AudioClip>("Audio/FX/Hive_Destruction");

        audioSrc = GetComponent<AudioSource>();
        levelAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelAudio.isPlaying)
        {
            levelAudio.clip = levelTracks[Random.Range(0, levelTracks.Length)];
            levelAudio.Play();
        }
    }


    public static void PlaySound (string clip)
    {
        switch (clip) {
            case "Defender_Attack":
                audioSrc.PlayOneShot(defenderAttack);
                break;
            case "Soldier_Attack":
                audioSrc.PlayOneShot(soldierAttack);
                break;
            case "Spitter_Attack":
                audioSrc.PlayOneShot(spitterAttack);
                break;
            case "Projectile_Hit":
                audioSrc.PlayOneShot(projectileHit);
                break;
            case "Defender_Death":
                audioSrc.PlayOneShot(defenderDeath);
                break;
            case "Soldier_Death":
                audioSrc.PlayOneShot(soldierDeath);
                break;
            case "Spitter_Death":
                audioSrc.PlayOneShot(spitterDeath);
                break;
            case "Hive_Destruction":
                audioSrc.PlayOneShot(hiveDestruction);
                break;
        }
    }
}
