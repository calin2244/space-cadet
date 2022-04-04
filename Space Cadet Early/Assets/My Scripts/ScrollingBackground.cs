using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private float backgroundScrollSpeed = 0.5f;
    private Material myMaterial;
    Vector2 offset;

    void Awake()
    {
        myMaterial = GetComponent<Renderer>().material;
        offset = new Vector2(backgroundScrollSpeed, 0f);
    }

    void Update()
    {
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
    }
}
