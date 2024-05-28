import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class MemberCardComponent implements OnInit {
  @Input() member: member | undefined;
  constructor(private memberService: MembersService
    , private toastr: ToastrService
    , public presenceService: PresenceService) { }
    ngOnInit(): void {
    }
  AddLike(member: member) {
    this.memberService.addLike(member.userName).subscribe({
      next: () => {
        this.toastr.success('You have liked '+ member.knownAs);
      },
    })
  }

}
