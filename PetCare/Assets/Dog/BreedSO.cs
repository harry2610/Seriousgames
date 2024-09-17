using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dog
{
    [CreateAssetMenu(fileName = "Breed", menuName = "Dog/Breed")]
    public class BreedSO : ScriptableObject
    {
        [ScriptableObjectIdAttribute]
        public long id;
        [Header("Fur")]
        public Material material;
    }
}