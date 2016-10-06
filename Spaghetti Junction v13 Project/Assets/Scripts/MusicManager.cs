using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public AudioSource[] music;

    private int prevSong;

    // Use this for initialization
    void Start () {
        music = GetComponents<AudioSource>();
        int rand = Random.Range(0, 3);
        prevSong = rand;
        music[rand].Play();
    }
	
    public void ToggleMusic()
    {
        if (music[0].isPlaying || music[1].isPlaying || music[2].isPlaying)
        {
            music[prevSong].Stop();
            int rand = Random.Range(3, 5);
            prevSong = rand;
            music[rand].Play();
        }
        else
        {
            music[prevSong].Stop();
            int rand = Random.Range(0, 3);
            prevSong = rand;
            music[rand].Play();
     
        }
    }

	public void setMusicSpeed(float speed) {
		music [prevSong].pitch = speed;
	}

    public void GameOver()
    {
        music[prevSong].Stop();
        music[5].Play();
        prevSong = 5;
    }
}
