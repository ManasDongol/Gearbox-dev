import { inject } from '@angular/core';
import { CanMatchFn } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { map,catchError,of } from 'rxjs';
export const customerGuard: CanMatchFn = (route, segments) => {
const http = inject(HttpClient);

  return http.get<any>('api/auth/me', { withCredentials: true }).pipe(
    map(user => user.roles?.includes('Customer')),
    catchError(() => of(false)))
};
