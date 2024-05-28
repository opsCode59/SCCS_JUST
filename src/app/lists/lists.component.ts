import { Component, OnInit } from '@angular/core';
import { member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';
import { user } from '../_models/user';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  members: member[] | undefined;
  user: user | undefined;
  predicate = '';
  pageNumber = 1;
  pagSize = 4;
  Pagination: Pagination | undefined;
  constructor(
    private memberService: MembersService,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
        }
      },
    });
    if (this.user?.roles.indexOf('Clinic') !== -1) {
      this.predicate = 'likedBy';
    } else if (
      this.user?.roles.indexOf('Patient') !== -1 ||
      this.user?.roles.indexOf('Doctor') !== -1
    ) {
      this.predicate = 'liked';
    }
    this.loadLikes();
  }
  loadLikes() {
    this.memberService
      .getLikes(this.predicate, this.pageNumber, this.pagSize)
      .subscribe({
        next: (response) => {
          (this.members = response.result),
            (this.Pagination = response.pagination);
        },
      });
  }
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }
}
