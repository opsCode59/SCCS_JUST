import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit{
  baseUrl = environment.apiUrl;
  validationErrors: string[]=[];
  ngOnInit(): void {
  }
  constructor(private http: HttpClient,private toastr:ToastrService) { }
  get404Error() {
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe({
      next: Response => console.log(Response),
      error: error => console.log(error)
    })
  }
  get400Error() {
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next: Response => console.log(Response),
      error: error => console.log(error)
    })
  }
  get500Error() {
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({
      next: Response => console.log(Response),
      error: error => console.log(error)
    })
  }
  get401Error() {
    this.http.get(this.baseUrl + 'buggy/auth').subscribe({
      next: Response => console.log(Response),
      error: error => console.log(error)
    })
  }
  get400ValidationError() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe({
      next: Response => console.log(Response),
      error: error =>this.validationErrors=error
    })
  }

}
