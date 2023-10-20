namespace Item
{
    public interface IActionItem : IItem    
    {
        void Use(IActionContext context);
    }
}

