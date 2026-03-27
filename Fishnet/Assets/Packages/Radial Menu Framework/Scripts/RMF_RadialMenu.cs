using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Radial Menu Framework/RMF Core Script")]
public class RMF_RadialMenu : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rt;

    [Tooltip("Adjusts the radial menu for use with a gamepad or joystick.")]
    public bool useGamepad = false;

    [Tooltip("With lazy selection, you only have to point in the direction of an element to select it.")]
    public bool useLazySelection = true;

    [Tooltip("If set to true, a pointer graphic will aim in the direction of your mouse.")]
    public bool useSelectionFollower = true;

    [Tooltip("The rect transform of the selection follower's container.")]
    public RectTransform selectionFollowerContainer;

    [Tooltip("Text object that displays labels of radial elements on hover.")]
    public Text textLabel;

    [Tooltip("List of radial menu elements, order-dependent.")]
    public List<RMF_RadialMenuElement> elements = new List<RMF_RadialMenuElement>();

    [Tooltip("Total angle offset for all elements.")]
    public float globalOffset = 0f;

    [HideInInspector]
    public float currentAngle = 0f;

    [HideInInspector]
    public int index = 0;

    private int elementCount;
    private float angleOffset;
    private int previousActiveIndex = 0;
    private PointerEventData pointer;

    private Vector2 joystickInput;
    private bool submitPressed;
    private bool clickPressed;

    void Awake()
    {
        pointer = new PointerEventData(EventSystem.current);

        rt = GetComponent<RectTransform>();

        if (rt == null)
            Debug.LogError("Radial Menu: Rect Transform for radial menu " + gameObject.name + " could not be found.");

        if (useSelectionFollower && selectionFollowerContainer == null)
            Debug.LogError("Radial Menu: Selection follower container is unassigned on " + gameObject.name);

        elementCount = elements.Count;
        angleOffset = (360f / (float)elementCount);

        for (int i = 0; i < elementCount; i++)
        {
            if (elements[i] == null)
            {
                Debug.LogError("Radial Menu: element " + i.ToString() + " is null!");
                continue;
            }
            elements[i].parentRM = this;
            elements[i].setAllAngles((angleOffset * i) + globalOffset, angleOffset);
            elements[i].assignedIndex = i;
        }
    }

    void Start()
    {
        if (useGamepad)
        {
            EventSystem.current.SetSelectedGameObject(gameObject, null);
            if (useSelectionFollower && selectionFollowerContainer != null)
                selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, -globalOffset);
        }
    }

    public void OnJoystick(InputValue value) => joystickInput = value.Get<Vector2>();
    public void OnSubmit(InputValue value) => submitPressed = value.isPressed;
    public void OnClick(InputValue value) => clickPressed = value.isPressed;

    void Update()
    {
        bool joystickMoved = joystickInput != Vector2.zero;

        float rawAngle;

        if (!useGamepad)
        {
            Vector2 mouse = Mouse.current.position.ReadValue();
            rawAngle = Mathf.Atan2(mouse.y - rt.position.y, mouse.x - rt.position.x) * Mathf.Rad2Deg;
        }
        else
            rawAngle = Mathf.Atan2(joystickInput.y, joystickInput.x) * Mathf.Rad2Deg;

        if (!useGamepad)
            currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));
        else if (joystickMoved)
            currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));

        if (angleOffset != 0 && useLazySelection)
        {
            index = (int)(currentAngle / angleOffset);

            if (elements[index] != null)
            {
                selectButton(index);

                if (clickPressed || submitPressed)
                {
                    elements[index].button.onClick.Invoke();
                    clickPressed = false;
                    submitPressed = false;
                }
            }
        }

        if (useSelectionFollower && selectionFollowerContainer != null)
        {
            if (!useGamepad || joystickMoved)
                selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270);
        }
    }

    public void SelectCurrent()
    {
        if (elements[index] != null)
        {
            Debug.Log("Selecting index: " + index);
            elements[index].button.onClick.Invoke();
        }
    }

    private void selectButton(int i)
    {
        if (elements[i].active == false)
        {
            elements[i].highlightThisElement(pointer);

            if (previousActiveIndex != i)
                elements[previousActiveIndex].unHighlightThisElement(pointer);
        }

        previousActiveIndex = i;
    }

    private float normalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0) angle += 360;
        return angle;
    }
}