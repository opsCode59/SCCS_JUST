import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { feedback } from 'src/app/_models/feedback';
import { user } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { ClinicService } from 'src/app/_services/clinic.service';
import { FeedbackService } from 'src/app/_services/feedback.service';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.css'],
})
export class FeedbackComponent implements OnInit {
  user: user | undefined;
  feedbacks: feedback[] = [];
  feedback: feedback | undefined;
  availableRoles = ['Admin', 'Doctor', 'Member', 'Clinic'];

  constructor(
    private clinicService: ClinicService,
    private feedbackService: FeedbackService,
    private accountService:AccountService
  ) {}

  getAllFeedback() {
    this.feedbackService.getAllFeedback().subscribe();
    this.feedbackService.feedbacks$.subscribe({
      next: (feedbacks) => (this.feedbacks = feedbacks),
    });
  }
  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
        }
      },
    });
    this.getAllFeedback();
  }
}
