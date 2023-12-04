import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { user } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private currentUserSource = new BehaviorSubject<user|null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  baseUrl = "https://localhost:5001/api/";
  constructor(private http: HttpClient) { }

  Login(model: any) {
    return this.http.post<user>(this.baseUrl + 'account/login', model).pipe(
      map((Response: user) => {
        const user = Response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    )
  }
  register(model: any) {
    return this.http.post<user>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
            this.currentUserSource.next(user);
        }
        return user;
      })
    )
  }

  setCurrentUser(user: user) {
    this.currentUserSource.next(user);
  }
  Logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
