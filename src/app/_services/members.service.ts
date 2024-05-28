import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of, take } from 'rxjs';
import { environment } from '../../environments/environment';
import { member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { user } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { createAppointment } from '../_models/createAppointment';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  members: member[] = [];
  baseUrl = environment.apiUrl;
  memberCache = new Map();
  userParams: UserParams | undefined;
  user: user | undefined;
  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      },
    });
  }
  getUserParams() {
    return this.userParams;
  }
  setUserParams(params: UserParams) {
    this.userParams = params;
  }
  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }
  //getHttpOptions() {
  //  const userString = localStorage.getItem('user');
  //  if (!userString) return;

  //  const user = JSON.parse(userString);
  //  return {
  //    headers: new HttpHeaders({
  //      'Authorization': 'Bearer ' + user.token
  //    })
  //  };
  //}

  getMembers(UserParams: UserParams) {
    //get members form cache
    const response = this.memberCache.get(Object.values(UserParams).join('-'));
    if (response) return of(response);

    let params = getPaginationHeaders(
      UserParams.pageNumber,
      UserParams.pageSize
    );
    //if (this.members.length > 0) return of(this.members);
    params = params.append('minAge', UserParams.minAge);
    params = params.append('maxAge', UserParams.maxAge);
    params = params.append('gender', UserParams.gender);
    params = params.append('orderBy', UserParams.orderBy);

    return getPaginatedResult<member[]>(
      this.baseUrl + 'user/',
      params,
      this.http
    ).pipe(
      map((response) => {
        this.memberCache.set(Object.values(UserParams).join('-'), response);
        return response;
      })
    );
  }



  getMember(userUame: string) {
    console.log(userUame);
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: member) => member.userName === userUame);

    if (member) return of(member);
    return this.http.get<member>(this.baseUrl + 'user/' + userUame);
  }

  updateMember(member: member) {
    return this.http.put(this.baseUrl + 'user', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'user/set-main-photo/' + photoId, {});
  }
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'user/delete-photo/' + photoId);
  }
  addLike(username: string) {
    console.log(username);
    return this.http.post(this.baseUrl + 'like/' + username, {});
  }
  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);

    return getPaginatedResult<member[]>(
      this.baseUrl + 'like',
      params,
      this.http
    );
  }
}
