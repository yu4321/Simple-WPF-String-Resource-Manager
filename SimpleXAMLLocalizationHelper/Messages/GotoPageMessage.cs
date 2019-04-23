namespace SimpleXAMLLocalizationHelper.Messages
{
    public enum PageName { Start, Core, Setting, History }

    public class GotoPageMessage
    {
        public PageName NextPage;

        public GotoPageMessage(PageName _nextPage)
        {
            NextPage = _nextPage;
        }
    }
}