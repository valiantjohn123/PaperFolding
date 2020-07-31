using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class PaperController : MonoBehaviour
{
    [SerializeField]
    private Transform frontPage;
    [SerializeField]
    private Transform mask;
    [SerializeField]
    private InputHandler input;
    [SerializeField]
    private Transform mainParent;

    [SerializeField]
    private bool duplicate;

    private Vector3 initMaskPos;
    private Vector3 initPos;
    private RectTransform mainRect;
    private RectTransform frontPageRect;

    private void Awake()
    {
        initMaskPos = mask.position;
        initPos = transform.position;
        mainRect = GetComponent<RectTransform>();
        frontPageRect = frontPage.GetComponent<RectTransform>();
        input.TouchChanged += OnTouchChanged;
    }

    private void OnTouchChanged(InputHandler.TouchData data)
    {
        if (data.Type == InputHandler.InputType.Down)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mainRect, Input.mousePosition, data.Camera, out Vector2 screenPos);

            screenPos.x /= mainRect.sizeDelta.x;
            screenPos.y /= mainRect.sizeDelta.y;

            SetPivot(mainRect, screenPos);
            SetPivot(frontPageRect, new Vector2(screenPos.x, 1 - screenPos.y));
        }

        if (data.Type == InputHandler.InputType.Moved)
        {
            var posi = data.CurrentPos;
            posi.z = 0;
            frontPage.position = posi;

            Vector3 pos = frontPage.localPosition;
            float theta = Mathf.Atan2(pos.y, pos.x) * 180.0f / Mathf.PI;

            float deg = -(90.0f - theta) * 2.0f;
            frontPage.eulerAngles = new Vector3(0.0f, 0.0f, deg);

            transform.SetParent(mainParent);
            mask.position = (transform.position + frontPage.position) * 0.5f;
            mask.eulerAngles = new Vector3(0.0f, 0.0f, deg * 0.5f);
            transform.SetParent(mask);

            frontPage.gameObject.SetActive(true);
            ResetRectZPosition(frontPageRect);
        }

        if (data.Type == InputHandler.InputType.Up)
        {
            if (duplicate)
            {
                var newObj = Instantiate(transform.parent, transform);
                ResetRectZPosition(newObj.GetComponent<RectTransform>());
                Destroy(newObj.GetChild(0).GetComponent<PaperController>());
                Destroy(newObj.GetChild(0).GetComponent<InputHandler>());
            }

            mask.position = initMaskPos;
            mask.rotation = Quaternion.identity;
            transform.rotation = Quaternion.identity;
            SetPivot(mainRect, Vector2.zero);
            SetPivot(frontPageRect, new Vector2(0, 1));
            transform.position = initPos;
        }
    }

    private void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    private void ResetRectZPosition(RectTransform rect)
    {
        var pos3D = rect.anchoredPosition3D;
        pos3D.z = 0;
        rect.anchoredPosition3D = pos3D;
    }
}
