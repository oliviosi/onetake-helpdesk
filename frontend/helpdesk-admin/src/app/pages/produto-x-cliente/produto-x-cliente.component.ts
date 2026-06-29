import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SelectModule } from 'primeng/select';

import { Combo } from '../../core/models/combo.model';
import { ProdutoXCliente } from '../../core/models/produto-x-cliente.model';
import { CombosService } from '../../core/services/combos.service';
import { ProdutoXClienteService } from '../../core/services/produto-x-cliente.service';

@Component({
  selector: 'app-produto-x-cliente',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    MessageModule,
    ProgressSpinnerModule,
    SelectModule
  ],
  templateUrl: './produto-x-cliente.component.html',
  styleUrl: './produto-x-cliente.component.css'
})
export class ProdutoXClienteComponent implements OnInit {
  clientes: Combo[] = [];
  produtos: Combo[] = [];
  vinculos: ProdutoXCliente[] = [];

  idClienteSelecionado: number | null = null;
  idProdutoSelecionado: number | null = null;

  carregando = false;
  carregandoCombos = false;
  salvando = false;

  erro = '';
  sucesso = '';

  constructor(
    private combosService: CombosService,
    private produtoXClienteService: ProdutoXClienteService
  ) {}

  ngOnInit(): void {
    this.carregarCombos();
  }

  carregarCombos(): void {
    this.carregandoCombos = true;
    this.erro = '';

    this.combosService.clientes().subscribe({
      next: (response) => {
        this.clientes = response.dados;
        this.carregandoCombos = false;
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os clientes.';
        this.carregandoCombos = false;
      }
    });

    this.combosService.produtos().subscribe({
      next: (response) => {
        this.produtos = response.dados;
      },
      error: () => {
        this.erro = 'Não foi possível carregar os produtos.';
      }
    });
  }

  aoSelecionarCliente(): void {
    this.idProdutoSelecionado = null;
    this.vinculos = [];
    this.erro = '';
    this.sucesso = '';

    if (!this.idClienteSelecionado) {
      return;
    }

    this.carregarVinculos();
  }

  carregarVinculos(): void {
    if (!this.idClienteSelecionado) {
      return;
    }

    this.carregando = true;
    this.erro = '';
    this.sucesso = '';

    this.produtoXClienteService.listarPorCliente(this.idClienteSelecionado).subscribe({
      next: (response) => {
        this.vinculos = response.dados;
        this.carregando = false;
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível carregar os vínculos.';
        this.carregando = false;
      }
    });
  }

  produtosDisponiveis(): Combo[] {
    const idsVinculados = this.vinculos.map(x => x.idProduto);
    return this.produtos.filter(produto => !idsVinculados.includes(produto.id));
  }

  vincular(): void {
    this.erro = '';
    this.sucesso = '';

    if (!this.idClienteSelecionado) {
      this.erro = 'Selecione um cliente.';
      return;
    }

    if (!this.idProdutoSelecionado) {
      this.erro = 'Selecione um produto.';
      return;
    }

    this.salvando = true;

    this.produtoXClienteService.vincular({
      idCliente: this.idClienteSelecionado,
      idProduto: this.idProdutoSelecionado
    }).subscribe({
      next: (response) => {
        this.sucesso = response.messages?.[0] ?? 'Produto vinculado com sucesso.';
        this.idProdutoSelecionado = null;
        this.salvando = false;
        this.carregarVinculos();
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível vincular o produto.';
        this.salvando = false;
      }
    });
  }

  remover(vinculo: ProdutoXCliente): void {
    this.erro = '';
    this.sucesso = '';

    this.produtoXClienteService.remover(vinculo.idCliente, vinculo.idProduto).subscribe({
      next: (response) => {
        this.sucesso = response.messages?.[0] ?? 'Vínculo removido com sucesso.';
        this.carregarVinculos();
      },
      error: (error) => {
        this.erro = error?.error?.messages?.[0] ?? 'Não foi possível remover o vínculo.';
      }
    });
  }
}