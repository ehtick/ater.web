import { Injectable } from "@angular/core";

const AdminUser = "AdminUser";

@Injectable({ providedIn: 'root' })
export class LoginStateService {
  isLogin = false;
  isAdmin = false;
  userName?: string | null = null;
  id?: string | null = null;
  constructor() {
    this.updateUserLoginState();
  }

  saveLoginState(data: any): void {
    this.isLogin = true;
    this.userName = data.username;
    if (data.roles.includes(AdminUser)) {
      this.isAdmin = true;
    }
    localStorage.setItem("id", data.id);
    localStorage.setItem("role", data.roles.join(';'));
    localStorage.setItem("username", data.username);
    localStorage.setItem("accessToken", data.token);
  }

  updateUserLoginState(): void {
    const userId = localStorage.getItem('id');
    const username = localStorage.getItem('username');
    const token = localStorage.getItem('accessToken');
    const role = localStorage.getItem('role');
    if (userId && token && username) {
      this.userName = username;
      this.isLogin = true;
      var roles = role?.split(';');
      if (roles && roles.includes(AdminUser)) {
        this.isAdmin = true;
      }

    } else {
      this.isLogin = false;
    }
  }
  logout(): void {
    localStorage.clear();
    this.isLogin = false;
  }
}
