import { CommonModule } from '@angular/common';
import { Component, HostListener, inject } from '@angular/core';
import { ConfirmCardService } from './confirm-card.service';

@Component({
  selector: 'app-confirm-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm-card.html',
  styleUrl: './confirm-card.css',
})
export class ConfirmCard {
  confirmCard = inject(ConfirmCardService);

  @HostListener('document:keydown.escape')
  onEscape() {
    this.confirmCard.dismiss();
  }
}
