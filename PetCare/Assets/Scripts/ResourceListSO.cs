using Activity;
using Dog;
using Impulses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ResourceList", menuName = "ScriptableObjects/Resource List")]
public class ResourceListSO : ScriptableObject
{
    public BreedSO[] breeds;
    public ItemSO[] items;
    public CharacterTraitSO[] characterTraits;
    public ActivitySO[] activities;
    public ConditionSO[] conditions;
    public AdoptableDogSO[] adoptableDogs;
    public StartGameSO[] startGames;
    public ImpulseSO[] impulses;
    public QuicktipSO[] quicktips;
}
