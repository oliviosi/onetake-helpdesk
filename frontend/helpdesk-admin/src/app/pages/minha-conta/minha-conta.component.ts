import { Component, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';

import { AuthService } from '../../core/services/auth.service';
import { MinhaContaService } from '../../core/services/minha-conta.service';

@Component({
    selector: 'app-minha-conta',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ButtonModule,
        MessageModule,
        PasswordModule
    ],
    templateUrl: './minha-conta.component.html',
    styleUrl: './minha-conta.component.css'
})
export class MinhaContaComponent {
    usuario = computed(() => this.authService.usuarioLogado());

    carregando = false;
    erro = '';
    sucesso = '';

    form = {
        senhaAtual: '',
        novaSenha: '',
        confirmarNovaSenha: ''
    };

    constructor(
        private authService: AuthService,
        private minhaContaService: MinhaContaService
    ) { }

    alterarSenha(): void {
        this.erro = '';
        this.sucesso = '';

        if (!this.form.senhaAtual.trim()) {
            this.erro = 'Informe a senha atual.';
            return;
        }

        if (!this.form.novaSenha.trim()) {
            this.erro = 'Informe a nova senha.';
            return;
        }

        if (this.form.novaSenha.trim().length < 6) {
            this.erro = 'A nova senha deve ter pelo menos 6 caracteres.';
            return;
        }

        if (this.form.novaSenha !== this.form.confirmarNovaSenha) {
            this.erro = 'A confirmação da senha não confere.';
            return;
        }

        this.carregando = true;

        this.minhaContaService.alterarSenha({
            senhaAtual: this.form.senhaAtual,
            novaSenha: this.form.novaSenha
        }).subscribe({
            next: (response) => {
                this.carregando = false;
                this.sucesso = response.messages?.[0] ?? 'Senha alterada com sucesso.';

                this.form = {
                    senhaAtual: '',
                    novaSenha: '',
                    confirmarNovaSenha: ''
                };
            },
            error: (error) => {
                this.carregando = false;
                this.erro = error?.error?.messages?.[0] ?? 'Não foi possível alterar a senha.';
            }
        });
    }
}