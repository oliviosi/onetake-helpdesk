import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, UsuarioLogado } from '../models/auth.model';
import { ResponseModel } from '../models/response.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly tokenKey = 'helpdesk_token';
  private readonly usuarioKey = 'helpdesk_usuario';

  private usuarioSignal = signal<UsuarioLogado | null>(this.carregarUsuarioStorage());

  usuarioLogado = computed(() => this.usuarioSignal());
  autenticado = computed(() => !!this.obterToken());

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  login(request: LoginRequest): Observable<ResponseModel<LoginResponse>> {
    return this.http
      .post<ResponseModel<LoginResponse>>(`${environment.apiUrl}/Auth/login`, request)
      .pipe(
        tap((response) => {
          localStorage.setItem(this.tokenKey, response.dados.token);
          localStorage.setItem(this.usuarioKey, JSON.stringify(response.dados.usuario));
          this.usuarioSignal.set(response.dados.usuario);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.usuarioKey);
    this.usuarioSignal.set(null);
    this.router.navigate(['/login']);
  }

  obterToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private carregarUsuarioStorage(): UsuarioLogado | null {
    const usuario = localStorage.getItem(this.usuarioKey);

    if (!usuario) {
      return null;
    }

    try {
      return JSON.parse(usuario) as UsuarioLogado;
    } catch {
      localStorage.removeItem(this.usuarioKey);
      return null;
    }
  }
}