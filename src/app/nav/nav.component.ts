import { Component, OnInit } from '@angular/core';
import { Route, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { user } from '../_models/user';
import { take } from 'rxjs';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  user: user | undefined;
  constructor(
    public accountService: AccountService,
    private toastr: ToastrService,
    private router: Router
  ) {}
  ngOnInit(): void {}

  Login() {
    this.accountService.Login(this.model).subscribe({
      next: () => {
        this.accountService.currentUser$.pipe(take(1)).subscribe({
          next: (user) => {
            if (user) {
              // Check the roles and navigate accordingly
              if (user.roles.includes('Admin')) {
                this.router.navigateByUrl('/admin');
              } else if (user.roles.includes('Clinic')) {
                this.router.navigateByUrl('/clinicDashbord');
              } else {
                this.router.navigateByUrl('/');
              }
            }
          },
          error: (error) => {
            this.toastr.error('Failed to fetch current user');
          },
        });
      },
      error: (error) => {
        this.toastr.error(error.error);
      },
    });
  }
  Logout() {
    this.accountService.Logout();
    this.router.navigateByUrl('/');
    //  this.loggedIn = false;
  }
}
