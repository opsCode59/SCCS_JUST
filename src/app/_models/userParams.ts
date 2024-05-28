import { user } from './user';

export class UserParams {
  gender: string;
  minAge = 18;
  maxAge = 99;
  pageNumber = 1;
  pageSize = 5;
  orderBy = 'lastActive';
  constructor(user: user) {
    if (user.gender === 'female') this.gender = 'male';
    else if (user.gender === 'male') this.gender = 'female';
    else if (user.gender === 'Clinic') this.gender = 'Clinic';
    else this.gender = 'Doctor';
  }
}
