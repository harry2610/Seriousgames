using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Activity
{
    [CreateAssetMenu(menuName = "Dog/Activity/PlayAnimation")]
    public class PlayAnimationSO : ActivitySO
    {

        [Header("Play Animation")] 
        public DogAnimation animation;
        public override ActivityExecution StartActivity(GameObject dog, object argument)
        {
            return new PlayAnimation(this, dog);
        }
    }

    [System.Serializable]
    public class PlayAnimation : ActivityExecution
    {
        public PlayAnimation(PlayAnimationSO info, GameObject dog) : base(info)
        {
            if (dog.TryGetComponent(out Body body))
            {
                body.StartAnimation(info.animation, () => {EndActivity(); });
            }
            
        }
        public override void Update()
        {
            
        }
    }
}
