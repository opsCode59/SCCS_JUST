import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { user } from '../_models/user';
import { Appointment } from '../_models/appointment';
import { createAppointment } from '../_models/createAppointment';
import { Schedule } from '../_models/Schedule';

@Injectable({
  providedIn: 'root',
})
export class ClinicService {
  baseUrl = environment.apiUrl;
  private usersSource = new BehaviorSubject<user[] | null>(null);
  users$ = this.usersSource.asObservable();
  createAppointment: createAppointment | undefined;

  constructor(private http: HttpClient) {}
  getUsersLikeClinic(): Observable<user[]> {
    return this.http
      .get<user[]>(this.baseUrl + 'clinic/subscribe', {})
      .pipe(tap((users) => this.usersSource.next(users)));
  }
  getUsersWithRoles(): Observable<user[]> {
    return this.http
      .get<user[]>(this.baseUrl + 'clinic', {})
      .pipe(tap((users) => this.usersSource.next(users)));
  }

  addSchedule(model: any) {
    return this.http.post(this.baseUrl + 'clinic/add-schedule', model, {});
  }
  getScheduleByCliniUserName(cliniUserName: string): Observable<Schedule[]> {
    return this.http.get<Schedule[]>(
      this.baseUrl + 'clinic/view-schedule/' + cliniUserName
    );
  }
  deleteSchedule(userName: string, clinicUserName: string): Observable<void> {
     const params = new HttpParams()
       .set('userName', userName)
       .set('clinicUserName', clinicUserName);
    return this.http.delete<void>(this.baseUrl + 'clinic/delete-schedule', { params });
  }
  getAppointmentsForApproval(
    clinicUsername: string | undefined
  ): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(
      this.baseUrl + 'clinic/Appointment-to-clinic/' + clinicUsername
    );
  }
  addAppointment(model: any) {
    this.createAppointment = model;
    console.log(this.createAppointment);
    return this.http.post<createAppointment>(
      this.baseUrl + 'clinic/accept/',
      this.createAppointment
    );
  }

  approveAppointment(appointmentId: number): Observable<void> {
    return this.http.post<void>(
      this.baseUrl + 'clinic/approve-appointment/' + appointmentId,
      {}
    );
  }
  rejectAppointment(appointmentId: number): Observable<void> {
    return this.http.post<void>(
      this.baseUrl + 'clinic/reject-appointment/' + appointmentId,
      {}
    );
  }
}
