import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Toast } from "./shared/components/toast/toast";
import { Spinner } from './shared/components/spinner/spinner';
import { ConfirmCard } from './shared/components/confirm-card/confirm-card';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Toast, ConfirmCard],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Gearbox.Frontend');
}
