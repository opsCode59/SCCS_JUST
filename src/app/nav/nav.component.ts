import { Component, OnInit } from '@angular/core';
import { Route, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
  model: any = {}

  constructor(public accountService: AccountService,private toastr:ToastrService,private router:Router) { }
    ngOnInit(): void {
  }

  Login() {
    this.accountService.Login(this.model).subscribe({
      next:()=>this.router.navigateByUrl('/members'),
        //console.log(response);
      //  this.loggedIn = true;
      error: error => this.toastr.error(error.error)
    })
  }
  Logout() {
    this.accountService.Logout();
    this.router.navigateByUrl('/');
    //  this.loggedIn = false;
  }
}
