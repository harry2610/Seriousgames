using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dog
{
    [CreateAssetMenu(fileName = "AdoptableDog", menuName = "Dog/Adoptable Dog")]
    public class AdoptableDogSO : ScriptableObject
    {
        public string givenName;
        public Dog.BreedSO breed;
        [TextArea]
        public string description;
        public Dog.Habit[] habits;
        public CommandListSO commands;
    }
}
