using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [SerializeField] EnemySpawner spawner;
    [SerializeField] Animator waveAnimator;
    [SerializeField] TextMeshProUGUI waveCountdownText;
    [SerializeField] TextMeshProUGUI waveCountText; 

    private EnemySpawner.SpawnState previousState;

    void Start()
    {
            if(spawner == null)
            {
                Debug.LogError("No wave spawner found on the Wave UI");
                this.enabled = false;
            }
            if(waveAnimator == null)
            {
                Debug.LogError("No wave waveAnimator found on the Wave UI");
                this.enabled = false;
            }
            if(waveCountdownText == null)
            {
                Debug.LogError("No wave waveCountdownText found on the Wave UI");
                this.enabled = false;
            }
            
            if(waveCountText == null)
            {
                Debug.LogError("No wave waveCountText found on the Wave UI");
                this.enabled = false;
            }
            
    }


  private void Update() 
  {
        switch(spawner.State)
        {
            case EnemySpawner.SpawnState.COUNTING:
                UpdateCountingUI();
                break;
            case EnemySpawner.SpawnState.SPAWNING:
                UpdateSpawningUI();
                break;
        } 

        previousState = spawner.State;   
  }

  void UpdateCountingUI()
  {
      if(previousState != EnemySpawner.SpawnState.COUNTING)
      { 
        waveAnimator.SetBool("WaveIncoming", false);
        waveAnimator.SetBool("WaveCountdown", true);
        //Debug.Log("counting");
      }
      waveCountdownText.text = ((int)spawner.WaveCountdown).ToString();
  }

   void UpdateSpawningUI()
  {
      if(previousState != EnemySpawner.SpawnState.SPAWNING)
      { 
        waveAnimator.SetBool("WaveIncoming", true);
        waveAnimator.SetBool("WaveCountdown", false);
        //Debug.Log("spawning");
      }

      waveCountText.text = spawner.NextWave.ToString();
  }
}
