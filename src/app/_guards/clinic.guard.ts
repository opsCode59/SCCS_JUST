import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';
import { AccountService } from '../_services/account.service';

export const clinicGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  return accountService.currentUser$.pipe(
    map((user) => {
      if (!user) return false;

      if (user.roles.includes('Clinic')) {
        return true;
      } else {
        toastr.error('You connet enter this area');
        return false;
      }
    })
  );
};
