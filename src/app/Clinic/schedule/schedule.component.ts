import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { user } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { ClinicService } from 'src/app/_services/clinic.service';

@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css'],
})
export class ScheduleComponent {
  registerForm: FormGroup = new FormGroup({});
  minDate: Date = new Date();
  ValidationErrors: string[] | undefined;
  clinic: user | undefined;
  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.clinic = user;
          this.initializeForm();
        }
      },
    });
  }
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private router: Router,
    private fb: FormBuilder,
    private clinicService: ClinicService
  ) {}

  initializeForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      clinicUserName: [
        { value: this.clinic?.userName, disabled: true },
        Validators.required,
      ],
    });
  }
  register() {
const formValue = this.registerForm.getRawValue(); // Use getRawValue to include disabled controls
    console.log(formValue);
    this.clinicService.addSchedule(formValue).subscribe({
      next: () => {
        this.router.navigateByUrl('/clinicDashbord'),
          this.toastr.success('Schedule is Add');
      },
      error: (error) => this.toastr.error(error.error),
    });
  }
}
