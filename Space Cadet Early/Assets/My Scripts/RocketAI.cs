using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RocketAI : MonoBehaviour
{
    #region Variables
    public Transform target;
    private float speed;
    public float turnSpeed;
    public float camShakeAmt;
    CameraShake camShake;
    private float interval = 8;
    public GameObject deathEffect;
    ParticleSystem rocketParticles;
    [SerializeField] private TextMeshProUGUI autoDestructTimerText;
    private AudioManager audioM;

    [Header("Speed Options")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    #endregion

    #region AwakeStartUpdate
    private void Awake() 
    {
        interval = Random.Range(6,9);
        speed = Random.Range((int)minSpeed, maxSpeed);
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();       
        camShake = FindObjectOfType<GameMaster>().GetComponent<CameraShake>();
        interval = Random.Range(7, 10);
        audioM = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }
    
    void Start()
    {
        if (camShake == null)
        {
            Debug.LogError("No camera shake script found :(");
        }       
    }

    
    void Update()
    {
        Follow();
        Rotate();
        AutoDistruct();
        //Debug.Log(interval);
    }

    #endregion

    #region Methods
    private void AutoDistruct()
    {       
        if(interval>0)
        {
            interval -= Time.deltaTime;
            autoDestructTimerText.text = interval.ToString("F1");
        }
        else
        {
            var deathEffectCopy = Instantiate(deathEffect, transform.position, Quaternion.identity);
            enabled = false;
            Destroy(this.gameObject);
            Destroy(deathEffectCopy, 0.3f);
            audioM.Play("RocketExplosion");
        }
    }

    private void Follow()
    {
        if (target.gameObject != null)
        {
            if (Vector2.Distance(target.transform.position, transform.position) > 0)
            {
                GetComponent<Rigidbody2D>().velocity = Vector3.Lerp(Vector3.zero, transform.up * speed, 0.8f);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / speed;
            }         
        }
    }

    public void Rotate()
    {
        if (Vector2.Distance(target.transform.position, transform.position) > 0.4f)
        {
            float AngleRad = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x);
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, (AngleDeg - 90)), turnSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D c) 
    {
        if(c.gameObject.tag == "Player")
        {
            var deathEffectCopy = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(deathEffectCopy, 0.3f);
            camShake.Shake(camShakeAmt, 0.1f);
            audioM.Play("RocketExplosion");
        }

        if (c.gameObject.tag == "Rockets" || c.gameObject.tag == "EnemyProjectile" || c.gameObject.tag == "Enemy")
        {
            var deathEffectCopy = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(c.gameObject);
            Destroy(deathEffectCopy, 0.3f);
            camShake.Shake(camShakeAmt, 0.1f);
            audioM.Play("RocketExplosion");
        }

    }

    #endregion           
}
