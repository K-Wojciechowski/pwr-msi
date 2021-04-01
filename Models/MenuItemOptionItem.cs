namespace pwr_msi.Models {
    public class MenuItemOptionItem {
        public int MenuItemOptionItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MenuItemOptionListId { get; set; }
        public virtual MenuItemOptionList MenuItemOptionList { get; set; }
    }
}
