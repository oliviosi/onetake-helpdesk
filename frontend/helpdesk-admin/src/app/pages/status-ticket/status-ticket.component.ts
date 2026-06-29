import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageModule } from 'primeng/message';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';

import {
  StatusTicket,
  StatusTicketCreate,
  StatusTicketUpdate
} from '../../core/models/status-ticket.model';
import { StatusTicketService } from '../../core/services/status-ticket.service';

@Component({
  selector: 'app-status-ticket',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    TagModule,
    ProgressSpinnerModule,
    MessageModule,
    DialogModule,
    InputTextModule,
    SelectModule
  ],
  templateUrl: './status-ticket.component.html',
  styleUrl: './status-ticket.component.css'
})
export class StatusTicketComponent implements OnInit {
  status: StatusTicket[] = [];

  carregando = false;
  salvando = false;
  erro = '';
  sucesso = '';

  modalVisivel = false;
  statusEditando: StatusTicket | null = null;

  tiposTicket = [
    { label: 'Aberto', value: 'ABERTO' },
    { label: 'Em andamento', value: 'ANDAMENTO' },
    { label: 'Aguardando cliente', value: 'CLIENTE' },
    { label: 'Fechado', value: 'FECHADO' },
    { label: 'Cancelado', value: 'CANCELADO' }
  ];

  form = {
    dscStatusTicket: '',
    tipoTicket: 'ABERTO',
  };

  constructor(private statusTicketService: StatusTicketService) { }

  ngOnInit(): void {
    this.carregarStatus();
  }

  carregarStatus(): void {
    this.carregando = true;
    this.erro = '';
    this.sucesso = '';

    this.statusTicketService.listar().subscribe({
      next: (response) => {
        this.status = response.dados;
        this.carregando = false;
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os status.';
        this.carregando = false;
      }
    });
  }

  abrirNovo(): void {
    this.erro = '';
    this.sucesso = '';
    this.statusEditando = null;

    this.form = {
      dscStatusTicket: '',
      tipoTicket: 'ABERTO',
    };

    this.modalVisivel = true;
  }

  abrirEdicao(item: StatusTicket): void {
    this.erro = '';
    this.sucesso = '';
    this.statusEditando = item;

    this.form = {
      dscStatusTicket: item.dscStatusTicket,
      tipoTicket: item.tipoTicket,
    };

    this.modalVisivel = true;
  }

  salvar(): void {
    this.erro = '';
    this.sucesso = '';

    if (!this.form.dscStatusTicket.trim()) {
      this.erro = 'Informe a descrição do status.';
      return;
    }

    if (!this.form.tipoTicket) {
      this.erro = 'Informe o tipo do ticket.';
      return;
    }

    this.salvando = true;

    if (this.statusEditando) {
      const request: StatusTicketUpdate = {
        dscStatusTicket: this.form.dscStatusTicket.trim(),
        tipoTicket: this.form.tipoTicket,
      };

      this.statusTicketService.atualizar(this.statusEditando.idStatusTicket, request).subscribe({
        next: (response) => {
          this.salvando = false;
          this.modalVisivel = false;
          this.sucesso = response.messages?.[0] ?? 'Status atualizado com sucesso.';
          this.carregarStatus();
        },
        error: (error) => {
          this.salvando = false;
          this.erro = error?.error?.messages?.[0] ?? 'Não foi possível atualizar o status.';
        }
      });

      return;
    }

    const request: StatusTicketCreate = {
      dscStatusTicket: this.form.dscStatusTicket.trim(),
      tipoTicket: this.form.tipoTicket,
    };

    this.statusTicketService.criar(request).subscribe({
      next: (response) => {
        this.salvando = false;
        this.modalVisivel = false;
        this.sucesso = response.messages?.[0] ?? 'Status criado com sucesso.';
        this.carregarStatus();
      },
      error: (error) => {
        this.salvando = false;
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível criar o status.';
      }
    });
  }

  excluir(item: StatusTicket): void {
    const confirmar = confirm(`Deseja realmente excluir o status "${item.dscStatusTicket}"?`);

    if (!confirmar) {
      return;
    }

    this.erro = '';
    this.sucesso = '';

    this.statusTicketService.excluir(item.idStatusTicket).subscribe({
      next: (response) => {
        this.sucesso = response.messages?.[0] ?? 'Status excluído com sucesso.';
        this.carregarStatus();
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível excluir o status.';
      }
    });
  }

  labelTipoTicket(tipoTicket: string): string {
    const item = this.tiposTicket.find(x => x.value === tipoTicket);
    return item?.label ?? tipoTicket;
  }

  severityTipoTicket(tipoTicket: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' {
    switch (tipoTicket) {
      case 'ABERTO':
        return 'info';
      case 'ANDAMENTO':
        return 'warning';
      case 'CLIENTE':
        return 'secondary';
      case 'FECHADO':
        return 'success';
      case 'CANCELADO':
        return 'danger';
      default:
        return 'secondary';
    }
  }

  classeTipoTicket(tipoTicket: string): string {
    switch (tipoTicket) {
      case 'ABERTO':
        return 'status-aberto';
      case 'ANDAMENTO':
        return 'status-andamento';
      case 'CLIENTE':
        return 'status-cliente';
      case 'FECHADO':
        return 'status-fechado';
      case 'CANCELADO':
        return 'status-cancelado';
      default:
        return 'status-padrao';
    }
  }
}