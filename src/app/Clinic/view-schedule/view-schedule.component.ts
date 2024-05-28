import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Schedule } from 'src/app/_models/Schedule';
import { user } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { ClinicService } from 'src/app/_services/clinic.service';

@Component({
  selector: 'app-view-schedule',
  templateUrl: './view-schedule.component.html',
  styleUrls: ['./view-schedule.component.css'],
})
export class ViewScheduleComponent implements OnInit {
  schedules: Schedule[] = [];
  user: user | undefined;
  constructor(
    private clinicalService: ClinicService,
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}
  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
          this.getAllScheduleByClinicUserName(this.user.userName);
        }
      },
    });
    console.log(this.user);
  }
  deleteSchedule(userName: string, clinicUserName: string) {
    this.clinicalService.deleteSchedule(userName, clinicUserName).subscribe({
      next: () => {
        this.toastr.success('Schedule has been deleted');
        this.schedules = this.schedules.filter(
          (schedule) =>
            !(
              schedule.userName === userName &&
              schedule.clinicUserName === clinicUserName
            )
        );
      },
    });
  }
  getAllScheduleByClinicUserName(clinicUserName: string) {
    this.clinicalService.getScheduleByCliniUserName(clinicUserName).subscribe({
      next: (schedules) => {
        if (schedules) {
          this.schedules = schedules;
        }
      },
    });
  }
}
