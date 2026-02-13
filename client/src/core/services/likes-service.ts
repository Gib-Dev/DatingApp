import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from '../../types/member';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:5001/api/';

  toggleLike(userId: string): Observable<void> {
    return this.http.post<void>(this.baseUrl + `likes/${userId}`, {});
  }

  getLikes(predicate: 'liked' | 'likedBy' | 'mutual'): Observable<Member[]> {
    return this.http.get<Member[]>(this.baseUrl + `likes?predicate=${predicate}`);
  }
}
