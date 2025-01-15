using UnityEngine;

public class ToothbrushTool : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 previousPosition;

    void Start()
    {
        // Find the main camera
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Make sure there is a camera tagged as 'MainCamera' in the scene.");
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartDragging(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        MoveObject(touch.position);
                    }
                    break;

                case TouchPhase.Ended:
                    StopDragging();
                    break;
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging(Input.mousePosition);
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            MoveObject(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }

    private void StartDragging(Vector2 inputPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                previousPosition = hit.point; // Store the initial position from the raycast hit
            }
        }
    }

    private void MoveObject(Vector2 inputPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 currentPosition = hit.point; // Get the current hit position
            Vector3 delta = currentPosition - previousPosition; // Calculate the movement delta

            // Ignore movement in the Z axis
            delta.z = 0;

            // Move the toothbrush based on the delta
            transform.position += delta;

            // Update the previous position while keeping the Z axis unchanged
            previousPosition = new Vector3(currentPosition.x, currentPosition.y, previousPosition.z);
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any surface during movement.");
        }
    }

    private void StopDragging()
    {
        isDragging = false;
    }
}
