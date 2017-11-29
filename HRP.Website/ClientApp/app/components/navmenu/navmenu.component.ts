import { Component, Input } from '@angular/core';

import { IMenuItem } from '../../models/menu-item';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})

export class NavMenuComponent {
    @Input() menus: IMenuItem[];
}