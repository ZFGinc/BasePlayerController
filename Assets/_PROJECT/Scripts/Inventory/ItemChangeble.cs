namespace ZFGinc.InventoryItems
{
    public class ItemChangeble : Item
    {
        private IChangeble changeble;

        protected override void Awake()
        {
            base.Awake();

            changeble = GetComponent<IChangeble>();
        }

        public override void ChangeVisible(bool visible)
        {
            base.ChangeVisible(visible);

            changeble.ChangeStatus(visible);
        }
    }
}