using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDInfo : MonoBehaviour
{

    public LeanTweenType easeType;
    [SerializeField] private float duration;
    Vector3 currentPos;
    [SerializeField] private bool goUp=true;
    //[SerializeField] private float timer;

    private void Awake() 
    {
        currentPos = gameObject.transform.position;
        goUp=true;
    }

    void Update()
    {

        //ResetTimer();
    
        if(Input.GetKeyDown(KeyCode.C) && goUp)
        {
            LeanTween.moveY(gameObject, -4.44f, duration).setEase(easeType);
            goUp = false;
        }
        
        if(Input.GetKeyUp(KeyCode.C))
        {
            LeanTween.moveY(gameObject, -7.2f, duration).setDelay(0.5f).setEase(easeType);
            StartCoroutine(DisableGoingUp());
        }

    }

    private IEnumerator DisableGoingUp()
    {
        yield return new WaitForSeconds(1.5f);
        goUp = true;
    }

}
