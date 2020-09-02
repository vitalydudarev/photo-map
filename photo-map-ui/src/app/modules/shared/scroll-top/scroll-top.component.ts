import { Component, OnInit, Inject, HostListener, Input, ViewEncapsulation } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-scroll-top',
  templateUrl: './scroll-top.component.html',
  styleUrls: ['./scroll-top.component.scss'],
  encapsulation: ViewEncapsulation.Emulated
})
export class ScrollTopComponent implements OnInit {
  @Input() scrollToElement;
  windowScrolled: boolean;
    
  constructor(@Inject(DOCUMENT) private document: Document) {}

  ngOnInit() {}

  @HostListener("window:scroll", [])
  onWindowScroll() {
    if (window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop > 100) {
      this.windowScrolled = true;
    } 
    else if (this.windowScrolled && window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop < 10) {
      this.windowScrolled = false;
    }
  }

  /*scrollToTop(): void {
    this.scrollToElement.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }*/
    
  scrollToTop() {
    (function smoothscroll() {
      var currentScroll = document.documentElement.scrollTop || document.body.scrollTop;
      if (currentScroll > 0) {
        window.requestAnimationFrame(smoothscroll);
        window.scrollTo(0, currentScroll - (currentScroll / 8));
      }
    })();
  }
}
