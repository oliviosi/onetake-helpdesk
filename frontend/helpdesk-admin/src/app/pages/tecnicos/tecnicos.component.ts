import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageModule } from 'primeng/message';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';

import { Tecnico, TecnicoCreate, TecnicoUpdate } from '../../core/models/tecnico.model';
import { TecnicosService } from '../../core/services/tecnicos.service';
import { Combo } from '../../core/models/combo.model';
import { CombosService } from '../../core/services/combos.service';

@Component({
    selector: 'app-tecnicos',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        TableModule,
        ButtonModule,
        ProgressSpinnerModule,
        MessageModule,
        DialogModule,
        InputTextModule,
        TagModule,
        SelectModule
    ],
    templateUrl: './tecnicos.component.html',
    styleUrl: './tecnicos.component.css'
})
export class TecnicosComponent implements OnInit {
    tecnicos: Tecnico[] = [];
    usuarios: Combo[] = [];

    carregando = false;
    salvando = false;
    carregandoUsuarios = false;

    erro = '';
    sucesso = '';

    modalVisivel = false;
    tecnicoEditando: Tecnico | null = null;

    form = {
        nome: '',
        notificarNovosChamados: 'S',
        idUsuario: null as number | null
    };

    constructor(
        private tecnicosService: TecnicosService,
        private combosService: CombosService
    ) { }

    ngOnInit(): void {
        this.carregarUsuarios();
        this.carregarTecnicos();
    }

    carregarTecnicos(): void {
        this.carregando = true;
        this.erro = '';
        this.sucesso = '';

        this.tecnicosService.listar().subscribe({
            next: (response) => {
                this.tecnicos = response.dados;
                this.carregando = false;
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os técnicos.';
                this.carregando = false;
            }
        });
    }

    carregarUsuarios(): void {
        this.carregandoUsuarios = true;

        this.combosService.usuarios().subscribe({
            next: (response) => {
                this.usuarios = response.dados;
                this.carregandoUsuarios = false;
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os usuários.';
                this.carregandoUsuarios = false;
            }
        });
    }

    abrirNovo(): void {
        this.erro = '';
        this.sucesso = '';
        this.tecnicoEditando = null;

        this.form = {
            nome: '',
            notificarNovosChamados: 'S',
            idUsuario: null
        };

        this.modalVisivel = true;
    }

    abrirEdicao(tecnico: Tecnico): void {
        this.erro = '';
        this.sucesso = '';
        this.tecnicoEditando = tecnico;

        this.form = {
            nome: tecnico.nome,
            notificarNovosChamados: tecnico.notificarNovosChamados === 'S' ? 'S' : 'N',
            idUsuario: tecnico.idUsuario ?? null
        };

        this.modalVisivel = true;
    }

    salvar(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.form.nome.trim()) {
            this.erro = 'Informe o nome do técnico.';
            return;
        }

        this.salvando = true;

        if (this.tecnicoEditando) {
            const request: TecnicoUpdate = {
                nome: this.form.nome.trim(),
                notificarNovosChamados: this.form.notificarNovosChamados === 'S' ? 'S' : 'N',
                idUsuario: this.form.idUsuario
            };

            this.tecnicosService.atualizar(this.tecnicoEditando.idTecnico, request).subscribe({
                next: (response) => {
                    this.salvando = false;
                    this.modalVisivel = false;
                    this.sucesso = response.messages?.[0] ?? 'Técnico atualizado com sucesso.';
                    this.carregarTecnicos();
                },
                error: (error) => {
                    this.salvando = false;
                    this.erro = error?.error?.messages?.[0] ?? 'Não foi possível atualizar o técnico.';
                }
            });

            return;
        }

        const request: TecnicoCreate = {
            nome: this.form.nome.trim(),
            notificarNovosChamados: this.form.notificarNovosChamados === 'S' ? 'S' : 'N',
            idUsuario: this.form.idUsuario
        };

        this.tecnicosService.criar(request).subscribe({
            next: (response) => {
                this.salvando = false;
                this.modalVisivel = false;
                this.sucesso = response.messages?.[0] ?? 'Técnico criado com sucesso.';
                this.carregarTecnicos();
            },
            error: (error) => {
                this.salvando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível criar o técnico.';
            }
        });
    }

    excluir(tecnico: Tecnico): void {
        const confirmar = confirm(`Deseja realmente excluir o técnico "${tecnico.nome}"?`);

        if (!confirmar) {
            return;
        }

        this.erro = '';
        this.sucesso = '';

        this.tecnicosService.excluir(tecnico.idTecnico).subscribe({
            next: (response) => {
                this.sucesso = response.messages?.[0] ?? 'Técnico excluído com sucesso.';
                this.carregarTecnicos();
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível excluir o técnico.';
            }
        });
    }

    labelNotificacao(valor: string): string {
        return valor === 'S' ? 'Sim' : 'Não';
    }

    severityNotificacao(valor: string): 'success' | 'secondary' {
        return valor === 'S' ? 'success' : 'secondary';
    }
}