import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  if (req.url.startsWith(environment.apiUrl) || req.url.startsWith(environment.apiUrl.replace(/\/$/, ''))) {
    const authReq = req.clone({
      withCredentials: true
    });
    console.log("yep its being intercepted");
    return next(authReq);
  }
  
  return next(req);
};
