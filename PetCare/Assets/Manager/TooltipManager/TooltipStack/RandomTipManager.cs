using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomTipManager : MonoBehaviour
{
    public List<RandomTipSO> randomTips; // List of possible tips
    private RandomTipSO lastShownTip; // The last displayed tip
    private TooltipStack m_TooltipStack; // Reference to the TooltipStack
    public float tipDisplayInterval = 300f; // Time interval in seconds

    private void Start()
    {
        m_TooltipStack = FindObjectOfType<TooltipStack>();
        // Start the coroutine that shows a new tip every X seconds
        StartCoroutine(ShowRandomTipWithInterval());
    }

    private IEnumerator ShowRandomTipWithInterval()
    {
        while (true)
        {
            ShowRandomTip(); // Show a new random tip
            yield return new WaitForSeconds(tipDisplayInterval); // Wait for the interval
        }
    }

    public void ShowRandomTip()
    {
        if (randomTips.Count == 0)
        {
            Debug.LogWarning("No tips in the randomTips list.");
            return;
        }

        RandomTipSO tipToShow = GetRandomTip();
        if (tipToShow != null)
        {
            Debug.Log($"Showing tip: {tipToShow.heading}");
            if (lastShownTip != null)
            {
                m_TooltipStack.RemoveTooltip(lastShownTip);
            }

            lastShownTip = tipToShow;
            m_TooltipStack.AddTooltip(tipToShow); // Display the new tip
        }
    }

    private RandomTipSO GetRandomTip()
    {
        List<RandomTipSO> availableTips = randomTips
            .Where(tip => tip.maxShowTimes > 0 && tip != lastShownTip)
            .ToList();

        if (availableTips.Count == 0)
        {
            Debug.LogWarning("No available tips to display.");
            return null;
        }

        int randomIndex = Random.Range(0, availableTips.Count);
        return availableTips[randomIndex];
    }
}
