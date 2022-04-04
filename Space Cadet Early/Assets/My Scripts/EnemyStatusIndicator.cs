using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusIndicator : MonoBehaviour
{
    [SerializeField]private RectTransform healthBarRect;

    void Start()
	{
		if (healthBarRect == null)
		{
			Debug.LogError("STATUS INDICATOR: No health bar object referenced!");
		}		
	}

	public void SetHealth(int current, int maximum)
	{
		float _value = (float)current / maximum;

		healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);	
    }
}
