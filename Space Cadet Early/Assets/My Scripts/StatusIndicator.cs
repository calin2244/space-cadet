using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIndicator : MonoBehaviour {

	[SerializeField]private RectTransform healthBarRect;
	[SerializeField] private TextMeshProUGUI healthText;



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
		healthText.text = current + " HP";
	}

}
