using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AsteroidMovement : MonoBehaviour
{
    [System.Serializable]
    public class AsteroidStats
    {
        public int maxHealth = 40;
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

    public AsteroidStats astStats = new AsteroidStats();

    [SerializeField] private GameObject rock;
    [SerializeField] private Transform[] spawn;

    public float asteroidSpeedMin = 2f;
    public float asteroidSpeedMax = 5f;
    public GameObject impactEffect;
    public GameObject deathEffect;
    float speed;
    private int damageReceived = 10;
    private Rigidbody2D rb;
    [SerializeField] private TMP_Text asteroidLifeText;
    private GameMaster gm;
    private SpriteRenderer sr;
    [SerializeField] private int numbOfFlashes;
    [SerializeField] private float flashDur;
    private AudioManager audioM;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = Random.Range(asteroidSpeedMin,asteroidSpeedMax);  
        astStats.Init();
        gm = GameObject.Find("GameManager").GetComponent<GameMaster>();
        sr = GetComponent<SpriteRenderer>();
        audioM = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        asteroidLifeText.text = astStats.CurrHealth.ToString();
    }

    private void OnTriggerEnter2D(Collider2D c) 
    {
        if(c.gameObject.tag == "Player")
        {
            var impEffCopy = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(impEffCopy, 0.3f);
            Destroy(gameObject);
        }

        if(c.gameObject.tag == "Bullet")
        {
            TakeDamage(damageReceived);
            if (astStats.CurrHealth <= 0)
            {

                //gm.KillDisaster(this); 
                speed = 0;
                StartCoroutine(Flash(numbOfFlashes, flashDur));
                asteroidLifeText.alpha = 0;
                gameObject.transform.Find("AsteroidTrail").GetComponent<ParticleSystem>().Stop();
            }
        }

        if (c.gameObject.tag == "Enemy")
        {
            //var impEffCopy = Instantiate(impactEffect, transform.position, Quaternion.identity);
            //Destroy(impEffCopy, 0.3f);
            TakeDamage(astStats.CurrHealth);
            if (astStats.CurrHealth <= 0)
            {

                //gm.KillDisaster(this); 
                speed = 0;
                StartCoroutine(Flash(numbOfFlashes, flashDur));
                asteroidLifeText.alpha = 0;
                gameObject.transform.Find("AsteroidTrail").GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    void TakeDamage(int damageTaken)
    {
        astStats.CurrHealth -= damageTaken;
    }

    private IEnumerator Flash(int numbOfFlashes, float flashDuration)
    {
        int temp = 0;
        Color initialColor = sr.color;
        GetComponent<PolygonCollider2D>().enabled = false;
        while(temp<=numbOfFlashes)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            sr.color = initialColor;
            yield return new WaitForSeconds(flashDuration);
            temp++;
        }
        var deathEffCopy = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffCopy, 0.3f);
        audioM.Play("AsteroidExplosion");
        gm.KillDisaster(this);
        for (int i = 0; i < spawn.Length; i++)
        {
            Vector3 cv = spawn[i].position;
            var rockCopy = Instantiate(rock, cv, Quaternion.identity);
            rockCopy.gameObject.name = "Rock" + i;
        }

    }
}
