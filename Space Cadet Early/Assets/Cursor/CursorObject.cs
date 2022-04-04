using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorObject : MonoBehaviour {

    [SerializeField] private CursorManager.CursorType cursorType;

    private void OnMouseEnter() {
        CursorManager.Instance.SetActiveCursorType(cursorType);
    }

    private void OnMouseExit() 
    {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Normal);
    }

}
