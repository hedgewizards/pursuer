using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelegraphUI: MonoBehaviour
{

    private void Start()
    {

    }

    public void Initialize(Sprite sprite, AudioClip sound, Vector3 offset)
    {
        RectTransform rect = GetComponent<RectTransform>();
        Image image = GetComponent<Image>();
        AudioSource aud = GetComponent<AudioSource>();

        image.sprite = sprite;
        rect.position += offset;
        aud.clip = sound;
        aud.Play();
    }

    private void Update()
    {

    }
}
