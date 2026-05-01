import { Injectable } from '@angular/core';
import { inject } from '@angular/core';
import { LoginRequest } from '../../DTOs/LoginRequest';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment.development';
@Injectable({
  providedIn: 'root',
})
export class LoginService
{
  http = inject(HttpClient);
  baseapi = environment.apiUrl;

  login(credentials: LoginRequest) {
   
  return this.http.post(this.baseapi+'/Auth/login', credentials);
}
  
}
