using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PNCCursor : MonoBehaviour
{
    Camera mainCamera;
    RectTransform trans;
    Canvas canvas;
    CanvasScaler scaler;
    Image cursorImage;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        trans = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        scaler = GetComponentInParent<CanvasScaler>();
        cursorImage = GetComponentInChildren<Image>();
    }

    public Vector3 GetPosition() {
        return Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = Input.mousePosition;
        position.z = 0;
        position /= canvas.scaleFactor;
        position.x -= scaler.referenceResolution.x / 2;
        position.y -= scaler.referenceResolution.y / 2;
        position.y += cursorImage.rectTransform.sizeDelta.y;
        position.x += cursorImage.rectTransform.sizeDelta.x/2;
        position.y += cursorImage.rectTransform.sizeDelta.y/2;
        trans.anchoredPosition = position;
    }
}
