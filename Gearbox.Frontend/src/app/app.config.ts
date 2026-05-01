import { ApplicationConfig, APP_INITIALIZER } from '@angular/core';
import { provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { Auth } from './core/services/auth/auth';
import { firstValueFrom } from 'rxjs';

export function initializeAuth(auth: Auth) {
  return () => firstValueFrom(auth.loadUser()).catch(() => null);
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),

    
    {
      provide: APP_INITIALIZER,
      useFactory: initializeAuth,
      deps: [Auth],
      multi: true
    }
  ]
};