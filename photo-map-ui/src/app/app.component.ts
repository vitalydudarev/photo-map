import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'photo-map-ui';

  menuItems = [ { title: "Gallery", route: "/gallery" } ]
}
