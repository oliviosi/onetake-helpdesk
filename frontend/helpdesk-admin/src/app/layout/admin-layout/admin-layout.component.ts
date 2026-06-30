import { Component, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { ButtonModule } from 'primeng/button';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    ButtonModule
  ],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent {
  usuario = computed(() => this.authService.usuarioLogado());

  menu = [
    { label: 'Dashboard', icon: 'pi pi-home', route: '/dashboard' },
    { label: 'Tickets', icon: 'pi pi-ticket', route: '/tickets' },
    { label: 'Clientes', icon: 'pi pi-building', route: '/clientes' },
    { label: 'Usuários', icon: 'pi pi-user', route: '/usuarios' },
    { label: 'Produtos', icon: 'pi pi-box', route: '/produtos' },
    { label: 'Produtos por Cliente', icon: 'pi pi-link', route: '/produto-x-cliente' },
    { label: 'Tipos de Ocorrência', icon: 'pi pi-tags', route: '/tipo-ocorrencia' },
    { label: 'Status', icon: 'pi pi-check-circle', route: '/status-ticket' },
    { label: 'Técnicos', icon: 'pi pi-users', route: '/tecnicos' },
    { label: 'Minha Conta', icon: 'pi pi-id-card', route: '/minha-conta' }
  ];

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  sair(): void {
    this.authService.logout();
  }
}