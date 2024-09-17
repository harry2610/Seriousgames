using UnityEngine;
using UnityEngine.AI;

enum PlacementState
{
    InHand,
    SetDown,
    Placed,
}
public class Placeable : MonoBehaviour
{
    // Distance between placeable and camera
    [Range(0, 5)]
    public float cameraDistance;
    // Objects Rigid body
    private Rigidbody m_RigidBody;
    // Current state of placement
    private PlacementState m_PlacementState;
    // Memorize last position before touch stopped
    private bool m_OnFloor;

    public bool IsPlaced { get => m_PlacementState == PlacementState.Placed; }
    
    // Start is called before the first frame update
    void Start()
    {
        Transform cameraTransform = Camera.main.transform;
        m_PlacementState = PlacementState.InHand;
        m_RigidBody = this.GetComponent<Rigidbody>();
        transform.position = cameraTransform.position + (cameraDistance * cameraTransform.forward);
        m_OnFloor = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (m_PlacementState)
        {
            case PlacementState.Placed:
                enabled = false;
                return;
            case PlacementState.InHand:
                ItemInHand();
                break;
            case PlacementState.SetDown:
                PlaceItem();
                break;
        }
    }

    void ItemInHand()
    {
        Transform cameraTransform = Camera.main.transform;
        transform.position = cameraTransform.position + (cameraDistance * cameraTransform.forward);
        if (Input.touchCount <= 0) return;
        Ray hitRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        // Check if item was touched
        if (Physics.Raycast(hitRay, out RaycastHit raycastHit, 100) && raycastHit.rigidbody)
        {
            if (raycastHit.rigidbody.Equals(m_RigidBody)) m_PlacementState = PlacementState.SetDown;
        };
    }
    void PlaceItem()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Ended)
        {
            // If item on floor after letting go, place permanently
            if (m_OnFloor)
            {
                //this.GetComponent<NavMeshObstacle>().enabled = true;
                
                if(TryGetComponent<ConditionContainer>(out ConditionContainer container))
                {
                    container.Apply();
                }
                
                
                
                m_PlacementState = PlacementState.Placed;
                m_OnFloor = false;
            }
            // If item not on the floor after letting go return it to hand
            else
            {
                m_PlacementState = PlacementState.InHand;
            }

            return;
        }
        Camera cameraMain = Camera.main;
        Vector2 touchPos = touch.position;
        Ray worldRay = cameraMain.ScreenPointToRay(touchPos);
        // Show item on floor position if on floor
        if (FloorHit(worldRay, out Vector3 floorHitPoint))
        {
            floorHitPoint.y += 0.005f;
            transform.position = floorHitPoint;
            m_OnFloor = true;
        }
        // Hover item if not on floor
        else
        {
            Vector3 newScreenPosition = touchPos;
            newScreenPosition.z = cameraDistance;
            transform.position = cameraMain.ScreenToWorldPoint(newScreenPosition);
            m_OnFloor = false;
        }
    }

    public static bool FloorHit(Ray worldRay, out Vector3 rayFloorHit)
    {
        // Prevent div by zero if ray parallel to ground
        if (worldRay.direction.y == 0)
        {
            rayFloorHit = default;
            return false;
        }
        
        float hitDistance = -worldRay.origin.y / worldRay.direction.y;
        // Ray floor intersect
        rayFloorHit = worldRay.origin + (worldRay.direction * hitDistance);
        NavMeshHit navHit = default;
        // Check if item on NavMesh
        return NavMesh.SamplePosition(rayFloorHit, hit: out navHit, 0.1f, 1);
    }
    
}
