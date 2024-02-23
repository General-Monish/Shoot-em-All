using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayMusic(menuTheme,2);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AudioManager.instance.PlayMusic(mainTheme,3);
        }
    }
}
