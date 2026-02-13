import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from '../../types/member';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:5001/api/';

  getMembers(): Observable<Member[]> {
    return this.http.get<Member[]>(this.baseUrl + 'members');
  }

  getMember(id: string): Observable<Member> {
    return this.http.get<Member>(this.baseUrl + 'members/' + id);
  }
}
