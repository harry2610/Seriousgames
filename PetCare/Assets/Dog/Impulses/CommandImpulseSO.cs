using Dog;
using UnityEngine;

namespace Impulses
{
    [CreateAssetMenu(fileName = "CommandImpulse", menuName = "Dog/Impulses/Command")]
    public class CommandImpulseSO : ImpulseSO
    {
        public Activity.ActivitySO notUnderstoodActivity;
        public override Activity.ActivitySO DecideActivity(Dog.Mind mind, object argument)
        {
            if (argument is string command)
            {
                foreach (GameState.CommandState cmd in mind.dogState.commands)
                {
                    if (command.ToLower().Contains(cmd.phrases.ToLower()))
                    {
                        if (cmd.rehearsed >= Random.Range(0f, 100f))
                        {
                            cmd.rehearsed = Mathf.Min(100f, cmd.rehearsed + 10f);
                            return cmd.activity;
                        }
                        else
                        {
                            return notUnderstoodActivity;
                        }
                    }
                }
            }
            return notUnderstoodActivity;
        }
    }
}