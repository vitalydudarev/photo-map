import { NgModule } from '@angular/core';
import { ScrollTopComponent } from './scroll-top/scroll-top.component';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';


@NgModule({
  declarations: [
    ScrollTopComponent
  ],
  imports: [
    MatIconModule,
    MatButtonModule,
    CommonModule
  ],
  providers: [
  ],
  exports: [
    ScrollTopComponent
  ]
})
export class SharedModule { }
