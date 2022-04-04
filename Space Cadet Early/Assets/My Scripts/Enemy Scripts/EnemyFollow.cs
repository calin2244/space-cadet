using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyFollow : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats
    {
        public int maxHealth = 50;
        private int currentHealth;

        public int CurrHealth
        {
            get { return currentHealth; }
            set { currentHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            CurrHealth = maxHealth;
        }
    }

    public EnemyStats enemyStat = new EnemyStats();
    GameMaster gm;

    [SerializeField] private float DetectDistance, RetreatDistance, TurnSpeed = 5f;
    private float speed;

    [Header("Speed details")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    public Transform target;
    public GameObject deathEffect;
    Animation myAnimation;
    private float healthBarSize = 1f;

    [HideInInspector]
    public bool isDead = false;
    [SerializeField] private EnemyStatusIndicator statusIndicator;

    [Header("Camera Shake Things")]
    CameraShake camShake;
    [SerializeField] private float camShakeAmount;

    [Header("Shooting Properties")]
    private float timeBtwShots;
    public float startTimeBtwShots;
    public GameObject projectile;
    private float speedCopy;

    [SerializeField] private bool isAttacking;

    [Header("Pop Up Damage Taext")]
    [SerializeField] private GameObject popUpDamageText;
    [SerializeField] private Transform whereToSpawnPopUp;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        speed = UnityEngine.Random.Range((int)minSpeed, maxSpeed);
        speedCopy = speed;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        gm = GameObject.Find("GameManager").GetComponent<GameMaster>();
        camShake = gm.GetComponent<CameraShake>();
    }

    void Start()
    {    

        if (camShake == null)
        {
            Debug.LogError("No camera shake script found :(");
        }

        enemyStat.Init();

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStat.CurrHealth, enemyStat.maxHealth);
        }
       
    }

    void Update()
    {
        if (isAttacking)
        {
            speed = 0;
        }
        else
        {
            Follow();
            speed = speedCopy;
        }

        Rotate();

        if (timeBtwShots <= 0)
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
            isAttacking = true;
            timeBtwShots = startTimeBtwShots;
        }
        else
        {
            isAttacking = false;
            timeBtwShots -= Time.deltaTime;
        }
    }

    private void Follow()
    {
        if (target.gameObject != null)
        {
            if (Vector2.Distance(transform.position, target.position) > DetectDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
            else if (Vector2.Distance(transform.position, target.position) < DetectDistance
            && Vector2.Distance(transform.position, target.position) > RetreatDistance)
            {
                transform.position = this.transform.position;
            }
            else if (Vector2.Distance(transform.position, target.position) < RetreatDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
            }
        }
    }

    public void Rotate()
    {
        if (Vector2.Distance(target.transform.position, transform.position) > 0.4f)
        {
            float AngleRad = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x);
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, (AngleDeg + 90)), TurnSpeed);
        }
    }

    public void TakeDamage(int damage)
    {
        enemyStat.CurrHealth -= damage;
        //Debug.Log(enemyStat.CurrHealth);

        if (enemyStat.CurrHealth <= 0)
        {
            var deathEffectCopy = Instantiate(deathEffect, transform.position, Quaternion.identity);
            camShake.Shake(camShakeAmount, 0.1f);
            FindObjectOfType<AudioManager>().Play("DeathSound");
            gm.KillEnemy(this);
            Destroy(deathEffectCopy, 0.3f);
            IsDead();
        }
        else
        {
            isDead = false;
        }

        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStat.CurrHealth, enemyStat.maxHealth);
        }

    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            //Debug.Log("Inamic ataca");
            var deathEffectCopy = Instantiate(deathEffect, transform.position, Quaternion.identity);
            //gm.KillEnemy(this);
            Destroy(deathEffectCopy, 0.3f);
            camShake.Shake(camShakeAmount, 0.1f);
            FindObjectOfType<AudioManager>().Play("DeathSound");
        }
        else if (c.gameObject.tag == "Rockets")
        {
            var deathEffectCopy = Instantiate(deathEffect, transform.position, Quaternion.identity);
            gm.KillEnemy(this);
            Destroy(c.gameObject);
            Destroy(deathEffectCopy, 0.3f);
            camShake.Shake(camShakeAmount, 0.1f);
            FindObjectOfType<AudioManager>().Play("RocketExplosion");
        }
        else if (c.gameObject.tag == "RockPiece")
        {
            TakeDamage(20);
        }
        else if (c.gameObject.tag == "Disasters")
        {
            TakeDamage(enemyStat.maxHealth / 2);
            FindObjectOfType<AudioManager>().Play("ImpactSound");
        }

    }

    private void IsDead()
    {
        isDead = true;
    }
}
