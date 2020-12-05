import { Component, OnInit, Inject, HostListener, ViewEncapsulation } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Component({
    selector: 'app-scroll-control',
    templateUrl: './scroll-control.component.html',
    styleUrls: ['./scroll-control.component.scss'],
    encapsulation: ViewEncapsulation.Emulated
})
export class ScrollControlComponent implements OnInit {
    canScrollUp: boolean;
    canScrollDown: boolean;
      
    constructor(@Inject(DOCUMENT) private document: Document) {
    }

    ngOnInit() {
        this.checkIfCanScrollDown();
    }

    @HostListener("window:scroll", [])
    onWindowScroll() {
        this.checkIfCanScrollUp();
        this.checkIfCanScrollDown();
    }
      
    scrollToTop() {
        (function smoothscroll() {
            const currentPosition = document.documentElement.scrollTop;
            
            if (currentPosition > 0) {
                window.requestAnimationFrame(smoothscroll);
                window.scrollTo(0, currentPosition - (currentPosition / 8));
            }
        })();
    }

    scrollToBottom(): void {
        (function smoothscroll() {
            const currentPosition = document.documentElement.scrollTop;
            const endPosition = document.documentElement.scrollHeight - document.documentElement.clientHeight;
            const diff = endPosition - currentPosition;
            const newPosition = currentPosition + (diff / 8);
            
            if (currentPosition + 10 < endPosition) {
                window.requestAnimationFrame(smoothscroll);
                window.scrollTo(0, newPosition);
            }
        })();             
    }

    private checkIfCanScrollUp() {
        if (window.pageYOffset || document.documentElement.scrollTop > 100) {
            this.canScrollUp = true;
        } else if (this.canScrollUp && window.pageYOffset || document.documentElement.scrollTop < 10) {
            this.canScrollUp = false;
        }
    }

    private checkIfCanScrollDown() {
        if (document.documentElement.scrollTop < document.documentElement.scrollHeight - document.documentElement.clientHeight) {
            this.canScrollDown = true;
        } else {
            this.canScrollDown = false;
        }
    }
}
