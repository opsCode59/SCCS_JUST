import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { feedback } from '../_models/feedback';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class FeedbackService {
  baseUrl = environment.apiUrl;
  private feedbacksSource = new BehaviorSubject<feedback[]>([]);
  feedbacks$ = this.feedbacksSource.asObservable();

  constructor(private http: HttpClient) {}

  getAllFeedback() {
    return this.http
      .get<feedback[]>(this.baseUrl + 'feedback', {})
      .pipe(
        tap((feedbacks: feedback[]) => this.feedbacksSource.next(feedbacks))
      );
  }

  deleteFeedback(id: number) {
    return this.http
      .delete(this.baseUrl + 'feedback/delete-Feedback/' + id, {})
      .pipe(
        tap(() => {
          const currentFeedbacks = this.feedbacksSource.getValue();
          this.feedbacksSource.next(
            currentFeedbacks.filter((f) => f.id !== id)
          );
        })
      );
  }

  getFeedbackByuserName(username: string): Observable<feedback> {
    return this.http.get<feedback>(this.baseUrl + 'feedback/' + username, {});
  }

  addFeedback(feedback: feedback): Observable<feedback> {
    return this.http
      .post<feedback>(this.baseUrl + 'feedback/Add-Feedback', feedback)
      .pipe(
        tap((newFeedback: feedback) => {
          const currentFeedbacks = this.feedbacksSource.getValue();
          this.feedbacksSource.next([...currentFeedbacks, newFeedback]);
        })
      );
  }
}
