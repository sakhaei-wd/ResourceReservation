import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import moment from 'jalali-moment';

import {
  ReservationService,
  ResourceWithSlots,
  CreateReservationDto,
  ReservationDto,
} from './services/reservation.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule,
    MatChipsModule,
    MatSnackBarModule,
  ],
  providers: [ReservationService],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  resources: ResourceWithSlots[] = [];
  reservations: ReservationDto[] = [];
  selectedResourceId: number | null = null;
  startTime = '';
  endTime = '';
  userId = 1;
  loading = false;

  displayedColumns = ['id', 'resource', 'start', 'end', 'status', 'actions'];

  constructor(
    private reservationService: ReservationService,
    private snackBar: MatSnackBar,
  ) {}

  ngOnInit() {
    this.loadResources();
  }

  loadResources() {
    this.reservationService.getResourcesWithSlots().subscribe({
      next: (data) => (this.resources = data),
      error: () => this.showError('خطا در دریافت منابع'),
    });
  }

  // تبدیل تاریخ میلادی به شمسی برای نمایش
  toJalali(dateStr: string): string {
    return moment(dateStr).locale('fa').format('jYYYY/jMM/jDD HH:mm');
  }

  // تبدیل ورودی شمسی به میلادی برای ارسال به API
  fromJalali(jalaliStr: string): string {
    return moment.from(jalaliStr, 'fa', 'jYYYY/jMM/jDD HH:mm').toISOString();
  }

  createReservation() {
    if (!this.selectedResourceId || !this.startTime || !this.endTime) {
      this.lastMessage = { type: 'error', text: 'لطفاً همه فیلدها را پر کنید' };
      return;
    }
    this.loading = true;
    this.lastMessage = null;
    const dto: CreateReservationDto = {
      resourceId: this.selectedResourceId,
      userId: this.userId,
      startTime: this.fromJalali(this.startTime),
      endTime: this.fromJalali(this.endTime),
    };
    this.reservationService.createReservation(dto).subscribe({
      next: (res) => {
        this.lastMessage = { type: 'success', text: 'رزرو با موفقیت ثبت شد' };
        this.reservations = [res, ...this.reservations];
        this.loading = false;
        this.loadResources();
      },
      error: (err) => {
        this.lastMessage = {
          type: 'error',
          text: err.error?.message || 'خطا در ثبت رزرو',
        };
        this.loading = false;
      },
    });
  }

  cancelReservation(id: number) {
    this.reservationService.cancelReservation(id).subscribe({
      next: () => {
        this.snackBar.open('رزرو لغو شد', 'بستن', { duration: 3000 });
        this.reservations = this.reservations.map((r) =>
          r.id === id ? { ...r, status: 'Cancelled' } : r,
        );
      },
      error: (err) => this.showError(err.error?.message || 'خطا در لغو رزرو'),
    });
  }

  private showError(msg: string) {
    this.snackBar.open(msg, 'بستن', { duration: 4000 });
  }
  // Add these methods inside the AppComponent class

  lastMessage: { type: 'success' | 'error'; text: string } | null = null;

  getTotalFreeSlots(): number {
    return this.resources.reduce((sum, r) => sum + r.availableSlots.length, 0);
  }

  getActiveReservations(): number {
    return this.reservations.filter((r) => r.status === 'Active').length;
  }

  getResourceIcon(type: string): string {
    const icons: Record<string, string> = {
      'اتاق جلسه': '🏢',
      پروژکتور: '📽️',
      خودرو: '🚗',
    };
    return icons[type] ?? '📦';
  }

  // clicking a slot auto-fills the form
  fillSlot(resourceId: number, slot: { startTime: string; endTime: string }) {
    this.selectedResourceId = resourceId;
    this.startTime = this.toJalali(slot.startTime);
    this.endTime = this.toJalali(slot.endTime);
  }
}
