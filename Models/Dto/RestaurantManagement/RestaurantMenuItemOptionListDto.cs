namespace pwr_msi.Models.Dto {
    public class RestaurantMenuItemOptionListDto {
        public int MenuItemOptionListId { get; set; }
        public string Name { get; set; }
        public bool IsMultipleChoice { get; set; }
        public int MenuItemId { get; set; }
        
        public MenuItemOptionList AsNewMenuItemOptionList(int itemId) => new() {
            MenuItemOptionListId = MenuItemOptionListId,
            Name = Name,
            IsMultipleChoice = IsMultipleChoice,
            MenuItemId = itemId
        };
    }
}
