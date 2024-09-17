using Geometry;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;
using UnityEngine.Events;

[System.Serializable]
public enum DogPose : int
{
    Stand = 0,
    Sit = 1,
    Place = 2,
    GivePaw = 3,
    Playful = 4,
    Pee = 5,
    Consume = 6,
}

public enum DogAnimation
{
    Bark,
    Questioning,
    Destroy,
    Vomit,
    Growl,
    Eat,
    Drink,
    Sniff,
    Howl
}

public class Body : MonoBehaviour
{
    public BodySettings settings;
    public Snout snout;
    public Head head;
    public Trunk trunk;
    public Legs legs;
    public Tail tail;
    public Breath breathing;
    public WalkableMesh walkableMesh;
    public Material m_DrawFurMaterial;
    private SkinnedMeshRenderer m_Mesh;
    private Walk m_Walk;
    private Animator m_Animator;
    private UnityAction m_OnPoseChangeFinished;
    private UnityAction m_OnAnimationFinished;
    private RigBuilder m_RigBuilder;
    private AudioSource m_AudioSource;
    private RenderTexture m_FurConditionTexture;
    private Material m_FurMaterial;
    private int m_LastSound = 0;
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
    private static readonly int PoseParameter = Animator.StringToHash("Pose");

    void Update()
    {
        transform.position = Vector3.Lerp(legs.rearStand.position, legs.frontStand.position, 0.5f);
        transform.rotation = Quaternion.Slerp(legs.rearStand.rotation, legs.frontStand.rotation, 0.5f);
        breathing.Update();
        head.Update();
        legs.Update();
        trunk.Update(head, legs, breathing);
        tail.Update();
        bool isInTargetPose = false;
        //m_RigBuilder.enabled = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Stand");
        m_Animator.SetFloat(MovementSpeed, m_Walk.move.speed);
        switch ((DogPose)m_Animator.GetInteger(PoseParameter))
        {
            case DogPose.Stand:
                isInTargetPose = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Stand");
                break;
            case DogPose.Sit:
                isInTargetPose = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Sit");
                break;
            case DogPose.Place:
                isInTargetPose = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Place");
                break;
            case DogPose.Consume:
                isInTargetPose = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Consume");
                break;
            case DogPose.Playful:
                isInTargetPose = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Playful");
                break;
            case DogPose.Pee:
                isInTargetPose = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("HoldPee");
                break;
            case DogPose.GivePaw:
                isInTargetPose = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("GivePaw");
                break;

        }

        if (m_OnPoseChangeFinished != null && isInTargetPose)
        {
            var drainedEvent = m_OnPoseChangeFinished;
            m_OnPoseChangeFinished = null;
            drainedEvent.Invoke();
        }
    }
    void Awake()
    {
        m_Animator = transform.GetComponent<Animator>();
        m_AudioSource = transform.GetComponent<AudioSource>();
        snout.Start();
        head.Start();
        trunk.Start(settings);
        legs.Start(transform, trunk);
        tail.Start(m_Animator);
        breathing.Start(m_Animator);
        m_Walk = GetComponent<Walk>();
        m_RigBuilder = GetComponent<RigBuilder>();
        m_RigBuilder.enabled = false;
        Assert.IsNotNull(m_Walk, "Body needs a Walk behaviour");
        m_Walk.Init(settings, legs, trunk);
        m_OnPoseChangeFinished = null;
        walkableMesh = Geometry.WalkableMesh.FromNavMesh(NavMesh.CalculateTriangulation());
    }
    private void Start()
    {
    }
    public bool Walk(WalkInfo info)
    {
        Assert.IsNotNull(info.onDestinationReached, "onDestinationReached must be defined for WalkInfo");
        if ((DogPose)m_Animator.GetInteger(PoseParameter) != DogPose.Stand)
            ChangeBodyPose(DogPose.Stand, () => m_Walk.StartWalk(info));
        else
            m_Walk.StartWalk(info);
        return true;
    }
    public void LookAt(Vector3 position)
    {
        head.LookAt(position);
    }
    public void ChangeBodyPose(DogPose dogPose, UnityAction onFinish)
    {
        UnityAction changeAction = () =>
        {
            m_OnPoseChangeFinished = onFinish;
            m_Animator.SetInteger(PoseParameter, (int)dogPose);
        };
        if (m_Walk.IsWalking())
            m_Walk.Abort(changeAction);
        else
            changeAction.Invoke();
    }
    public void Grab(GameObject grabbable, UnityAction onGrab)
    {
        ChangeBodyPose(DogPose.Consume, () =>
        {

            head.Grab(grabbable);
            ChangeBodyPose(DogPose.Stand, () =>
            {
                onGrab.Invoke();
            });
        });
    }
    public void StartAnimation(DogAnimation animationType, UnityAction onFinish)
    {
        UnityAction invokedAnimation = () =>
        {
            m_OnAnimationFinished = onFinish;
            m_Animator.SetTrigger(animationType.ToString());
        };
        switch (animationType)
        {
            case DogAnimation.Destroy:
                ChangeBodyPose(DogPose.Playful, () => { invokedAnimation.Invoke(); });
                break;
            case DogAnimation.Vomit:
                ChangeBodyPose(DogPose.Stand, () => { invokedAnimation.Invoke(); });
                
                break;
            default:
                invokedAnimation.Invoke();
                break;
        }
    }

    private void AnimationEndEvent()
    {
        var drainedEvent = m_OnAnimationFinished;
        if (drainedEvent != null)
        {
            m_OnAnimationFinished = null;
            drainedEvent.Invoke();
        }
    }

    private void SimpleSound(int audioType)
    {
        switch (audioType)
        {
            case 0:
                m_AudioSource.clip = settings.barkingSounds[Random.Range(0, settings.barkingSounds.Length)];
                break;
            case 1:
                m_AudioSource.clip = settings.howlingSounds[Random.Range(0, settings.howlingSounds.Length)];
                break;
            case 2:
                m_AudioSource.clip = settings.growlingSounds[Random.Range(0, settings.growlingSounds.Length)];
                break;
            case 3:
                m_AudioSource.clip = settings.sniffingSounds[Random.Range(0, settings.sniffingSounds.Length)];
                break;
            default:
                return;
        }
        
        m_AudioSource.Play();
    }
    
    private void NonRepeatingSound(int audioType)
    {
        AudioClip[] clips = audioType == 0 ? settings.drinkingSounds : settings.eatingSounds;
        int soundIndex = Random.Range(0, clips.Length);
        // Prevent sound duplication
        if (soundIndex == m_LastSound)
        {
            soundIndex -= 1;
            soundIndex = soundIndex < 0 ? clips.Length - 1 : soundIndex;
        }

        m_LastSound = soundIndex;
        Debug.Log("Sound " + soundIndex);
        m_AudioSource.clip = clips[Random.Range(0, soundIndex)];
        m_AudioSource.Play();
    }
}


