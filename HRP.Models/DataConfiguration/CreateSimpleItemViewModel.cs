namespace HRP.Models.DataConfiguration
{
    public class CreateSimpleItemViewModel
    {
        public CreateSimpleItemViewModel()
        {
            Item = new SimpleItem();
        }

        public SimpleItem Item { get; set; }

        public string FieldName { get; set; }

        public int FieldId { get; set; }

        public string LinkCreateItem { get; set; }
    }
}