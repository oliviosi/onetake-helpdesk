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

import { Produto, ProdutoCreate, ProdutoUpdate } from '../../core/models/produto.model';
import { ProdutosService } from '../../core/services/produtos.service';

@Component({
  selector: 'app-produtos',
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
  templateUrl: './produtos.component.html',
  styleUrl: './produtos.component.css'
})
export class ProdutosComponent implements OnInit {
  produtos: Produto[] = [];

  carregando = false;
  salvando = false;
  erro = '';
  sucesso = '';

  modalVisivel = false;
  produtoEditando: Produto | null = null;

  form = {
    dscProduto: '',
    ativo: 'S'
  };

  constructor(private produtosService: ProdutosService) { }

  ngOnInit(): void {
    this.carregarProdutos();
  }

  carregarProdutos(): void {
    this.carregando = true;
    this.erro = '';
    this.sucesso = '';

    this.produtosService.listar().subscribe({
      next: (response) => {
        this.produtos = response.dados;
        this.carregando = false;
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os produtos.';
        this.carregando = false;
      }
    });
  }

  abrirNovo(): void {
    this.erro = '';
    this.sucesso = '';
    this.produtoEditando = null;

    this.form = {
      dscProduto: '',
      ativo: 'S'
    };

    this.modalVisivel = true;
  }

  abrirEdicao(produto: Produto): void {
    this.erro = '';
    this.sucesso = '';
    this.produtoEditando = produto;

    this.form = {
      dscProduto: produto.dscProduto,
      ativo: produto.ativo
    };

    this.modalVisivel = true;
  }

  salvar(): void {
    this.erro = '';
    this.sucesso = '';

    if (!this.form.dscProduto.trim()) {
      this.erro = 'Informe a descrição do produto.';
      return;
    }

    this.salvando = true;

    if (this.produtoEditando) {
      const request: ProdutoUpdate = {
        dscProduto: this.form.dscProduto.trim(),
        ativo: this.form.ativo === 'N' ? 'N' : 'S'
      };

      this.produtosService.atualizar(this.produtoEditando.idProduto, request).subscribe({
        next: (response) => {
          this.salvando = false;
          this.modalVisivel = false;
          this.sucesso = response.messages?.[0] ?? 'Produto atualizado com sucesso.';
          this.carregarProdutos();
        },
        error: (error) => {
          this.salvando = false;
          this.erro = error?.error?.messages?.[0] ?? 'Não foi possível atualizar o produto.';
        }
      });

      return;
    }

    const request: ProdutoCreate = {
      dscProduto: this.form.dscProduto.trim()
    };

    this.produtosService.criar(request).subscribe({
      next: (response) => {
        this.salvando = false;
        this.modalVisivel = false;
        this.sucesso = response.messages?.[0] ?? 'Produto criado com sucesso.';
        this.carregarProdutos();
      },
      error: (error) => {
        this.salvando = false;
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível criar o produto.';
      }
    });
  }

  ativar(produto: Produto): void {
    this.erro = '';
    this.sucesso = '';

    this.produtosService.ativar(produto.idProduto).subscribe({
      next: (response) => {
        this.sucesso = response.messages?.[0] ?? 'Produto ativado com sucesso.';
        this.carregarProdutos();
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível ativar o produto.';
      }
    });
  }

  desativar(produto: Produto): void {
    this.erro = '';
    this.sucesso = '';

    this.produtosService.desativar(produto.idProduto).subscribe({
      next: (response) => {
        this.sucesso = response.messages?.[0] ?? 'Produto desativado com sucesso.';
        this.carregarProdutos();
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível desativar o produto.';
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