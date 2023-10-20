namespace RPG.Core
{
    public interface IHideable
    {
        bool IsHidden { get; }
        void Show();
        void Hide();
    }
}
