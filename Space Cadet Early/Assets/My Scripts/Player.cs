using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    #region Classes
    [System.Serializable]
    public class PlayerStats
    {
        public int maxHealth = 100;
        public int currentHealth;
        public int CurrHealth
        {
            get{return currentHealth; }
            set{currentHealth = Mathf.Clamp(value,0, maxHealth);}
        }

        public void Init()
        {
            CurrHealth = maxHealth;
        }
    }

    #endregion

    #region Variables
    public PlayerStats playerStats = new PlayerStats();
    [SerializeField] private float moveSpeedY = 8f, moveSpeedX = 4f;
    [SerializeField] private float padding = 5f;
    [SerializeField] private float projectileSpeed = 5f;
    float xMin, xMax, yMin, yMax;
    public GameObject laserPrefab1;
    public GameObject laserPrefab2;
    public Transform firePoint1;
    public Transform firePoint2;
    Rigidbody2D rb;
    private bool pressingToFireProjectile = false;
    Color initialBarColor, initialBarColorCopy;
    private BoxCollider2D boxC;
    
    [Header("Shooting Slider")]
    [SerializeField] private Slider bulletSlider;
    public int maxShootingSize = 100, shootingSize = 100;
    public int decreaseValue = 20;
    [SerializeField] private Image Fill;
    Color initialSliderColor;
    int CanShootFrameTimer = 0;
    public int barIncreaseValue = 10;
    private bool Reloading = false;
    private int ReloadingTimer = 0;
    private int DashingTimer=0;
    Vector2 mousePos;
    public Camera cam;
    private bool phase1 = false, empty1 = false;

    private SpriteRenderer sr;

    [Header("IFrame Stuff")]
    public Color flashColor;
    public Color regularColor;
    public float flashDuration;
    public int numberOfFlashes;

    [Header("ReloadingDurations")]
    public int CanShootFrameTimerMax = 30;
    public int MaxReloadingTimer = 30;
    public int MaxDashingTimer;

    //Config params
    private float cooldownTime;
    private float myTimer;
    private float nextFireTime, cooldownCopy;
    GlitchEffect glitch;
    
    [Header ("Damage Taken Info")]
    public int enemyProjectileDamage = 10;
    public int onEnemyHitDamage = 10;
    public int disastersDamage = 10;
    public int rockPieceDamage = 5;

    [Header ("Status Indicator for Health")]
    [SerializeField] private StatusIndicator myStatusIndicator;
    
    [Header("Player Hud GAME OBJECT")]
    [SerializeField] private GameObject playerHudObject;
   
    [HideInInspector]
    private bool autoReload;

    private Vector3 moveDir;
    public GameObject dashEffect;
    public Transform dashEffectPosition;
    
    [Header("Dash Slider and Options")]
    public int dashSpeed;
    private bool isDashing;
    [SerializeField] private Slider dashSlider;
    public int dashingSize = 50, maxDashingSize = 50;
    [SerializeField] private int dashIncreaseValue;
    [SerializeField] private int dashNumberOfFlashes;
    [SerializeField] private float dashFlashDuration;

    ChromaticAberration chromatic;
    Vignette vignette;
    FilmGrain filmGrain;
    AudioManager audioM;
    GameMaster gm;

    #endregion

    #region UnityAwakeStartUpdate
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        glitch = GetComponent<GlitchEffect>();
        audioM = FindObjectOfType<AudioManager>();
        boxC = GetComponent<BoxCollider2D>();
        gm = FindObjectOfType<GameMaster>();
        autoReload = true;
    }

    void Start()
    {
        bulletSlider.maxValue = maxShootingSize;
        dashSlider.maxValue = maxDashingSize;
        shootingSize = maxShootingSize;
        dashingSize = maxDashingSize;
        SetBulletSlider();
        SetDashSlider();
        SetUpPlayerBorder();
        initialBarColor = new Color(0.06674972f, 0.7675536f, 0.9433962f);
        initialBarColorCopy = initialBarColor;
        initialSliderColor = Fill.color;

        playerStats.Init();

        if(myStatusIndicator != null)
        {
            myStatusIndicator.SetHealth(playerStats.CurrHealth, playerStats.maxHealth);
        }

        Volume volume = GameObject.FindObjectOfType<Volume>();
        ChromaticAberration tmpC;
        if(volume.profile.TryGet<ChromaticAberration>(out tmpC))
        {
            chromatic = tmpC;
        }
        Vignette tmpV;
        if(volume.profile.TryGet<Vignette>(out tmpV))
        {
            vignette = tmpV;
        }
        FilmGrain tmpFG;
        if(volume.profile.TryGet<FilmGrain>(out tmpFG))
        {
            filmGrain = tmpFG;
        }

    }

    //inainte de schimbari  
    private void FixedUpdate()
    {
        SliderAndPlayerCooldown();
    }

    void Update()
    {
        Reload();
        MovePlayer();
        FaceMouse();
        
        if(dashingSize==maxDashingSize)
        {
            DashMove();
        }

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        StartCoroutine(PlayBarFullSound());        

        if(playerStats.currentHealth<=30)
        {
            playerHudObject.GetComponent<Animator>().SetBool("below30", true);
        }     
        else
        {
            playerHudObject.GetComponent<Animator>().SetBool("below30", false);
        }

        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            GetComponent<Animator>().SetBool("Attack", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (rb != null && c.gameObject.tag == "Enemy")
        {
            StartCoroutine(FlashCo(numberOfFlashes, flashDuration));
            StartCoroutine(IncreaseChromAberration());
            DamagePlayer(onEnemyHitDamage);
            vignette.intensity.value+=0.1f;
            filmGrain.intensity.value+=0.1f;
            Destroy(c.gameObject);
            audioM.Play("Hurt");
        }

        if(rb != null && c.gameObject.tag == "EnemyProjectile")
        {
            StartCoroutine(FlashCo(numberOfFlashes, flashDuration));
            StartCoroutine(IncreaseChromAberration());
            DamagePlayer(enemyProjectileDamage);
            vignette.intensity.value+=0.05f;
            filmGrain.intensity.value+=0.5f;
            Destroy(c.gameObject);
            audioM.Play("Hurt");
        }
        
        if(rb!=null && (c.gameObject.tag == "Disasters" || c.gameObject.tag == "Rockets"))
        {
            StartCoroutine(FlashCo(numberOfFlashes, flashDuration));
            StartCoroutine(IncreaseChromAberration());
            DamagePlayer(disastersDamage);
            vignette.intensity.value+=0.05f;
            filmGrain.intensity.value+=0.5f;
            audioM.Play("Hurt");
        }

        if(rb!=null && c.gameObject.tag == "RockPiece")
        {
            DamagePlayer(rockPieceDamage);
            audioM.Play("Hurt");
        }

        if(myStatusIndicator != null)
        {
            myStatusIndicator.SetHealth(playerStats.CurrHealth, playerStats.maxHealth);
        }   
        
    }

    #endregion

    #region TakingDamageAndShooting
    public void DamagePlayer(int damage)
    {
        playerStats.CurrHealth -= damage;
        if(playerStats.CurrHealth <= 0)
        {
            GameMaster.KillPlayer(this);
        }
    }

    public void Shoot()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject laser = Instantiate(laserPrefab1, firePoint1.position, firePoint1.rotation) as GameObject;
        GameObject laser2 = Instantiate(laserPrefab2, firePoint2.position, firePoint2.rotation) as GameObject;

        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        Rigidbody2D rb2 = laser2.GetComponent<Rigidbody2D>();

        FindObjectOfType<AudioManager>().Play("ShootSound");
               
        rb.AddForce(firePoint1.up * projectileSpeed, ForceMode2D.Impulse);
        rb2.AddForce(firePoint2.up * projectileSpeed, ForceMode2D.Impulse);

        GetComponent<Animator>().SetBool("Attack",true);

        RemoveValue(decreaseValue);
        SetBulletSlider();
    }

    #endregion

    #region MovementAndShootingSlider
   
    private void SetUpPlayerBorder()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;

    }

    private void MovePlayer()
    {
        if (Time.timeScale != 0)
        {
            var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeedX;
            var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeedY;
            var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
            var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
            moveDir = new Vector2(newXPos, newYPos);
            transform.position = moveDir;
        }
    }

    private void DashMove()
    {
        Quaternion rotation = transform.rotation;
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {           
            //Quaternion rotation = transform.rotation;
            float distIUse = Vector3.Distance(transform.position, mousePos);          
            if(distIUse <=4)
            {
                transform.position = mousePos;
                StartCoroutine(FlashCo(dashNumberOfFlashes, dashFlashDuration));
            }
            else if(distIUse > 4)
            {
                transform.position = Vector3.MoveTowards(transform.position, mousePos, dashSpeed);
                StartCoroutine(FlashCo(dashNumberOfFlashes,dashFlashDuration));
            }
            GameObject dashEff = Instantiate(dashEffect, dashEffectPosition.transform.position, Quaternion.identity);
            Transform tr = dashEff.GetComponent<Transform>();
            //rotation.z += 90;
            tr.up = transform.right;
            dashingSize = 0;
            SetDashSlider();
        }
    }

    private void Reload()
    {
        if(Input.GetKeyDown(KeyCode.R) && autoReload)
        {
            IncreaseValue(barIncreaseValue);
            Reloading = true;
            autoReload = false;
        }

        if (shootingSize == maxShootingSize)
        {
            autoReload = true;
        }      
    }
    //Functie pentru shooting cu delay de 0.2 secunde

    private void SliderAndPlayerCooldown()
    {
        if (CanShootFrameTimer >= CanShootFrameTimerMax)
        {
            if (Input.GetKey(KeyCode.Mouse0) && !Reloading)
            {
                Shoot();
                CanShootFrameTimer = 0;
            }
        }

        if (shootingSize <= 30)
        {
            bulletSlider.GetComponent<Animator>().SetBool("IsBelow30perCent", true);
        }
        else
        {
            Fill.color = initialSliderColor;
            bulletSlider.GetComponent<Animator>().SetBool("IsBelow30perCent", false);
        }

        CanShootFrameTimer++;

        if (shootingSize <= 0)
        {
            Reloading = true;
            empty1 = true;
            StartCoroutine(PlayerBarEmpty());         
        }

        if (Reloading)
        {
            if (ReloadingTimer >= MaxReloadingTimer)
            {
                IncreaseValue(barIncreaseValue);

                if (shootingSize == maxShootingSize)
                {
                    Reloading = false;
                }

                ReloadingTimer = 0;
                phase1 = true;
            }

            ReloadingTimer++;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                audioM.Play("OutOfAmmoSound");
            }
        }

        if(dashingSize != maxDashingSize)
        {
            if(DashingTimer >= MaxDashingTimer)
            {
                IncreaseDashSliderValue(dashIncreaseValue);
                DashingTimer = 0;
            }
        }
        DashingTimer++;
    }

    public void SetBulletSlider()
    {
        bulletSlider.value = shootingSize;

        if (bulletSlider.value == 0)
        {
            Fill.gameObject.SetActive(false);
        }
        else
        {
            Fill.gameObject.SetActive(true);
        }
    }

    public void SetDashSlider()
    {
        dashSlider.value = dashingSize;

        if(dashSlider.value == 0)
        {
            Fill.gameObject.SetActive(false);
        }
        else
        {
            Fill.gameObject.SetActive(true);
        }
    }

    public void IncreaseDashSliderValue(int value)
    {
        dashingSize += value;
        dashingSize = Mathf.Clamp(dashingSize, 0, maxDashingSize);
        SetDashSlider();
    }

    public void RemoveValue(int value)
    {
        shootingSize -= value;
        shootingSize = Mathf.Clamp(shootingSize, 0, maxShootingSize);
        SetBulletSlider();
    }

    public void IncreaseValue(int value)
    {
        shootingSize += value;
        shootingSize = Mathf.Clamp(shootingSize, 0, maxShootingSize);
        SetBulletSlider();
    }

    public void SetColor(Color color)
    {
        Fill.color = color;
    }

    private void FaceMouse()
    {
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90;
        rb.rotation = angle;
    }
    #endregion

    #region CoroutineSounds(ForShootingSlider)
    private IEnumerator PlayBarFullSound()
    {
        if (shootingSize == maxShootingSize && phase1)
        {
            FindObjectOfType<AudioManager>().Play("BarFullSound");
            yield return new WaitForSeconds(0f);
            phase1 = false;
        }
    }
  
    
    private IEnumerator PlayerBarEmpty()
    {        
        FindObjectOfType<AudioManager>().Play("BarEmptySound");
        yield return new WaitForSeconds(0f);
        empty1 = false;
    }
    
    #endregion

    #region Invincibility/GettingHitByAnEnemy

    private IEnumerator FlashCo(int numOfFlashes, float flshDuration)
    {
        int temp = 0;
        boxC.enabled = false;
        while (temp < numOfFlashes)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(flshDuration);
            sr.enabled = true;
            yield return new WaitForSeconds(flshDuration);
            temp++;
        }
        if (temp >= numOfFlashes)
            boxC.enabled = true;
    }

    #endregion 

    #region PostProcessing

    private IEnumerator IncreaseChromAberration()
    {
        float saturation = chromatic.intensity.value;
        if(saturation<0.29f)
        {
            saturation+=0.3f;
            chromatic.intensity.value=saturation;
            yield return new WaitForSeconds(0.1f);
        }
        else if(saturation>=0.29f)
        {
            saturation-=0.45f;
            chromatic.intensity.value=saturation;
            yield return new WaitForSeconds(0.1f);
        }
        chromatic.intensity.value=0;
    }

    #endregion
}
