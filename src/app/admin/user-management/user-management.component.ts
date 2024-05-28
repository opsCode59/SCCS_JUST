import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from '../../modal/roles-modal/roles-modal.component';
import { user } from '../../_models/user';
import { AdminService } from '../../_services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
})
export class UserManagementComponent implements OnInit {
  users: user[] = [];
  user: user | undefined;
  availableRoles = ['Admin', 'Doctor', 'Member', 'Clinic'];
  bsModalRef: BsModalRef<RolesModalComponent> =
    new BsModalRef<RolesModalComponent>();
  constructor(
    private adminService: AdminService,
    private modalService: BsModalService
  ) {}

  getUsersWithRoles() {
     this.adminService.getUsersWithRoles().subscribe({
      next: (users) => (this.users = users),
    });
  }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }
  openRolesModal(user: user) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        userName: user.userName,
        availableRoles: this.availableRoles,
        selectedRoles: [...user.roles],
      },
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content?.selectedRoles;
        if (!this.arrayEqual(selectedRoles!, user.roles)) {
          this.adminService
            .updateUserRoles(user.userName, selectedRoles!)
            .subscribe({
              next: (roles) => {
                user.roles = roles;
              },
            });
        }
      },
    });
  }
  deleteUser(username: string) {
    this.adminService.deleteUser(username).subscribe({
      next: (_) => {
        this.users = this.users.filter((p) => p.userName !== username);
      },
    });
    this.getUsersWithRoles();
  }

  private arrayEqual(arr1: any[], arr2: any[]) {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort());
  }
}
