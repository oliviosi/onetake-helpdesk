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
import { PasswordModule } from 'primeng/password';

import {
    Usuario,
    UsuarioCreate,
    UsuarioUpdate
} from '../../core/models/usuario.model';
import { UsuariosService } from '../../core/services/usuarios.service';

@Component({
    selector: 'app-usuarios',
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
        PasswordModule
    ],
    templateUrl: './usuarios.component.html',
    styleUrl: './usuarios.component.css'
})
export class UsuariosComponent implements OnInit {
    usuarios: Usuario[] = [];

    carregando = false;
    salvando = false;
    erro = '';
    sucesso = '';

    modalVisivel = false;

    usuarioEditando: Usuario | null = null;

    form = {
        nomeUsuario: '',
        senha: '',
        nomeCompleto: '',
        email: '',
        ehAdm: 'N',
        ativo: 'S'
    };

    constructor(private usuariosService: UsuariosService) { }

    ngOnInit(): void {
        this.carregarUsuarios();
    }

    carregarUsuarios(): void {
        this.carregando = true;
        this.erro = '';
        this.sucesso = '';

        this.usuariosService.listar().subscribe({
            next: (response) => {
                this.usuarios = response.dados;
                this.carregando = false;
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os usuários.';
                this.carregando = false;
            }
        });
    }

    abrirNovo(): void {
        this.erro = '';
        this.sucesso = '';
        this.usuarioEditando = null;

        this.form = {
            nomeUsuario: '',
            senha: '',
            nomeCompleto: '',
            email: '',
            ehAdm: 'N',
            ativo: 'S'
        };

        this.modalVisivel = true;
    }

    abrirEdicao(usuario: Usuario): void {
        this.erro = '';
        this.sucesso = '';
        this.usuarioEditando = usuario;

        this.form = {
            nomeUsuario: usuario.nomeUsuario,
            senha: '',
            nomeCompleto: usuario.nomeCompleto ?? '',
            email: usuario.email ?? '',
            ehAdm: usuario.ehAdm === 'S' ? 'S' : 'N',
            ativo: usuario.ativo === 'N' ? 'N' : 'S'
        };

        this.modalVisivel = true;
    }

    salvar(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.form.nomeUsuario.trim()) {
            this.erro = 'Informe o nome do usuário.';
            return;
        }

        if (!this.usuarioEditando && !this.form.senha.trim()) {
            this.erro = 'Informe a senha.';
            return;
        }

        this.salvando = true;

        if (this.usuarioEditando) {
            const request: UsuarioUpdate = {
                nomeUsuario: this.form.nomeUsuario.trim(),
                nomeCompleto: this.form.nomeCompleto?.trim() || null,
                email: this.form.email?.trim() || null,
                ehAdm: this.form.ehAdm === 'S' ? 'S' : 'N',
                ativo: this.form.ativo === 'N' ? 'N' : 'S'
            };

            this.usuariosService.atualizar(this.usuarioEditando.idUsuario, request).subscribe({
                next: (response) => {
                    this.salvando = false;
                    this.modalVisivel = false;
                    this.sucesso = response.messages?.[0] ?? 'Usuário atualizado com sucesso.';
                    this.carregarUsuarios();
                },
                error: (error) => {
                    this.salvando = false;
                    this.erro = error?.error?.messages?.[0] ?? 'Não foi possível atualizar o usuário.';
                }
            });

            return;
        }

        const request: UsuarioCreate = {
            nomeUsuario: this.form.nomeUsuario.trim(),
            senha: this.form.senha.trim(),
            nomeCompleto: this.form.nomeCompleto?.trim() || null,
            email: this.form.email?.trim() || null,
            ehAdm: this.form.ehAdm === 'S' ? 'S' : 'N'
        };

        this.usuariosService.criar(request).subscribe({
            next: (response) => {
                this.salvando = false;
                this.modalVisivel = false;
                this.sucesso = response.messages?.[0] ?? 'Usuário criado com sucesso.';
                this.carregarUsuarios();
            },
            error: (error) => {
                this.salvando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível criar o usuário.';
            }
        });
    }

    ativar(usuario: Usuario): void {
        this.erro = '';
        this.sucesso = '';

        this.usuariosService.ativar(usuario.idUsuario).subscribe({
            next: (response) => {
                this.sucesso = response.messages?.[0] ?? 'Usuário ativado com sucesso.';
                this.carregarUsuarios();
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível ativar o usuário.';
            }
        });
    }

    desativar(usuario: Usuario): void {
        const confirmar = confirm(`Deseja realmente desativar o usuário "${usuario.nomeUsuario}"?`);

        if (!confirmar) {
            return;
        }

        this.erro = '';
        this.sucesso = '';

        this.usuariosService.desativar(usuario.idUsuario).subscribe({
            next: (response) => {
                this.sucesso = response.messages?.[0] ?? 'Usuário desativado com sucesso.';
                this.carregarUsuarios();
            },
            error: (error) => {
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível desativar o usuário.';
            }
        });
    }

    statusSeverity(ativo: string): 'success' | 'danger' {
        return ativo === 'S' ? 'success' : 'danger';
    }

    statusLabel(ativo: string): string {
        return ativo === 'S' ? 'Ativo' : 'Inativo';
    }

    admSeverity(ehAdm: string): 'info' | 'secondary' {
        return ehAdm === 'S' ? 'info' : 'secondary';
    }

    admLabel(ehAdm: string): string {
        return ehAdm === 'S' ? 'Admin' : 'Usuário';
    }
}