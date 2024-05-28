import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { member } from '../../_models/member';
import { Pagination } from '../../_models/pagination';
import { user } from '../../_models/user';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  //  members$: Observable<member[]> | undefined;
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [
    { value: 'male', display: 'Male' },
    { value: 'female', display: 'Female' },
    { value: 'Clinic', display: 'clinic' },
    { value: 'Doctor', display: 'doctor' },
  ];
  members: member[] = [];
  ngOnInit(): void {
    this.loadMembers();
    //this.members$ = this.memberService.getMembers();
  }
  constructor(private memberService: MembersService) {
    this.userParams = this.memberService.getUserParams();
  }
  resetFilters() {
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }

  //loadMembers() {
  //  this.memberService.getMembers().subscribe({
  //    next: members => this.members =members
  //  })
  //  console.log(this.members);
  //}
  loadMembers() {
    if (this.userParams) {
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: (Response) => {
          if (Response.result && Response.pagination) {
            this.members = Response.result;
            this.pagination = Response.pagination;
          }
        },
      });
    }
  }
  pageChanged(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
