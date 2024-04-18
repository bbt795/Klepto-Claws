using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider VolumeSlider;
    private AudioSource audioSource;
    public GameObject ObjectMusic;
    private float musicVolume = 1f;
    void Start()
    {
        ObjectMusic = GameObject.FindWithTag("GameMusic");
        audioSource = ObjectMusic.GetComponent<AudioSource>();

        //SetVolume
        if(PlayerPrefs.HasKey("volume"))
        {
            musicVolume = PlayerPrefs.GetFloat("volume");
        }
        else
        {
            musicVolume = 1f;
            PlayerPrefs.SetFloat("volume", musicVolume);
            PlayerPrefs.Save();
        }
        
        audioSource.volume = musicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = musicVolume;
        PlayerPrefs.SetFloat("volume", musicVolume);
    }

    public void UpdateVolume(float volume)
    {
        musicVolume = volume;
    }
}
