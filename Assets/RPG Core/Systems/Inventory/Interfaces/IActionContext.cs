namespace Item
{
    //A context within which an action is executed
    public interface IActionContext<TItem> where TItem : class, IActionItem
    {
        void Use(TItem action);
    }

    public interface IActionContext : IActionContext<IActionItem> { }
}