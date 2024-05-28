import { Component, OnInit } from '@angular/core';
import { Statistics } from 'src/app/_models/statistics';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css'],
})
export class StatisticsComponent implements OnInit {
  Statistics: Statistics | null = null;

  constructor(public adminService: AdminService) {}
  reloadData() {
    this.getStatistics();
  }
  ngOnInit(): void {
    this.getStatistics();
  }

  getStatistics() {
    this.adminService.getStatistics().subscribe();
    this.adminService.statistics$.subscribe({
      next: (statistics) => (this.Statistics = statistics),
    });
  }
}
