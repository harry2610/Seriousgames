using UnityEngine;

public class ParkSetup : MonoBehaviour
{
    public StaticCam staticCam;
    public ParkEntryPositionSO[] possibleEntryPositions;
    private ParkEntryPositionSO m_CurrentEntryPosition;
    private GameObject m_Dog;
    void Awake()
    {
        GlobalEvents.Instance.onDogSpawned.AddListener(InitParkObjects);
        m_CurrentEntryPosition = GetRandomEntryPosition();
        FindObjectOfType<DogSpawner>().transform.position = m_CurrentEntryPosition.dogPos;
    }

    void InitParkObjects(GameObject obj)
    {
        m_Dog = obj;
        staticCam.SetInitialCameraPosition(m_CurrentEntryPosition.camPos);

    }

    private ParkEntryPositionSO GetRandomEntryPosition() 
    {
        int index = Random.Range(0, possibleEntryPositions.Length);
        return possibleEntryPositions[index];
    }

}
