import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateMessage, Message } from '../../types/message';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:5001/api/';

  sendMessage(message: CreateMessage): Observable<Message> {
    return this.http.post<Message>(this.baseUrl + 'messages', message);
  }

  getMessages(container: 'inbox' | 'outbox' | 'unread' = 'inbox'): Observable<Message[]> {
    return this.http.get<Message[]>(this.baseUrl + `messages?container=${container}`);
  }

  getMessageThread(userId: string): Observable<Message[]> {
    return this.http.get<Message[]>(this.baseUrl + `messages/thread/${userId}`);
  }

  deleteMessage(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + `messages/${id}`);
  }
}
