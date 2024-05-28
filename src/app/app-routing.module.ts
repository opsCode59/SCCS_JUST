import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { adminGuard } from './_guards/admin.guard';
import { authGuard } from './_guards/auth.guard';
import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { memberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { ClinicDashboardComponent } from './Clinic/clinic-dashboard/clinic-dashboard.component';
import { clinicGuard } from './_guards/clinic.guard';
import { ScheduleComponent } from './Clinic/schedule/schedule.component';
import { PostListComponent } from './Posts/post-list/post-list.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { AdminRegisterComponent } from './admin/admin-register/admin-register.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'members', component: MemberListComponent },
      {
        path: 'members/:username',
        component: MemberDetailComponent,
        resolve: { member: memberDetailedResolver },
      },
      { path: 'contactUs', component: ContactUsComponent },

      {
        path: 'member/edit',
        component: MemberEditComponent,
        canDeactivate: [preventUnsavedChangesGuard],
      },
      { path: 'lists', component: ListsComponent },
      { path: 'messages', component: MessagesComponent },
      {
        path: 'clinicDashbord',
        component: ClinicDashboardComponent,
        canActivate: [clinicGuard],
      },
      {
        path: 'clinicDashbord/schedule',
        component: ScheduleComponent,
        canActivate: [clinicGuard],
      },
      {
        path: 'admin',
        component: AdminPanelComponent,
        canActivate: [adminGuard],
      },
      {
        path: 'admin/add-user',
        component: AdminRegisterComponent,
        canActivate: [adminGuard],
      },
    ],
  },
  { path: 'blogs', component: PostListComponent },
  { path: 'aboutUs', component: AboutUsComponent },

  { path: 'errors', component: TestErrorComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', component: HomeComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
