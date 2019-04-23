namespace SimpleXAMLLocalizationHelper.Messages
{
    public class ResetMessage
    {
        public bool isSoftReset;
        public string newPath;
        public bool isFolderMode;

        public ResetMessage(bool _isSoftReset = false)
        {
            isSoftReset = _isSoftReset;
        }

        public ResetMessage(bool _isFolderMode, string _newPath)
        {
            isSoftReset = true;
            isFolderMode = _isFolderMode;
            newPath = _newPath;
        }
    }
}