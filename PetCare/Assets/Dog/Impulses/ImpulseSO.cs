using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Impulses
{
    public abstract class ImpulseSO : ScriptableObject
    {
        public abstract Activity.ActivitySO DecideActivity(Dog.Mind mind, object argument);
    }
}