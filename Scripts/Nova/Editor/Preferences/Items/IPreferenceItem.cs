namespace Nova.Editor
{
    internal interface IPreferenceItem
    {
        string Name { get; }

        void OnInitialize();
        void OnGUI();
    }
}
