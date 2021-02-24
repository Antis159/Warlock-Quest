using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSoundController : MonoBehaviour
{
    public Sprite emptyCheckbox;
    public Sprite fullCheckbox;

    public bool musicToggle;
    public bool soundToggle;

    private void Start()
    {
        musicToggle = true;
        soundToggle = true;
    }

    public Sprite GetMusicCheckboxSprite()
    {
        if (musicToggle == false)
            return emptyCheckbox;
        else
            return fullCheckbox;
    }

    public Sprite GetSoundCheckboxSprite()
    {
        if (soundToggle == false)
            return emptyCheckbox;
        else
            return fullCheckbox;
    }
}
