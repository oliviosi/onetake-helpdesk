import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageModule } from 'primeng/message';

import { TicketResumo } from '../../core/models/ticket.model';
import { TicketsService } from '../../core/services/tickets.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    TagModule,
    ProgressSpinnerModule,
    MessageModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  tickets: TicketResumo[] = [];

  carregando = false;
  erro = '';

  totalTickets = 0;
  totalAbertos = 0;
  totalAndamento = 0;
  totalFechados = 0;
  totalCancelados = 0;

  constructor(private ticketsService: TicketsService) { }

  ngOnInit(): void {
    this.carregarDashboard();
  }

  carregarDashboard(): void {
    this.carregando = true;
    this.erro = '';

    this.ticketsService.listar().subscribe({
      next: (response) => {
        this.tickets = response.dados;
        this.calcularIndicadores();
        this.carregando = false;
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar o dashboard.';
        this.carregando = false;
      }
    });
  }

  calcularIndicadores(): void {
    this.totalTickets = this.tickets.length;

    this.totalCancelados = this.tickets.filter(t =>
      t.ticketCancelado === 'S'
    ).length;

    this.totalFechados = this.tickets.filter(t =>
      t.ticketCancelado !== 'S' && !!t.dtHrFinalizacao
    ).length;

    this.totalAndamento = this.tickets.filter(t => {
      const status = t.dscStatusTicket?.toLowerCase() ?? '';

      return (
        t.ticketCancelado !== 'S' &&
        !t.dtHrFinalizacao &&
        (status.includes('andamento') || status.includes('atendimento'))
      );
    }).length;

    this.totalAbertos = this.tickets.filter(t => {
      const status = t.dscStatusTicket?.toLowerCase() ?? '';

      return (
        t.ticketCancelado !== 'S' &&
        !t.dtHrFinalizacao &&
        !status.includes('andamento') &&
        !status.includes('atendimento')
      );
    }).length;
  }

  ultimosTickets(): TicketResumo[] {
    return this.tickets.slice(0, 5);
  }

  formatarData(data: string | null | undefined): string {
    if (!data) {
      return '-';
    }

    return new Date(data).toLocaleString('pt-BR');
  }

  classeStatus(ticket: TicketResumo): string {
    if (ticket.ticketCancelado === 'S') {
      return 'status-cancelado';
    }

    if (ticket.dtHrFinalizacao) {
      return 'status-fechado';
    }

    const status = ticket.dscStatusTicket?.toLowerCase() ?? '';

    if (status.includes('andamento') || status.includes('atendimento')) {
      return 'status-andamento';
    }

    if (status.includes('cliente')) {
      return 'status-cliente';
    }

    if (status.includes('fechado')) {
      return 'status-fechado';
    }

    if (status.includes('cancelado')) {
      return 'status-cancelado';
    }

    return 'status-aberto';
  }

  labelStatus(ticket: TicketResumo): string {
    if (ticket.ticketCancelado === 'S') {
      return 'Cancelado';
    }

    return ticket.dscStatusTicket || 'Aberto';
  }

  severityPrioridade(prioridade: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' {
    switch ((prioridade || '').toLowerCase()) {
      case 'baixa':
        return 'success';
      case 'normal':
        return 'info';
      case 'média':
      case 'media':
        return 'warning';
      case 'alta':
      case 'urgente':
        return 'danger';
      default:
        return 'secondary';
    }
  }
}