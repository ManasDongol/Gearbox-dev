import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay, catchError, of, map, switchMap, ReplaySubject, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BehaviorSubject } from 'rxjs';
export type Role = 'Admin' | 'Staff' | 'Customer';


export interface User {
  userId: string;
  email: string;
  name?: string;
  roles: string[];
}

@Injectable({ providedIn: 'root' })
export class Auth {
  private http = inject(HttpClient);
   baseapi=environment.apiUrl;

  private userSubject = new BehaviorSubject<User | null>(null);
  user$ = this.userSubject.asObservable();

  get user() {
    return this.userSubject.value;
  }

loadUser() {
  return this.http.get<User>(this.baseapi+'/auth/me').pipe(
    tap(user => this.userSubject.next(user)),
    catchError(() => {
      this.userSubject.next(null);
      return of(null);
    })
  );
}

  hasRole(role: Role): boolean {
    return this.user?.roles?.includes(role) ?? false;
  }

  getRole():string[] {
    return this.user!.roles;
  }

  hasAnyRole(roles: Role[]): boolean {
  return this.user?.roles?.some(r => roles.includes(r as Role)) ?? false;
}
  isLoggedIn(): boolean {
    return !!this.user;
  }

  logout() {
    this.userSubject.next(null);
  }

}
