import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  ActivatedRoute,
  Router,
  RouterLinkActive,
  RouterModule,
} from '@angular/router';
import { Gallery, GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { member } from '../../_models/member';
import { message } from '../../_models/message';
import { user } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { ClinicService } from 'src/app/_services/clinic.service';
import { Appointment } from 'src/app/_models/appointment';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [
    CommonModule,
    TabsModule,
    GalleryModule,
    TimeagoModule,
    MemberMessagesComponent,
    RouterModule,
  ],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  member: member = {} as member;
  images: GalleryItem[] = [];
  appointment?: Appointment;
  activeTab?: TabDirective;
  messages: message[] = [];
  user?: user;
  model: any;
  constructor(
    public accountService: AccountService,
    private memberService: MembersService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private clinicService: ClinicService,
    private messageService: MessageService,
    public presenceService: PresenceService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
        }
      },
    });
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  ngOnInit(): void {
    //this.loadMember();
    this.route.data.subscribe({
      next: (data) => (this.member = data['member']),
    });

    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });
    this.getImages();
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find((x) => x.heading === heading)!.active = true;
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  loadMessages() {
    if (this.member?.userName) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: (messages) => {
          this.messages = messages;
        },
      });
    }
  }

  //loadMember() {
  //  var username = this.route.snapshot.paramMap.get('username');
  //  if (!username) return;
  //  this.memberService.getMember(username).subscribe({
  //    next: member => {
  //      this.member = member,
  //        this.getImages()
  //    }
  //  })
  //}
  getImages() {
    if (!this.member) return;
    for (const photo of this.member.photos) {
      console.log('url is' + photo.url);
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }

  AddLike(member: member) {
    this.memberService.addLike(member.userName).subscribe({
      next: () => {
        this.toastr.success('You have liked ' + member.knownAs);
      },
    });
  }
  addAppointment() {
    const appointmentData = {
      userName: this.user?.userName,
      clinicUsername: this.member.userName,
      isApproved: false,
    };

    this.clinicService.addAppointment(appointmentData).subscribe({
      next: (appointment) => {
        this.toastr.success('Appointment made successfully');
      }
    });
  }
}
