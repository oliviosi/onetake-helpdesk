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
import { TextareaModule } from 'primeng/textarea';

import { Combo } from '../../core/models/combo.model';
import {
    TicketCreate,
    TicketDetalhe,
    TicketResponder,
    TicketResumo
} from '../../core/models/ticket.model';
import { Tecnico } from '../../core/models/tecnico.model';

import { CombosService } from '../../core/services/combos.service';
import { TicketsService } from '../../core/services/tickets.service';
import { TecnicosService } from '../../core/services/tecnicos.service';

@Component({
    selector: 'app-tickets',
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
        SelectModule,
        TextareaModule
    ],
    templateUrl: './tickets.component.html',
    styleUrl: './tickets.component.css'
})
export class TicketsComponent implements OnInit {
    tickets: TicketResumo[] = [];

    clientes: Combo[] = [];
    usuarios: Combo[] = [];
    tiposOcorrencia: Combo[] = [];
    tecnicos: Tecnico[] = [];

    ticketDetalhe: TicketDetalhe | null = null;

    carregando = false;
    carregandoCombos = false;
    salvando = false;
    carregandoDetalhe = false;

    erro = '';
    sucesso = '';

    modalNovoVisivel = false;
    modalDetalheVisivel = false;

    prioridades = [
        { label: 'Baixa', value: 'Baixa' },
        { label: 'Normal', value: 'Normal' },
        { label: 'Média', value: 'Média' },
        { label: 'Alta', value: 'Alta' },
        { label: 'Urgente', value: 'Urgente' }
    ];

    formNovo = {
        idCliente: null as number | null,
        idUsuario: null as number | null,
        assunto: '',
        idTipoOcorrencia: null as number | null,
        dscTicket: '',
        prioridade: 'Normal'
    };

    formResposta = {
        mensagem: '',
        idTecnico: null as number | null,
        privativo: 'N',
        aguardarInteracaoUsuario: 'N'
    };

    formFechamento = {
        idTecnico: null as number | null
    };

    constructor(
        private ticketsService: TicketsService,
        private combosService: CombosService,
        private tecnicosService: TecnicosService
    ) { }

    ngOnInit(): void {
        this.carregarCombos();
        this.carregarTecnicos();
        this.carregarTickets();
    }

    carregarTickets(): void {
        this.carregando = true;
        this.erro = '';
        this.sucesso = '';

        this.ticketsService.listar().subscribe({
            next: (response) => {
                this.tickets = response.dados;
                this.carregando = false;
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os tickets.';
                this.carregando = false;
            }
        });
    }

    carregarCombos(): void {
        this.carregandoCombos = true;

        this.combosService.clientes().subscribe({
            next: (response) => {
                this.clientes = response.dados;
            },
            error: () => {
                this.erro = 'Não foi possível carregar os clientes.';
            }
        });

        this.combosService.usuarios().subscribe({
            next: (response) => {
                this.usuarios = response.dados;
            },
            error: () => {
                this.erro = 'Não foi possível carregar os usuários.';
            }
        });

        this.combosService.tiposOcorrencia().subscribe({
            next: (response) => {
                this.tiposOcorrencia = response.dados;
                this.carregandoCombos = false;
            },
            error: () => {
                this.erro = 'Não foi possível carregar os tipos de ocorrência.';
                this.carregandoCombos = false;
            }
        });
    }

    carregarTecnicos(): void {
        this.tecnicosService.listar().subscribe({
            next: (response) => {
                this.tecnicos = response.dados;
            },
            error: () => {
                this.erro = 'Não foi possível carregar os técnicos.';
            }
        });
    }

    abrirNovo(): void {
        this.erro = '';
        this.sucesso = '';

        this.formNovo = {
            idCliente: null,
            idUsuario: null,
            assunto: '',
            idTipoOcorrencia: null,
            dscTicket: '',
            prioridade: 'Normal'
        };

        this.modalNovoVisivel = true;
    }

    salvarNovo(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.formNovo.idCliente) {
            this.erro = 'Informe o cliente.';
            return;
        }

        if (!this.formNovo.idUsuario) {
            this.erro = 'Informe o usuário solicitante.';
            return;
        }

        if (!this.formNovo.assunto.trim()) {
            this.erro = 'Informe o assunto.';
            return;
        }

        if (!this.formNovo.idTipoOcorrencia) {
            this.erro = 'Informe o tipo de ocorrência.';
            return;
        }

        if (!this.formNovo.dscTicket.trim()) {
            this.erro = 'Informe a descrição do ticket.';
            return;
        }

        const request: TicketCreate = {
            idCliente: this.formNovo.idCliente,
            idUsuario: this.formNovo.idUsuario,
            assunto: this.formNovo.assunto.trim(),
            idTipoOcorrencia: this.formNovo.idTipoOcorrencia,
            dscTicket: this.formNovo.dscTicket.trim(),
            prioridade: this.formNovo.prioridade
        };

