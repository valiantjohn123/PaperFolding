using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

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

    private Vector3 initMaskPos;
    private Vector3 initPos;
    private RectTransform mainRect;
    private RectTransform frontPageRect;
    private Transform prevMainObject;
    private Transform prevDuplicateObject;
    private Image mainPaperImage;
    private Image frontPageImage;

    /// <summary>
    /// Mono awake
    /// </summary>
    private void Awake()
    {
        mainRect = GetComponent<RectTransform>();
        frontPageRect = frontPage.GetComponent<RectTransform>();

        mainPaperImage = GetComponent<Image>();
        frontPageImage = frontPage.GetChild(0).GetComponent<Image>();

        input.TouchChanged += OnTouchChanged;
    }

    /// <summary>
    /// Mono start
    /// </summary>
    private void Start()
    {
        initMaskPos = mask.position;
        initPos = transform.position;
    }

    /// <summary>
    /// Touch input trigger
    /// </summary>
    /// <param name="data"></param>
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
        }

        if (data.Type == InputHandler.InputType.Up)
        {
            SetFold();
        }
    }

    /// <summary>
    /// Set paper Fold 
    /// </summary>
    private void SetFold()
    {
        ResetRectZPosition(mainRect);
        var newObj = Instantiate(transform.parent, transform);
        ResetRectZPosition(newObj.GetComponent<RectTransform>());
        Destroy(newObj.GetChild(0).GetComponent<PaperController>());
        Destroy(newObj.GetChild(0).GetComponent<InputHandler>());
        newObj.SetAsFirstSibling();

        mainPaperImage.enabled = false;
        frontPageImage.enabled = false;

        mask.position = initMaskPos;
        mask.rotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;
        SetPivot(mainRect, Vector2.zero);
        SetPivot(frontPageRect, new Vector2(0, 1));

        transform.position = initPos;
        ResetRectZPosition(frontPageRect);

        frontPage.gameObject.SetActive(false);
        ResetRect(frontPageRect);

        var fChild = frontPage.GetChild(0).transform;
        fChild.rotation = Quaternion.identity;
        var newFObject = Instantiate(newObj, fChild);
        fChild.eulerAngles = new Vector3(180, 0, 0);

        if (prevDuplicateObject != null)
        {
            Destroy(prevDuplicateObject.gameObject);
        }
        if (prevMainObject != null)
        {
            Destroy(prevMainObject.gameObject);
        }

        prevMainObject = newObj;
        prevDuplicateObject = newFObject;
    }

    /// <summary>
    /// Set pivot of rect transform
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="pivot"></param>
    private void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    /// <summary>
    /// Reset rect Z position of rect
    /// </summary>
    /// <param name="rect"></param>
    private void ResetRectZPosition(RectTransform rect)
    {
        var pos3D = rect.anchoredPosition3D;
        pos3D.z = 0;
        rect.anchoredPosition3D = pos3D;
    }

    /// <summary>
    /// Reset rect
    /// </summary>
    /// <param name="rect"></param>
    private void ResetRect(RectTransform rect)
    {
        rect.pivot = Vector2.one / 2f;
        rect.anchoredPosition3D = Vector3.zero;
        rect.rotation = Quaternion.identity;
    }
}
