using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommandList", menuName = "Dog/Training/Command List")]
public class CommandListSO : ScriptableObject
{
    public GameState.CommandState[] commands;
}