        this.salvando = true;

        this.ticketsService.criar(request).subscribe({
            next: (response) => {
                this.salvando = false;
                this.modalNovoVisivel = false;
                this.sucesso = response.messages?.[0] ?? 'Ticket criado com sucesso.';
                this.carregarTickets();
            },
            error: (error) => {
                this.salvando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível criar o ticket.';
            }
        });
    }

    abrirDetalhe(ticket: TicketResumo): void {
        this.erro = '';
        this.sucesso = '';
        this.ticketDetalhe = null;
        this.modalDetalheVisivel = true;
        this.carregandoDetalhe = true;

        this.formResposta = {
            mensagem: '',
            idTecnico: null,
            privativo: 'N',
            aguardarInteracaoUsuario: 'N'
        };

        this.formFechamento = {
            idTecnico: null
        };

        this.ticketsService.buscarPorId(ticket.idTicket).subscribe({
            next: (response) => {
                this.ticketDetalhe = response.dados;
                this.carregandoDetalhe = false;
                this.formFechamento.idTecnico = this.ticketDetalhe.idTecnicoFinalizacao ?? null;
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar o detalhe do ticket.';
                this.carregandoDetalhe = false;
                this.modalDetalheVisivel = false;
            }
        });
    }

    responderTicket(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.ticketDetalhe) {
            return;
        }

        if (!this.formResposta.idTecnico) {
            this.erro = 'Informe o técnico responsável pela resposta.';
            return;
        }

        if (!this.formResposta.mensagem.trim()) {
            this.erro = 'Informe a mensagem da resposta.';
            return;
        }

        const request: TicketResponder = {
            mensagem: this.formResposta.mensagem.trim(),
            idTecnico: this.formResposta.idTecnico,
            idUsuario: null,
            privativo: this.formResposta.privativo === 'S' ? 'S' : 'N',
            aguardarInteracaoUsuario: this.formResposta.aguardarInteracaoUsuario === 'S' ? 'S' : 'N'
        };

        this.salvando = true;

        this.ticketsService.responder(this.ticketDetalhe.idTicket, request).subscribe({
            next: (response) => {
                this.salvando = false;
                this.sucesso = response.messages?.[0] ?? 'Resposta adicionada com sucesso.';

                this.formResposta = {
                    mensagem: '',
                    idTecnico: this.formResposta.idTecnico,
                    privativo: 'N',
                    aguardarInteracaoUsuario: 'N'
                };

                this.recarregarDetalhe();
                this.carregarTickets();
            },
            error: (error) => {
                this.salvando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível responder o ticket.';
            }
        });
    }

    fecharTicket(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.ticketDetalhe) {
            return;
        }

        if (!this.formFechamento.idTecnico) {
            this.erro = 'Informe o técnico responsável pelo fechamento.';
            return;
        }

        const confirmar = confirm(`Deseja realmente fechar o ticket #${this.ticketDetalhe.idTicket}?`);

        if (!confirmar) {
            return;
        }

        this.salvando = true;

        this.ticketsService.fechar(this.ticketDetalhe.idTicket, this.formFechamento.idTecnico).subscribe({
            next: (response) => {
                this.salvando = false;
                this.sucesso = response.messages?.[0] ?? 'Ticket fechado com sucesso.';
                this.recarregarDetalhe();
                this.carregarTickets();
            },
            error: (error) => {
                this.salvando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível fechar o ticket.';
            }
        });
    }

    cancelarTicket(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.ticketDetalhe) {
            return;
        }

        const confirmar = confirm(`Deseja realmente cancelar o ticket #${this.ticketDetalhe.idTicket}?`);

        if (!confirmar) {
            return;
        }

        this.salvando = true;

        this.ticketsService.cancelar(this.ticketDetalhe.idTicket).subscribe({
            next: (response) => {
                this.salvando = false;
                this.sucesso = response.messages?.[0] ?? 'Ticket cancelado com sucesso.';
                this.recarregarDetalhe();
                this.carregarTickets();
            },
            error: (error) => {
                this.salvando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível cancelar o ticket.';
            }
        });
    }

    recarregarDetalhe(): void {
        if (!this.ticketDetalhe) {
            return;
        }

        this.carregandoDetalhe = true;

        this.ticketsService.buscarPorId(this.ticketDetalhe.idTicket).subscribe({
            next: (response) => {
                this.ticketDetalhe = response.dados;
                this.carregandoDetalhe = false;
                this.formFechamento.idTecnico =
                    this.ticketDetalhe.idTecnicoFinalizacao ?? this.formFechamento.idTecnico;
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível recarregar o detalhe do ticket.';
                this.carregandoDetalhe = false;
            }
        });
    }

    ticketPodeReceberAcao(): boolean {
        if (!this.ticketDetalhe) {
            return false;
        }

        return this.ticketDetalhe.ticketCancelado !== 'S' && !this.ticketDetalhe.dtHrFinalizacao;
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