using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public float speed;
    public float scrollStrength;

    public LayerMask clickMask;

    private Vector2 _moveIn;
    private Vector2 _mousePosition;

    public Cinemachine.CinemachineVirtualCamera virtualCamera;

    private Camera _cam;
    private Pathfinding_AStar astar;
    private GridEntity controlledEntity;

    private bool _cameraLocked = false;

    // Start is called before the first frame update
    public void Init()
    {
        _cam = Camera.main;

        Controls controls = new Controls();
        controls.Enable();

        controls.Movement.Move.performed += _ => _moveIn = _.ReadValue<Vector2>();
        controls.Movement.Click.performed += Click;
        controls.Movement.RightClick.performed += RightClick;
        controls.Movement.MousePosition.performed += _ => _mousePosition = _.ReadValue<Vector2>();
        controls.Movement.CameraLock.performed += CameraLock;
        controls.Movement.Scroll.performed += ScrollInput;

        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;

        GameManager.Get().turnManager.OnTurnChange += UpdateControlledEntity;
        UpdateControlledEntity(0);
    }

    private void UpdateControlledEntity(int obj)
    {
        controlledEntity = GameManager.Get().turnManager.TurnEntity;

        if (!_cameraLocked) return;

        virtualCamera.Follow = controlledEntity.transform;
        virtualCamera.LookAt = controlledEntity.transform;
    }

    private void ScrollInput(InputAction.CallbackContext context)
    {
        return;

        float value = context.ReadValue<float>();

        float fov = virtualCamera.m_Lens.FieldOfView;

        if (value > .5f)
        {
            fov -= scrollStrength;
        }
        else
        {
            fov += scrollStrength;
        }

        fov = Mathf.Clamp(fov, 40, 100);

        virtualCamera.m_Lens.FieldOfView = fov;
    }

    private void CameraLock(InputAction.CallbackContext context)
    {
        _cameraLocked = !_cameraLocked;

        Transform target = (_cameraLocked) ? controlledEntity.transform : transform;

        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;
    }

    private void Update()
    {
        Vector3 move = new Vector3(_moveIn.x, 0, _moveIn.y);
        transform.Translate(move * speed * Time.unscaledDeltaTime);
    }

    #region Clicking
    private void Click(InputAction.CallbackContext ctx)
    {
        float val = ctx.ReadValue<float>();
        if (val < .5f) return;

        if (CastClickable(out IClickable clickable))
        {
            if (clickable.LeftClick())
            {
            }
        }

    }

    private void RightClick(InputAction.CallbackContext ctx)
    {
        float val = ctx.ReadValue<float>();
        if (val < .5f) return;

        if (CastClickable(out IClickable clickable))
        {
            if (clickable.RightClick())
            {

            }
        }
    }

    private bool CastClickable(out IClickable clickable)
    {
        clickable = null;

        Ray ray = _cam.ScreenPointToRay(_mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickMask))
        {
            clickable = hit.collider.gameObject.GetComponent<IClickable>();
            return (clickable != null);
        };

        return false;
    }
    #endregion
}