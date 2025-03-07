import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';

import { catchError } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { LoginStateService } from '../auth/login-state.service';

@Injectable()
export class CustomerHttpInterceptor implements HttpInterceptor {
  constructor(
    private snb: MatSnackBar,
    private router: Router,
    private auth: LoginStateService
  ) {

  }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          return this.handleError(error);
        })
      );
  }
  handleError(error: HttpErrorResponse) {
    const errors = {
      detail: '无法连接到服务器，请检查网络连接!',
      status: 500,
    };

    switch (error.status) {
      case 401:
        errors.detail = '401:未授权的请求';
        this.auth.logout();
        this.router.navigateByUrl('/login');
        break;
      case 403:
        errors.detail = '403:已拒绝请求';
        break;
      case 409:
        errors.detail = error.error.detail;
        break;
      default:
        if (!error.error) {
          if (error.message) {
            errors.detail = error.message;
          }
          errors.status = error.status;
        } else {
          if (error.error.detail) {
            errors.detail = error.error.detail;
          }
          if (error.error.title) {
            errors.detail = error.error.title + ':' + errors.detail;
          }
        }
        break;
    }
    errors.status = error.status;
    this.snb.open(errors.detail);
    return throwError(() => errors);
  }
}
