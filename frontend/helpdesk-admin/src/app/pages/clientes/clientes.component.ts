import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageModule } from 'primeng/message';

import { Cliente } from '../../core/models/cliente.model';
import { ClientesService } from '../../core/services/clientes.service';

@Component({
  selector: 'app-clientes',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TagModule,
    ProgressSpinnerModule,
    MessageModule
  ],
  templateUrl: './clientes.component.html',
  styleUrl: './clientes.component.css'
})
export class ClientesComponent implements OnInit {
  clientes: Cliente[] = [];
  carregando = false;
  erro = '';

  constructor(private clientesService: ClientesService) {}

  ngOnInit(): void {
    this.carregarClientes();
  }

  carregarClientes(): void {
    this.carregando = true;
    this.erro = '';

    this.clientesService.listar().subscribe({
      next: (response) => {
        this.clientes = response.dados;
        this.carregando = false;
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os clientes.';
        this.carregando = false;
      }
    });
  }

  statusSeverity(ativo: string): 'success' | 'danger' {
    return ativo === 'S' ? 'success' : 'danger';
  }

  statusLabel(ativo: string): string {
    return ativo === 'S' ? 'Ativo' : 'Inativo';
  }
}