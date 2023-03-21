using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    GameObject canvas;

    GameObject startParent;
    Vector2 startPos;
    GameObject dropZone;
    bool isOverDropZone;

    bool isDragging = false;

    void Start()
    {
        canvas = GameObject.Find("MainCanvas");
    }

    void Update()
    {
        if(isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(canvas.transform, true);
        }
    }

    public void StartDrag()
    {
        isDragging = true;

        startParent = transform.parent.gameObject;
        startPos = transform.position;
    }

    public void StopDrag()
    {
        isDragging = false;

        if(isOverDropZone)
        {
            transform.SetParent(dropZone.transform, false);
        }
        else
        {
            transform.position = startPos;
            transform.SetParent(startParent.transform, false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
    }
}
