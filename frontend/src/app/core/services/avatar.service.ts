import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '@app/core/models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AvatarService {
  private apiUrl = 'http://localhost:5239/api/users';

  constructor(private http: HttpClient) {}

  uploadAvatar(userId: string, file: File): Observable<User> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<User>(`${this.apiUrl}/${userId}/avatar`, formData);
  }

  getAvatarUrl(avatarPath: string): string {
    if (!avatarPath) return '';
    if (avatarPath.startsWith('http')) return avatarPath;
    return `http://localhost:5239${avatarPath}`;
  }
}
