using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundArchive : MonoBehaviour
{
    public static SoundArchive Instance;

    public AudioClip[] sounds;

    public void Play(int clipIndex, Transform parent)
    {
        GameObject obj = new GameObject();

        obj.transform.parent = parent;

        obj.transform.position = parent.position;
        AudioSource source = obj.AddComponent<AudioSource>();

        source.clip = sounds[clipIndex];
        source.Play();
        Destroy(obj, source.clip.length + 1);
    }

    private void Awake()
    {
        Instance = this;
    }
}
