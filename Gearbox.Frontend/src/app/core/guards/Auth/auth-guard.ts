import { CanMatchFn } from '@angular/router';
import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanMatchFn = (route, segments) => {
  const http = inject(HttpClient);

  return http.get<any>('api/auth/me', { withCredentials: true }).pipe(
    map(user => {
      return !!user; // allow access if logged in
    }),
    catchError(() => of(false))
  );
};