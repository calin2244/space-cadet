using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotRotateText : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(1, 1, 1);
    }
}
