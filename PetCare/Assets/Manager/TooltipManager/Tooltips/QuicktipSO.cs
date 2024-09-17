using UnityEngine;

[CreateAssetMenu]
public class QuicktipSO : TooltipSO
{
    [ScriptableObjectIdAttribute]
    public long id;
    public int maxShowTimes = 1;
}
