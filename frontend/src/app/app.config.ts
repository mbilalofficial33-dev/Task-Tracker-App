import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),

    // ✅ HTTP + INTERCEPTOR
    provideHttpClient(
      withInterceptors([
        (req, next) => {
          const token = localStorage.getItem('token');

          if (token) {
            req = req.clone({
              headers: req.headers.set('Authorization', `Bearer ${token}`)
            });
          }

          return next(req);
        }
      ])
    )
  ]
};