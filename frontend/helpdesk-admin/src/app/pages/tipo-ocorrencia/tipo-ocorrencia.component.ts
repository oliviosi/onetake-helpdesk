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

import {
    TipoOcorrencia,
    TipoOcorrenciaCreate,
    TipoOcorrenciaUpdate
} from '../../core/models/tipo-ocorrencia.model';
import { TipoOcorrenciaService } from '../../core/services/tipo-ocorrencia.service';

@Component({
    selector: 'app-tipo-ocorrencia',
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
    templateUrl: './tipo-ocorrencia.component.html',
    styleUrl: './tipo-ocorrencia.component.css'
})
export class TipoOcorrenciaComponent implements OnInit {
    tipos: TipoOcorrencia[] = [];

    carregando = false;
    salvando = false;
    erro = '';
    sucesso = '';

    modalVisivel = false;
    tipoEditando: TipoOcorrencia | null = null;

    form = {
        dscTipoOcorrencia: '',
        ativo: 'S'
    };

    constructor(private tipoOcorrenciaService: TipoOcorrenciaService) { }

    ngOnInit(): void {
        this.carregarTipos();
    }

    carregarTipos(): void {
        this.carregando = true;
        this.erro = '';
        this.sucesso = '';

        this.tipoOcorrenciaService.listar().subscribe({
            next: (response) => {
                this.tipos = response.dados;
                this.carregando = false;
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os tipos de ocorrência.';
                this.carregando = false;
            }
        });
    }

    abrirNovo(): void {
        this.erro = '';
        this.sucesso = '';
        this.tipoEditando = null;

        this.form = {
            dscTipoOcorrencia: '',
            ativo: 'S'
        };

        this.modalVisivel = true;
    }

    abrirEdicao(tipo: TipoOcorrencia): void {
        this.erro = '';
        this.sucesso = '';
        this.tipoEditando = tipo;

        this.form = {
            dscTipoOcorrencia: tipo.dscTipoOcorrencia,
            ativo: tipo.ativo
        };

        this.modalVisivel = true;
    }

    salvar(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.form.dscTipoOcorrencia.trim()) {
            this.erro = 'Informe a descrição do tipo de ocorrência.';
            return;
        }

        this.salvando = true;

        if (this.tipoEditando) {
            const request: TipoOcorrenciaUpdate = {
                dscTipoOcorrencia: this.form.dscTipoOcorrencia.trim(),
                ativo: this.form.ativo === 'N' ? 'N' : 'S'
            };

            this.tipoOcorrenciaService.atualizar(this.tipoEditando.idTipoOcorrencia, request).subscribe({
                next: (response) => {
                    this.salvando = false;
                    this.modalVisivel = false;
                    this.sucesso = response.messages?.[0] ?? 'Tipo de ocorrência atualizado com sucesso.';
                    this.carregarTipos();
                },
                error: (error) => {
                    this.salvando = false;
                    this.erro = error?.error?.messages?.[0] ?? 'Não foi possível atualizar o tipo de ocorrência.';
                }
            });

            return;
        }

        const request: TipoOcorrenciaCreate = {
            dscTipoOcorrencia: this.form.dscTipoOcorrencia.trim()
        };

        this.tipoOcorrenciaService.criar(request).subscribe({
            next: (response) => {
                this.salvando = false;
                this.modalVisivel = false;
                this.sucesso = response.messages?.[0] ?? 'Tipo de ocorrência criado com sucesso.';
                this.carregarTipos();
            },
            error: (error) => {
                this.salvando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível criar o tipo de ocorrência.';
            }
        });
    }

    ativar(tipo: TipoOcorrencia): void {
        this.erro = '';
        this.sucesso = '';

        this.tipoOcorrenciaService.ativar(tipo.idTipoOcorrencia).subscribe({
            next: (response) => {
                this.sucesso = response.messages?.[0] ?? 'Tipo de ocorrência ativado com sucesso.';
                this.carregarTipos();
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível ativar o tipo de ocorrência.';
            }
        });
    }

    desativar(tipo: TipoOcorrencia): void {
        this.erro = '';
        this.sucesso = '';

        this.tipoOcorrenciaService.desativar(tipo.idTipoOcorrencia).subscribe({
            next: (response) => {
                this.sucesso = response.messages?.[0] ?? 'Tipo de ocorrência desativado com sucesso.';
                this.carregarTipos();
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível desativar o tipo de ocorrência.';
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