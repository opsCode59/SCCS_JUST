import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouteReuseStrategy, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { CommonModule } from '@angular/common';
import { SharedModule } from './_module/shared.module';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { ErrorInterceptor } from './_interceptor/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { JwtInterceptor } from './_interceptor/jwt.interceptor';
import { LoadingInterceptor } from './_interceptor/loading.interceptor';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DatePickerComponent } from './_forms/date-picker/date-picker.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/has-role.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { RolesModalComponent } from './modal/roles-modal/roles-modal.component';
import { CustomRouteReuseStrategy } from './_services/customRouteReuseStrategy';
import { ConfirmDialogComponent } from './modal/confirm-dialog/confirm-dialog.component';
import { AdminFeedbackComponent } from './admin/admin-feedback/admin-feedback.component';
import { StatisticsComponent } from './admin/statistics/statistics.component';
import { ClinicDashboardComponent } from './Clinic/clinic-dashboard/clinic-dashboard.component';
import { FeedbackComponent } from './Clinic/feedback/feedback.component';
import { AppointmentComponent } from './Clinic/appointment/appointment.component';
import { ScheduleComponent } from './Clinic/schedule/schedule.component';
import { DatePickersComponent } from './_forms/date-pickers/date-pickers.component';
import { ViewScheduleComponent } from './Clinic/view-schedule/view-schedule.component';
import { PostListComponent } from './Posts/post-list/post-list.component';
import { PostAddComponent } from './Posts/post-add/post-add.component';
import { PostCardComponent } from './Posts/post-card/post-card.component';
import { PostManagementComponent } from './Posts/post-management/post-management.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { FooterComponent } from './footer/footer.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { AdminRegisterComponent } from './admin/admin-register/admin-register.component';
@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    //MemberDetailComponent,
    MemberListComponent,
    MessagesComponent,
    ListsComponent,
    TestErrorComponent,
    NotFoundComponent,
    ServerErrorComponent,
    MemberCardComponent,
    PhotoEditorComponent,
    TextInputComponent,
    DatePickerComponent,
    MemberEditComponent,
    AdminPanelComponent,
    HasRoleDirective,
    UserManagementComponent,
    PhotoManagementComponent,
    RolesModalComponent,
    ConfirmDialogComponent,
    AdminFeedbackComponent,
    StatisticsComponent,
    ClinicDashboardComponent,
    FeedbackComponent,
    AppointmentComponent,
    ScheduleComponent,
    DatePickersComponent,
    ViewScheduleComponent,
    PostListComponent,
    PostAddComponent,
    PostCardComponent,
    PostManagementComponent,
    AboutUsComponent,
    FooterComponent,
    ContactUsComponent,
    AdminRegisterComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule, //shared add manual
    RouterModule.forRoot([
      { path: 'nav', component: NavComponent },
      { path: 'register', component: RegisterComponent },
    ]),
  ],
  providers: [
    { provide: RouteReuseStrategy, useClass: CustomRouteReuseStrategy },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
