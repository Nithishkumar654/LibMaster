import { Component, ElementRef } from '@angular/core';
import { ComponentsModule } from '../components.module';

/**
 * HomeComponent manages the display of the homepage, including the mobile menu toggle
 * and a horizontally scrollable carousel of book items.
 */
@Component({
    selector: 'app-home',
    standalone: true,
    imports: [ComponentsModule],
    templateUrl: './home.component.html',
    styleUrl: './home.component.scss'
})
export class HomeComponent {
    /** Flag to track the mobile menu open/close state. */
    mobileMenuOpen: boolean = false;

    /** Reference to the carousel DOM element. */
    carousel!: ElementRef;

    /** Array representing books in the carousel. */
    // books: number[] = [1, 2, 3, 4, 5, 6, 7, 8];

    /**
     * Toggles the state of the mobile menu between open and closed.
     * @returns {void}
     */
    toggleMobileMenu(): void {
        this.mobileMenuOpen = !this.mobileMenuOpen;
    }

    books = [
        { title: 'The Great Gatsby', genre: 'Fiction' },
        { title: 'To Kill a Mockingbird', genre: 'Classic' },
        { title: '1984', genre: 'Dystopian' },
        { title: 'Moby Dick', genre: 'Adventure' },
        { title: 'The Catcher in the Rye', genre: 'Literary Fiction' },
        { title: 'Pride and Prejudice', genre: 'Romance' }
    ];


    /**
     * Scrolls the carousel in the specified direction.
     * @param {'left' | 'right'} direction - The direction to scroll ('left' or 'right').
     * @returns {void}
     */
    scrollCarousel(direction: 'left' | 'right'): void {
        const element = this.carousel.nativeElement;
        const scrollAmount: number = 300;

        if (direction === 'left') {
            element.scrollLeft -= scrollAmount;
        } else {
            element.scrollLeft += scrollAmount;
        }
    }
}
