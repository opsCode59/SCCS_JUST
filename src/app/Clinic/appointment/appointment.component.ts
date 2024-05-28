import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs';
import { Appointment } from 'src/app/_models/appointment';
import { user } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { AdminService } from 'src/app/_services/admin.service';
import { ClinicService } from 'src/app/_services/clinic.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-appointment',
  templateUrl: './appointment.component.html',
  styleUrls: ['./appointment.component.css'],
})
export class AppointmentComponent implements OnInit {
  Appointments: Appointment[] = [];
  clinic: user | undefined;
  constructor(
    private clinicService: ClinicService,
    private accountService: AccountService,
    private route: ActivatedRoute,
    private memberService: MembersService
  ) {}
  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.clinic = user;
        }
      },
    });
    this.getAppointmentsForApproval();
  }
  getAppointmentsForApproval() {
    this.clinicService
      .getAppointmentsForApproval(this.clinic?.userName)
      .subscribe({
        next: (Appointments) => (this.Appointments = Appointments),
      });
  }
  approveAppointment(AppointmentId: number) {
    this.clinicService.approveAppointment(AppointmentId).subscribe({
      next: () => this.getAppointmentsForApproval(),
    });
  }
  rejectAppointment(AppointmentId: number) {
    this.clinicService.rejectAppointment(AppointmentId).subscribe({
      next: () =>
        this.Appointments.splice(
          this.Appointments.findIndex((p) => p.id === AppointmentId),
          1
        ),
    });
  }
}
