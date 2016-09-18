public interface IHyperlinkCallback
{
    void onHyperlinkEnter(string hyperlink);
    void onHyperLinkActivated(string hyperlink);
    void onHyperlinkLeave(string hyperlink);
}
