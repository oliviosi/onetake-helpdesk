import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import { Produto, ProdutoCreate, ProdutoUpdate } from '../models/produto.model';

@Injectable({
  providedIn: 'root'
})
export class ProdutosService {
  private readonly apiUrl = `${environment.apiUrl}/Produtos`;

  constructor(private http: HttpClient) {}

  listar(): Observable<ResponseModel<Produto[]>> {
    return this.http.get<ResponseModel<Produto[]>>(this.apiUrl);
  }

  buscarPorId(idProduto: number): Observable<ResponseModel<Produto>> {
    return this.http.get<ResponseModel<Produto>>(`${this.apiUrl}/${idProduto}`);
  }

  criar(request: ProdutoCreate): Observable<ResponseModel<Produto>> {
    return this.http.post<ResponseModel<Produto>>(this.apiUrl, request);
  }

  atualizar(idProduto: number, request: ProdutoUpdate): Observable<ResponseModel<object>> {
    return this.http.put<ResponseModel<object>>(`${this.apiUrl}/${idProduto}`, request);
  }

  ativar(idProduto: number): Observable<ResponseModel<object>> {
    return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idProduto}/ativar`, {});
  }

  desativar(idProduto: number): Observable<ResponseModel<object>> {
    return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idProduto}/desativar`, {});
  }
}