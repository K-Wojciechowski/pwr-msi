import {RestaurantMenuItemOptionList} from "./menu/restaurant-menu-item-option-list";
import {MenuItemOptionItemWrapper} from "./menu-item-option-item-wrapper";
import {OrderItemCustomization} from "./order/order-item-customization";

export class MenuItemOptionListWrapper {
   public value: RestaurantMenuItemOptionList;
   public items: MenuItemOptionItemWrapper[];
   public choice: number;

   constructor(value: RestaurantMenuItemOptionList, items: MenuItemOptionItemWrapper[]) {
       this.value = value;
       this.items = items;
       this.choice = 0;
   }

   getChoices(): MenuItemOptionItemWrapper[] {
       if (this.value.isMultipleChoice) {
           return this.items.filter(i => i.checked);
       } else {
           const choiceItem = this.items.find(i => i.value.menuItemOptionItemId === this.choice);
           return choiceItem === undefined ? [] : [choiceItem];
       }
   }

    asOrderItemCustomizations(): OrderItemCustomization[] {
        return this.getChoices().map(c => {
            return {menuItemOptionItem: c.value};
        });
    }
}
