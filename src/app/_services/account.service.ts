import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { user } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private currentUserSource = new BehaviorSubject<user|null>(null);
  currentUser$ = this.currentUserSource.asObservable();

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient,private presenceService:PresenceService) { }

  Login(model: any) {
    return this.http.post<user>(this.baseUrl + 'account/login', model).pipe(
      map((Response: user) => {
        const user = Response;
        if (user) {
          this.setCurrentUser(user);
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
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
    this.presenceService.createHubConnection(user);
  }
  Logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();
  }
  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]))
  }
}
