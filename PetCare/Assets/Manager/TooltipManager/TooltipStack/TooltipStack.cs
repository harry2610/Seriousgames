using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipStack : MonoBehaviour
{
    public GameObject ui;
    private UIControllerTips uiController;

    private List<TooltipSO> m_TooltipQueue = new List<TooltipSO>();

    public List<TooltipSO> tooltipQueue
    {
        get => m_TooltipQueue;
    }

    public TooltipSO empty;
    private int m_CurrentlyShownTooltip;

    public int GetCurrentTooltipIndex()
    {
        return m_CurrentlyShownTooltip;
    }

    private void Awake()
    {
        uiController = ui.GetComponent<UIControllerTips>();
        m_CurrentlyShownTooltip = 0;
        UpdateTooltipText(false);
    }



    public void AddTooltip(TooltipSO tooltip)
    {
        if (m_TooltipQueue.Contains(tooltip))
        {
            Debug.Log("Tooltip: " + tooltip.text + " - Already exists in Tooltip queue. Please fix Tooltip deletion.");
            RemoveTooltip(tooltip);
        }

        m_TooltipQueue.Insert(0, tooltip);
        UpdateTooltipText(true);
    }

    public void RemoveTooltip(TooltipSO tooltip)
    {
        m_TooltipQueue.Remove(tooltip);
        m_CurrentlyShownTooltip = 0;
        UpdateTooltipText(false);
    }

    private void UpdateTooltipText(bool withBlink = false)
    {
        if (m_TooltipQueue.Count != 0)
        {
            TooltipSO tooltip = m_TooltipQueue[m_CurrentlyShownTooltip];
            uiController.SetTooltip(tooltip.heading, tooltip.text);
            if (withBlink)
            {
                uiController.StartInfoButtonBlink();
            }
        }
        else
        {
            uiController.SetTooltip(empty.heading, empty.text);
        }
    }

    public void ShowNextTooltip()
    {
        m_CurrentlyShownTooltip++;
        if (m_CurrentlyShownTooltip > m_TooltipQueue.Count - 1)
        {
            m_CurrentlyShownTooltip = 0;
        }
        UpdateTooltipText(false);
    }
    public void ShowPreviousTooltip()
    {
        m_CurrentlyShownTooltip--;
        if (m_CurrentlyShownTooltip < 0)
        {
            m_CurrentlyShownTooltip = m_TooltipQueue.Count - 1;
        }
        UpdateTooltipText(false);
    }
}
