using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPiece : MonoBehaviour
{
    [SerializeField] private float speed;
    private float timer=0, MaxTimer=75;
    private CameraShake camShake;

    private void Awake()
    {
        camShake = FindObjectOfType<CameraShake>();
    }

    void Update()
    {
        if(gameObject.name=="Rock0")
        {
            transform.position += Vector3.up * Time.deltaTime * speed;
        }
        else if(gameObject.name=="Rock1")
        {
            transform.position += Vector3.left * Time.deltaTime * speed;
        }
        else if(gameObject.name=="Rock2")
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
        }
        else if(gameObject.name=="Rock3")
        {
            transform.position += Vector3.down * Time.deltaTime * speed;
        }
    }

    private void FixedUpdate()
    {
        if (timer != MaxTimer)
            timer += 0.25f;
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Player" || c.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            camShake.Shake(0.1f, 0.05f);
            FindObjectOfType<AudioManager>().Play("Hurt");
        }
    }

}
