using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    private float speed;
    public float minSpeed, maxSpeed;

    private Transform player;
    Rigidbody2D rb;
    private Vector2 moveDirection;
    public GameObject deathEffect;
    private CameraShake camShake;

    void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        rb = GetComponent<Rigidbody2D>();
        speed = Random.Range(minSpeed,maxSpeed);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        moveDirection = (player.transform.position - transform.position).normalized * speed;
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y);
    }

    private void OnTriggerEnter2D(Collider2D c) 
    {
        if(c.gameObject.tag == "Player")
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        var deathEffectCopy = Instantiate(deathEffect, transform.position, Quaternion.identity);
        camShake.Shake(0.1f,0.1f);
        Destroy(gameObject);
        Destroy(deathEffectCopy, 0.3f);
    }
}
