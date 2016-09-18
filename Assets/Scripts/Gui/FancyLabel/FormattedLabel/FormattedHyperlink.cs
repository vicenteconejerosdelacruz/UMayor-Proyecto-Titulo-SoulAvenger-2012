using UnityEngine;
using System.Collections;


public partial class FormattedLabel : IHyperlinkCallback
{
   /// <summary>
    /// Tooltips can be used to implement an OnMouseOver / OnMouseOut messaging system.
    /// http://unity3d.com/support/documentation/ScriptReference/GUI-tooltip.html
    /// </summary>
    private void handleHyperlink()
    {
        // Is the mouse over a hyperlink?  Yes if there's a tooltip to display
        if (Event.current.type == EventType.Repaint && GUI.tooltip != _lastTooltip)
        {
            // 1. Handle leaving the hyperlink
            if (_lastTooltip.StartsWith(HYPERLINK_TAG))
            {
                string hyperlinkId = _lastTooltip.Substring(HYPERLINK_TAG.Length);
                onHyperlinkLeave(hyperlinkId);
            }

            // 2. Handle entering the hyperlink
            if (GUI.tooltip.StartsWith(HYPERLINK_TAG))
            {
                string hyperlinkId = GUI.tooltip.Substring(HYPERLINK_TAG.Length);
                onHyperlinkEnter(hyperlinkId);
            }

            // 3. Store the current tooltip, even if not a hyperlink
            _lastTooltip = GUI.tooltip;
        }

        // 4. Handle a left mouse click on the hyperlink
        _activatedHyperlink = false;
        if (Event.current != null
            && Event.current.isMouse
            && Event.current.type == EventType.MouseUp
            && Event.current.button == 0 // Left button
            && isHyperlinkHovered())
        {
            //Debug.Log(_hoveredHyperlinkId);
            onHyperLinkActivated(_hoveredHyperlinkId);
        }
    }

    /// <summary>
    /// Specify the class to receive hyperlink callbacks
    /// </summary>
    /// <param name="hyperlinkCallback">The class implementing the IHyperlinkCallback interface</param>
    public void setHyperlinkCallback(IHyperlinkCallback hyperlinkCallback)
    {
        _hyperlinkCallback = hyperlinkCallback;
    }

    /// <summary>
    /// Whether the mouse is currently over a hyperlink
    /// </summary>
    /// <returns>true if the mouse is over a hyperlink, otherwise false</returns>
    public bool isHyperlinkHovered()
    {
        return _hoveredHyperlinkId.Length != 0;
    }

    /// <summary>
    /// Retrieve the hyperlink ID, if the mouse is currently over a hyperlink
    /// </summary>
    /// <returns>The hyperlink ID</returns>
    public string getHoveredHyperlink()
    {
        return _hoveredHyperlinkId;
    }

    /// <summary>
    /// Whether the mouse has clicked on a hyperlink
    /// </summary>
    /// <returns>true if the mouse has clicked on a hyperlink, otherwise false</returns>
    public bool isHyperlinkActivated()
    {
        return _activatedHyperlink;
    }

    /// <summary>
    /// Retrieve the hyperlink ID, if the mouse has clicked on a hyperlink
    /// </summary>
    /// <returns></returns>
    public string getActivatedHyperlink()
    {
        return _activatedHyperlink ? _hoveredHyperlinkId : "";
    }

    #region IHyperlinkCallback Members

    public void onHyperlinkEnter(string hyperlinkId)
    {
        //Debug.Log("onHyperlinkEnter: " + hyperlinkId);
        _hoveredHyperlinkId = hyperlinkId;
        if (_hyperlinkCallback != null)
        {
            _hyperlinkCallback.onHyperlinkEnter(hyperlinkId);
        }
    }

    public void onHyperLinkActivated(string hyperlinkId)
    {
        //Debug.Log("onHyperLinkActivated: " + hyperlinkId);
        _activatedHyperlink = true;
        if (_hyperlinkCallback != null)
        {
            _hyperlinkCallback.onHyperLinkActivated(hyperlinkId);
        }
    }

    public void onHyperlinkLeave(string hyperlinkId)
    {
        //Debug.Log("onHyperlinkLeave: " + hyperlinkId);
        _hoveredHyperlinkId = "";
        if (_hyperlinkCallback != null)
        {
            _hyperlinkCallback.onHyperlinkLeave(hyperlinkId);
        }
    }
    #endregion
}