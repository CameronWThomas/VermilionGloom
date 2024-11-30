using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class VoiceBox : MonoBehaviour
{
    AudioSource audioSource;

    public List<AudioClip> angry;
    public List<AudioClip> happy;
    public List<AudioClip> convoStarter;
    public List<AudioClip> bleh;
    public List<AudioClip> hurt;
    public List<AudioClip> canOpen;
    public List<AudioClip> slurp;
    public List<AudioClip> crunchSuccess;
    public List<AudioClip> crunchFail;
    public List<AudioClip> death;

    private float pitchModulationRange = 0.5f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = 1 + Random.Range(-pitchModulationRange, pitchModulationRange);

    }

    public void PlayAngry()
    {
        PlayClip(angry);
    }
    public void PlayHappy()
    {
        PlayClip(happy);
    }
    public void PlayConvoStarter()
    {
        PlayClip(convoStarter);
    }
    public void PlayBleh()
    {
        PlayClip(bleh);
    }
    public void PlayHurt()
    {
        PlayClip(hurt);
    }
    public void PlayCanOpen()
    {
        PlayClip(canOpen);
    }
    public void PlaySlurp()
    {
        //30% chance of can open sound first
        if (Random.Range(0, 100) < 30)
        {
            PlayCanOpen();
        }
        PlayClip(slurp);
    }
    public void PlayCrunchHit()
    {
        PlayClip(crunchSuccess);
    }
    public void PlayCrunchMiss()
    {
        PlayClip(crunchFail);
    }
    public void PlayDeath()
    {
        PlayClip(death);
    }
    public void PlayClip(List<AudioClip> options)
    {
        AudioClip clip = options[Random.Range(0, options.Count)];
        //audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
