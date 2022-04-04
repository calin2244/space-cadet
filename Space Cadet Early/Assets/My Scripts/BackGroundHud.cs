using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundHud : MonoBehaviour
{
    
    private SpriteRenderer sr;
    Color backgroundColorAlpha;
    Color normalBackgroundColor;
    private void Awake() 
    {
        sr = GetComponent<SpriteRenderer>();
        backgroundColorAlpha = new Color(0.4117647f, 0.2980392f, 0.5176471f, 0.5f);
        normalBackgroundColor = new Color(0.4117647f, 0.2980392f, 0.5176471f, 1f);
    }
    
    private void OnTriggerEnter2D(Collider2D c) 
    {
        if(c.gameObject.tag == "Enemy" || c.gameObject.tag =="Disasters" || c.gameObject.tag == "Rockets")
        {
            sr.color = backgroundColorAlpha;
        }
        else
        {
            sr.color = normalBackgroundColor;
        }
    }

    private void OnTriggerExit2D(Collider2D c) 
    {
        sr.color = normalBackgroundColor;
    }
}
