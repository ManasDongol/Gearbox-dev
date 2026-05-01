import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Check if the request URL starts with the base API URL
  // If so, attach withCredentials to true to ensure cookies are sent and received
  if (req.url.startsWith(environment.apiUrl) || req.url.startsWith(environment.apiUrl.replace(/\/$/, ''))) {
    const authReq = req.clone({
      withCredentials: true
    });
    return next(authReq);
  }
  
  return next(req);
};
