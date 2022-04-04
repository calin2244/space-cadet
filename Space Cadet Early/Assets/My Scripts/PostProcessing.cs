using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;

public class PostProcessing : MonoBehaviour
{
    
    ChromaticAberration chromatic;
    Vignette vignette;
    private float saturation;
    [SerializeField] private bool shiftIsHeld;
    
    void Start()
    {
        Volume volume = gameObject.GetComponent<Volume>();
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

    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.LeftShift))
       {
           chromatic.intensity.value=0.27f;
           shiftIsHeld = true;
       }
       else if(Input.GetKeyUp(KeyCode.LeftShift))
       {
           StartCoroutine(ChangeChromAberrSlowlyDecrease());
           shiftIsHeld = false;
       }
       
    }
    
    private IEnumerator ChangeChromAberrSlowlyDecrease()
    {
        saturation = chromatic.intensity.value;
        if(saturation > 0)
        {
            saturation = 0.23f;
            chromatic.intensity.value = saturation;
            yield return new WaitForSeconds(0.02f);
            saturation = 0.19f;
            chromatic.intensity.value = saturation;
            yield return new WaitForSeconds(0.02f);
            saturation = 0.15f;
            chromatic.intensity.value = saturation;
            yield return new WaitForSeconds(0.02f);
            saturation = 0.11f;
            chromatic.intensity.value = saturation;
            yield return new WaitForSeconds(0.02f);
            saturation = 0.05f;
            chromatic.intensity.value = saturation;
            yield return new WaitForSeconds(0.02f);
            saturation = 0;
            chromatic.intensity.value = saturation;
            yield return new WaitForSeconds(0.02f);
        }
        chromatic.intensity.value = 0;
    }

   
}
