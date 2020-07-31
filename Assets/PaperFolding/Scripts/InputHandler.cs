using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    #region Custom objects
    public enum InputType
    {
        Down, Moved, Up
    }

    [Serializable]
    public class TouchData
    {
        public InputType Type;
        public Vector3 InitPos;
        public Vector3 CurrentPos;
        public Camera Camera;
    }
    #endregion

    public Action<TouchData> TouchChanged;

    [SerializeField]
    private Camera currentCamera;

    [SerializeField]
    private TouchData currentTouchData;

    /// <summary>
    /// Mono awake
    /// </summary>
    private void Awake()
    {
        currentTouchData.Camera = currentCamera;
    }

    /// <summary>
    /// Mono update
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentTouchData.Type = InputType.Down;
            currentTouchData.InitPos = currentCamera.ScreenToWorldPoint(Input.mousePosition);
            currentTouchData.CurrentPos = currentCamera.ScreenToWorldPoint(Input.mousePosition);
            currentTouchData.InitPos.z = 0;
            currentTouchData.CurrentPos.z = 0;

            TouchChanged?.Invoke(currentTouchData);
        }

        if (Input.GetMouseButton(0))
        {
            currentTouchData.Type = InputType.Moved;
            currentTouchData.CurrentPos = currentCamera.ScreenToWorldPoint(Input.mousePosition);
            currentTouchData.CurrentPos.z = 0;
            TouchChanged?.Invoke(currentTouchData);
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentTouchData.Type = InputType.Up;
            currentTouchData.CurrentPos = currentCamera.ScreenToWorldPoint(Input.mousePosition);
            currentTouchData.CurrentPos.z = 0;
            TouchChanged?.Invoke(currentTouchData);
        }
    }
}
