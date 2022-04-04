using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Disasters")
        {
            Destroy(other.gameObject,0.1f);
        }    
    }
}
