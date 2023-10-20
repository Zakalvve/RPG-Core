namespace InventorySystem
{
    //An inventory that can contain any item but allows trying to execute an item within it
    //If the item  is an active item it will be executed orwise the attempt fails
    public interface IActiveInventory<TIndex>
    {
        public void TryExecute(TIndex index);
    }
}
