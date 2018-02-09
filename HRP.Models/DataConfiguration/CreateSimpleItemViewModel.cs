namespace HRP.Models.DataConfiguration
{
    public class CreateSimpleItemViewModel
    {
        public CreateSimpleItemViewModel()
        {
            Item = new SimpleItem();
        }

        public SimpleItem Item { get; set; }

        public Field Field { get; set; }
    }
}