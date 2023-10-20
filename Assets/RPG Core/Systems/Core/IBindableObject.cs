using System;
namespace InventorySystem
{
    public interface IBindableObject<T>
    {
        void Bind(Action<T> handleObjectChanged);
        void Unbind(Action<T> handleObjectChanged);
    }
}