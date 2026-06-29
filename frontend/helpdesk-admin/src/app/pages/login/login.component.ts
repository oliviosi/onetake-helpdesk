import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { MessageModule } from 'primeng/message';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    PasswordModule,
    MessageModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  codigoAcesso = 'ONETAKE';
  usuario = 'admin';
  senha = '123456';

  carregando = false;
  erro = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  entrar(): void {
    this.erro = '';

    if (!this.codigoAcesso || !this.usuario || !this.senha) {
      this.erro = 'Informe código de acesso, usuário e senha.';
      return;
    }

    this.carregando = true;

    this.authService.login({
      codigoAcesso: this.codigoAcesso,
      usuario: this.usuario,
      senha: this.senha
    }).subscribe({
      next: () => {
        this.carregando = false;
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.carregando = false;
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível realizar o login.';
      }
    });
  }
}