import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.html',
  styleUrls: ['./confirm-email.css'],
  imports:[RouterLink]
})
export class ConfirmEmailComponent implements OnInit {

  status: 'loading' | 'success' | 'error' = 'loading';

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const token = this.route.snapshot.queryParamMap.get('token');

    this.http.get(
      `http://localhost:5289/api/auth/confirm-email?userId=${userId}&token=${encodeURIComponent(token!)}`
    ).subscribe({
      next: () => this.status = 'success',
      error: () => this.status = 'error'
    });
  }
}