import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MemberService } from '../../../core/services/member-service';
import { ToastService } from '../../../core/services/toast-service';
import { Photo } from '../../../types/member';

@Component({
  selector: 'app-photo-upload',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './photo-upload.html',
  styleUrl: './photo-upload.css'
})
export class PhotoUpload {
  photos = signal<Photo[]>([]);
  uploading = signal(false);
  selectedFile = signal<File | null>(null);
  previewUrl = signal<string | null>(null);

  private memberService = inject(MemberService);
  private toastService = inject(ToastService);

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];

      // Validate file type
      if (!file.type.startsWith('image/')) {
        this.toastService.error('Please select an image file');
        return;
      }

      // Validate file size (10MB)
      if (file.size > 10 * 1024 * 1024) {
        this.toastService.error('File size cannot exceed 10MB');
        return;
      }

      this.selectedFile.set(file);

      // Create preview
      const reader = new FileReader();
      reader.onload = (e) => {
        this.previewUrl.set(e.target?.result as string);
      };
      reader.readAsDataURL(file);
    }
  }

  uploadPhoto(): void {
    const file = this.selectedFile();
    if (!file) return;

    this.uploading.set(true);

    this.memberService.addPhoto(file).subscribe({
      next: (photo) => {
        this.photos.update(current => [...current, photo]);
        this.toastService.success('Photo uploaded successfully!');
        this.clearSelection();
        this.uploading.set(false);
      },
      error: (err) => {
        this.toastService.error('Failed to upload photo');
        console.error('Upload error:', err);
        this.uploading.set(false);
      }
    });
  }

  setMainPhoto(photoId: number): void {
    this.memberService.setMainPhoto(photoId).subscribe({
      next: () => {
        this.toastService.success('Main photo updated!');
        // Optionally reload photos or update local state
      },
      error: (err) => {
        this.toastService.error('Failed to set main photo');
        console.error('Error:', err);
      }
    });
  }

  deletePhoto(photoId: number): void {
    if (!confirm('Are you sure you want to delete this photo?')) {
      return;
    }

    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        this.photos.update(current => current.filter(p => p.id !== photoId));
        this.toastService.success('Photo deleted successfully!');
      },
      error: (err) => {
        this.toastService.error('Failed to delete photo');
        console.error('Error:', err);
      }
    });
  }

  clearSelection(): void {
    this.selectedFile.set(null);
    this.previewUrl.set(null);
  }

  loadPhotos(photos: Photo[]): void {
    this.photos.set(photos);
  }
}
