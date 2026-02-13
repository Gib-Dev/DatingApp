import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Member, Photo } from '../../types/member';
import { ENVIRONMENT } from '../tokens/environment.token';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  private env = inject(ENVIRONMENT);
  private baseUrl = this.env.apiUrl + '/';

  getMembers(): Observable<Member[]> {
    return this.http.get<Member[]>(this.baseUrl + 'members');
  }

  getMember(id: string): Observable<Member> {
    return this.http.get<Member>(this.baseUrl + 'members/' + id);
  }

  updateMember(updateData: { description?: string; city: string; country: string }): Observable<void> {
    return this.http.put<void>(this.baseUrl + 'members', updateData);
  }

  addPhoto(file: File): Observable<Photo> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + 'members/add-photo', formData);
  }

  setMainPhoto(photoId: number): Observable<void> {
    return this.http.put<void>(this.baseUrl + `members/set-main-photo/${photoId}`, {});
  }

  deletePhoto(photoId: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + `members/delete-photo/${photoId}`);
  }
}
