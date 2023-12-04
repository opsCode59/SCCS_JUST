import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  model: any = {}
  ngOnInit(): void {
  }
  constructor(private accountService: AccountService,private toastr:ToastrService,private router: Router) { }
  @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  register() {
    this.accountService.register(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');      
        this.cancel();
      },
      error: error => this.toastr.error(error.error)
    })
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
