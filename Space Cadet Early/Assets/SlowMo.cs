using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class SlowMo : MonoBehaviour
{
    private bool shiftIsHeld = false;
    [SerializeField] private Slider slowMoSlider;
    [SerializeField] private Image Fill;
    public float slowMoSize, maxSlowMoSize;
    private float slowMoTimer;
    private bool canUseSlowMo = true;
    private float reloadintTimer;
    private float canGoUpTimer;
    private bool shiftKeyIsUp;
    [Header("Change the way Slider works")]
    public float maxSlowMoTimer;
    public float maxReloadingTimer;
    public float canGoUpTimerMax;

    ChromaticAberration chromatic;

    private enum SlowM
    {
        Normal,
        SlowMotion
    };

    private SlowM state = SlowM.Normal;

    private SlowM State
    {
        get { return state; }
    }

    private void Awake()
    {
        slowMoSlider.maxValue = maxSlowMoSize;
        slowMoSize = maxSlowMoSize;
        SetSlider();

        Volume volume = FindObjectOfType<Volume>();

        ChromaticAberration tmpC;
        if (volume.profile.TryGet<ChromaticAberration>(out tmpC))
        {
            chromatic = tmpC;
        }

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            state = SlowM.SlowMotion;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            state = SlowM.Normal;
        }

        ControlChromAberration();

        if (state == SlowM.Normal)
        {
            shiftKeyIsUp = true;
        }
        else
        {
            shiftKeyIsUp = false;
        }

        if (!canUseSlowMo)
        {
            state = SlowM.Normal;
        }

        if (state == SlowM.SlowMotion)
        {
            Time.timeScale = 0.3f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void FixedUpdate()
    {
        SlowMoSlider();
    }

    #region Slider
    private void SlowMoSlider()
    {
        if (slowMoTimer >= maxSlowMoTimer)
        {
            if (Input.GetKey(KeyCode.LeftShift) && canUseSlowMo)
            {
                RmoveSliderValue(10);
                slowMoTimer = 0;
            }
        }
        slowMoTimer++;

        if (canGoUpTimer >= canGoUpTimerMax)
        {
            if (canUseSlowMo && shiftKeyIsUp)
            {
                IncreaseSliderValue(10);
                canGoUpTimer = 0;
            }
        }
        canGoUpTimer++;

        if (slowMoSize <= 0)
        {
            canUseSlowMo = false;
        }

        if (!canUseSlowMo)
        {

            state = SlowM.Normal;
            StartCoroutine(ChangeChromAberrSlowlyDecrease());
            if (reloadintTimer >= maxReloadingTimer)
            {
                IncreaseSliderValue(10);

                if (slowMoSize == maxSlowMoSize)
                {
                    canUseSlowMo = true;
                }
                reloadintTimer = 0;
            }
            reloadintTimer++;
        }
    }

    private void SetSlider()
    {
        slowMoSlider.value = slowMoSize;

        if (slowMoSlider.value == 0)
        {
            Fill.gameObject.SetActive(false); ;
        }
        else
        {
            Fill.gameObject.SetActive(true);
        }
    }

    private void IncreaseSliderValue(float value)
    {
        slowMoSize += value;
        slowMoSize = Mathf.Clamp(slowMoSize, 0, maxSlowMoSize);
        SetSlider();
    }

    private void RmoveSliderValue(float value)
    {
        slowMoSize -= value;
        slowMoSize = Mathf.Clamp(slowMoSize, 0, maxSlowMoSize);
        SetSlider();
    }
    #endregion

    #region PostProcessing
    private void ControlChromAberration()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canUseSlowMo)
        {
            StartCoroutine(IncreaseChromAberration());
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StartCoroutine(ChangeChromAberrSlowlyDecrease());
        }
    }

    private IEnumerator IncreaseChromAberration()
    {
        float saturation = chromatic.intensity.value;
        if (saturation <=0.25f && canUseSlowMo)
        {          
            saturation += 0.05f;
            chromatic.intensity.value = saturation;
            yield return null;
            saturation += 0.05f;
            chromatic.intensity.value = saturation;
            yield return null;
            saturation += 0.05f;
            chromatic.intensity.value = saturation;
            yield return null;
            saturation += 0.05f;
            chromatic.intensity.value = saturation;
            yield return null;
            saturation += 0.05f;
            chromatic.intensity.value = saturation;
            yield return null;
        }    
    }

    private IEnumerator ChangeChromAberrSlowlyDecrease()
    {
        float saturation = chromatic.intensity.value;
        if (saturation > 0)
        {
            saturation = 0.25f;
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
    #endregion
}
