using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bullet : MonoBehaviour
{
    public int damage = 40;
    public GameObject impactEffect;
    [SerializeField] private GameObject popUpDamageText;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Physics2D.IgnoreLayerCollision(5, 9);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        EnemyFollow enemy = hitInfo.GetComponent<EnemyFollow>();
        if(enemy!=null)
        {
            enemy.TakeDamage(damage);
            ShowPopUpDamageText(damage);
            FindObjectOfType<AudioManager>().Play("ImpactSound");
        }    
        if(hitInfo.gameObject.tag != "Player" && hitInfo.gameObject.tag != "Bullet")
        { 
            Destroy(gameObject,0.01f);
            damage = 0;
            sr.enabled = false;
        }
        if(hitInfo.gameObject.tag != "Bullet")
        {
            var impactEffectClone = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(impactEffectClone, 0.3f);
        }
    }
    private void ShowPopUpDamageText(int damage)
    {
        Quaternion rotation = popUpDamageText.transform.rotation;
        var popUp = Instantiate(popUpDamageText, transform.position + new Vector3(0,0.1f), rotation);
        popUp.GetComponentInChildren<TextMeshPro>().text = damage.ToString();
        Destroy(popUp, 1f);
    }

    private IEnumerator AnimatePopUpText()
    {
        popUpDamageText.transform.position += new Vector3(0, 0.05f, 0);
        yield return new WaitForSeconds(0.01f);
    }
}
