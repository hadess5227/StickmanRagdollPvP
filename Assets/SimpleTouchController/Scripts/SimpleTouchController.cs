using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RagdollCreatures;
using UnityEditor;
public class SimpleTouchController : MonoBehaviour
{

    // PUBLIC
    public RectTransform handle;
    public int controllerIndex = 0;
    public delegate void TouchDelegate(Vector2 value);
    public event TouchDelegate TouchEvent;

    public delegate void TouchStateDelegate(bool touchPresent);
    public event TouchStateDelegate TouchStateEvent;

    // PRIVATE
    [SerializeField]
    private RectTransform joystickArea;
    private bool touchPresent = false;
    public Vector2 movementVector;

    public Vector2 dir = Vector2.zero;
    Vector2 _dir = Vector2.zero;
    Vector2 dir0 = Vector2.zero;
    public Vector2 GetTouchPosition
    {
        get { return movementVector; }
    }

    public void BeginDrag()
    {
        touchPresent = true;
        if (TouchStateEvent != null)
            TouchStateEvent(touchPresent);
    }

    public void EndDrag()
    {
        touchPresent = false;
        movementVector = joystickArea.anchoredPosition = Vector2.zero;

        if (TouchStateEvent != null)
            TouchStateEvent(touchPresent);
    }

    public void OnPointerEnter()
    {
        if(GamePlay.Instance.localPlayer && GamePlay.Instance.localPlayer.interact && GamePlay.Instance.localPlayer.interact.currentInteractable)
            dir0 = GamePlay.Instance.localPlayer.interact.currentInteractable.transform.position;
    }
    public void OnPointerExit()
    {
        GamePlay.Instance.localPlayer.interact.Instance.dir = dir;
        if (controllerIndex == 1 && GamePlay.Instance.localPlayer && dir.magnitude > 0)
        {
            if (GamePlay.Instance.localPlayer.interact.currentInteractable.name.ToLower().Contains("grenade"))
            {
                GamePlay.Instance.localPlayer.interact.Instance.OnShot();
            }
        }
    }

    private void Update()
    {
        //OnShot();


        if (Input.GetMouseButton(0) && controllerIndex == 1)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(handle.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
            {
                OnShot();
            }
        }
    }

    public void OnValueChanged(Vector2 value)
    {
        if (touchPresent)
        {
            // convert the value between 1 0 to -1 +1
            movementVector.x = ((1 - value.x) - 0.5f) * 2f;
            movementVector.y = ((1 - value.y) - 0.5f) * 2f;

            if (TouchEvent != null)
            {
                TouchEvent(movementVector);
            }

            if (GamePlay.Instance && GamePlay.Instance.localPlayer)
            {
                _dir = GamePlay.Instance.localPlayer.interact.currentInteractable.transform.position;
                dir = _dir - dir0;
            }
        }

    }

    public void OnShot()
    {
        if (controllerIndex == 1 && GamePlay.Instance.localPlayer)
        {
            if (!GamePlay.Instance.localPlayer.interact.currentInteractable.name.ToLower().Contains("grenade"))
                GamePlay.Instance.localPlayer.interact.Instance.OnShot();
        }
    }
}
