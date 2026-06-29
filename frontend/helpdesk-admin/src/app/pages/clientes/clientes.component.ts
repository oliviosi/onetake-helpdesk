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

import { Cliente, ClienteCreate, ClienteUpdate } from '../../core/models/cliente.model';
import { ClientesService } from '../../core/services/clientes.service';

@Component({
    selector: 'app-clientes',
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
        InputTextModule
    ],
    templateUrl: './clientes.component.html',
    styleUrl: './clientes.component.css'
})
export class ClientesComponent implements OnInit {
    clientes: Cliente[] = [];

    carregando = false;
    salvando = false;
    erro = '';
    sucesso = '';

    modalVisivel = false;
    clienteEditando: Cliente | null = null;

    form = {
        dscCliente: '',
        email: '',
        codClienteERP: '',
        ativo: 'S'
    };

    constructor(private clientesService: ClientesService) { }

    ngOnInit(): void {
        this.carregarClientes();
    }

    carregarClientes(): void {
        this.carregando = true;
        this.erro = '';
        this.sucesso = '';

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

    abrirNovo(): void {
        this.erro = '';
        this.sucesso = '';
        this.clienteEditando = null;

        this.form = {
            dscCliente: '',
            email: '',
            codClienteERP: '',
            ativo: 'S'
        };

        this.modalVisivel = true;
    }

    abrirEdicao(cliente: Cliente): void {
        this.erro = '';
        this.sucesso = '';
        this.clienteEditando = cliente;

        this.form = {
            dscCliente: cliente.dscCliente,
            email: cliente.email ?? '',
            codClienteERP: cliente.codClienteERP ?? '',
            ativo: cliente.ativo
        };

        this.modalVisivel = true;
    }

    salvar(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.form.dscCliente.trim()) {
            this.erro = 'Informe o nome do cliente.';
            return;
        }

        this.salvando = true;

        if (this.clienteEditando) {
            const request: ClienteUpdate = {
                dscCliente: this.form.dscCliente.trim(),
                email: this.form.email?.trim() || undefined,
                codClienteERP: this.form.codClienteERP?.trim() || undefined,
                ativo: this.form.ativo === 'N' ? 'N' : 'S'
            };

            this.clientesService.atualizar(this.clienteEditando.idCliente, request).subscribe({
                next: (response) => {
                    this.salvando = false;
                    this.modalVisivel = false;
                    this.sucesso = response.messages?.[0] ?? 'Cliente atualizado com sucesso.';
                    this.carregarClientes();
                },
                error: (error) => {
                    this.salvando = false;
                    this.erro = error?.error?.messages?.[0] ?? 'Não foi possível atualizar o cliente.';
                }
            });

            return;
        }

        const request: ClienteCreate = {
            dscCliente: this.form.dscCliente.trim(),
            email: this.form.email?.trim() || undefined,
            codClienteERP: this.form.codClienteERP?.trim() || undefined
        };

        this.clientesService.criar(request).subscribe({
            next: (response) => {
                this.salvando = false;
                this.modalVisivel = false;
                this.sucesso = response.messages?.[0] ?? 'Cliente criado com sucesso.';
                this.carregarClientes();
            },
            error: (error) => {
                this.salvando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível criar o cliente.';
            }
        });
    }

    ativar(cliente: Cliente): void {
        this.erro = '';
        this.sucesso = '';

        this.clientesService.ativar(cliente.idCliente).subscribe({
            next: (response) => {
                this.sucesso = response.messages?.[0] ?? 'Cliente ativado com sucesso.';
                this.carregarClientes();
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível ativar o cliente.';
            }
        });
    }

    desativar(cliente: Cliente): void {
        this.erro = '';
        this.sucesso = '';

        this.clientesService.desativar(cliente.idCliente).subscribe({
            next: (response) => {
                this.sucesso = response.messages?.[0] ?? 'Cliente desativado com sucesso.';
                this.carregarClientes();
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível desativar o cliente.';
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