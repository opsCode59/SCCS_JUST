import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Photo } from '../_models/Photo';
import { user } from '../_models/user';
import { Statistics } from '../_models/statistics';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private statisticsSource = new BehaviorSubject<Statistics | null>(null);
  statistics$ = this.statisticsSource.asObservable();
  private usersSource = new BehaviorSubject<user[] | null>(null);
  users$ = this.usersSource.asObservable();
  constructor(private http: HttpClient) {}

  getUsersWithRoles(): Observable<user[]> {
    return this.http
      .get<user[]>(this.baseUrl + 'admin/users-with-roles')
      .pipe(tap((users) => this.usersSource.next(users)));
  }

  updateUserRoles(userName: string, roles: string[]): Observable<user[]> {
    return this.http.post<user[]>(
      this.baseUrl + 'admin/edit-roles/' + userName + '?roles=' + roles,
      {}
    );
  }

  getPhotosForApproval(): Observable<Photo[]> {
    return this.http.get<Photo[]>(this.baseUrl + 'admin/photos-to-moderate');
  }

  approvePhoto(photoId: number): Observable<void> {
    return this.http.post<void>(
      this.baseUrl + 'admin/approve-photo/' + photoId,
      {}
    );
  }
  deleteUser(username: string) {
    return this.http.delete(this.baseUrl + 'admin/delete-user/' + username);
  }
  register(model: any) {
    return this.http.post<user>(this.baseUrl + 'admin/register', model);
  }
  
  getStatistics() {
    return this.http
      .get<Statistics>(this.baseUrl + 'admin/GetStatistics')
      .pipe(tap((statistics) => this.statisticsSource.next(statistics)));
  }

  rejectPhoto(photoId: number): Observable<void> {
    return this.http.post<void>(
      this.baseUrl + 'admin/reject-photo/' + photoId,
      {}
    );
  }
}
