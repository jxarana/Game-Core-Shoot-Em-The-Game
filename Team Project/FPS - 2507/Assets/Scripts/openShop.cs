using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openShop : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.openShop();
            gameManager.instance.playAudio(audioClip, transform, 1f, false);
        }
    }
}
