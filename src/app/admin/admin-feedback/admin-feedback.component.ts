import { Component, OnInit } from '@angular/core';
import { feedback } from 'src/app/_models/feedback';
import { user } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { FeedbackService } from 'src/app/_services/feedback.service';

@Component({
  selector: 'app-admin-feedback',
  templateUrl: './admin-feedback.component.html',
  styleUrls: ['./admin-feedback.component.css'],
})
export class AdminFeedbackComponent implements OnInit {
  users: user[] | null = [];
  feedbacks: feedback[] = [];
  feedback: feedback | undefined;
  availableRoles = ['Admin', 'Doctor', 'Member', 'Clinic'];

  constructor(
    private adminService: AdminService,
    private feedbackService: FeedbackService
  ) {}

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: (users) => (this.users = users),
    });
  }

  getAllFeedback() {
    this.feedbackService.getAllFeedback().subscribe();
    this.feedbackService.feedbacks$.subscribe({
      next: (feedbacks) => (this.feedbacks = feedbacks),
    });
  }

  removeFeedback(id: number) {
    this.feedbackService.deleteFeedback(id).subscribe({
      next: () => {
        // Feedbacks list is already updated in the service using BehaviorSubject
      },
    });
  }

  getFeedbackByuserName(username: string) {
    this.feedbackService.getFeedbackByuserName(username).subscribe({
      next: (feedback) => {
        if (feedback) this.feedback = feedback;
      },
    });
  }

  ngOnInit(): void {
    this.getUsersWithRoles();
    this.getAllFeedback();
  }
}
