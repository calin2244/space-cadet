using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;
using UnityEngine.U2D;
public class ChangePostOnRuntime : MonoBehaviour
{
    ChromaticAberration m_chromAber;
    LensDistortion m_lensDist;

    bool shiftIsHeld;
    bool cond1 = true;

    [Header ("Chromatic Aberration Animation")]
    [SerializeField] private float timeMod;
    [SerializeField] private float rangeMod;

    float saturation;
    void Start()
    {
        PostProcessVolume activeVolume = gameObject.GetComponent<PostProcessVolume>();
        activeVolume.profile.TryGetSettings(out m_chromAber);
        activeVolume.profile.TryGetSettings(out m_lensDist);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            shiftIsHeld = true;
            cond1 = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            shiftIsHeld = false;
            cond1 = false;
        }
           
        ChangeChromaticAberationOnSlowMo();
        //ChangeLensDistortionOnSlowMo();
    }    

    void ChangeChromaticAberationOnSlowMo()
    {
        if(shiftIsHeld)
        {
            m_chromAber.enabled.value = true;
            StartCoroutine(ChangeChromAberrSlowlyIncrease());
        }  
        else
        {
            //m_chromAber.enabled.value = false;
            StartCoroutine(ChangeChromAberrSlowlyDecrease());
        }
    }

    void ChangeLensDistortionOnSlowMo()
    {
        if(shiftIsHeld)
        {
            m_lensDist.enabled.value = true;
        }
        else
        {
            m_lensDist.enabled.value=false;
        }
    }

    private IEnumerator ChangeChromAberrSlowlyIncrease()
    {
        saturation = 0;
        if(saturation < 0.29 && shiftIsHeld)
        {
            saturation = saturation + 0.03f;
            // sleepEffect
            m_chromAber.intensity.value = saturation;
            yield return new WaitForSeconds(0.01f);
        }
        m_chromAber.intensity.value = 0.29f;
    }

    private IEnumerator ChangeChromAberrSlowlyDecrease()
    {
        saturation = m_chromAber.intensity.value;
        if(saturation > 0 && !shiftIsHeld)
        {
            saturation = saturation - 0.03f;
            // sleepEffect
            m_chromAber.intensity.value = saturation;
            yield return new WaitForSeconds(0.1f);
        }
        m_chromAber.enabled.value = false;
    }
}
