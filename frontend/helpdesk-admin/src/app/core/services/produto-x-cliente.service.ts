import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import { ProdutoXCliente, ProdutoXClienteCreate } from '../models/produto-x-cliente.model';

@Injectable({
    providedIn: 'root'
})
export class ProdutoXClienteService {
    private readonly apiUrl = `${environment.apiUrl}/ProdutoXCliente`;

    constructor(private http: HttpClient) { }

    listarPorCliente(idCliente: number): Observable<ResponseModel<ProdutoXCliente[]>> {
        return this.http.get<ResponseModel<ProdutoXCliente[]>>(`${this.apiUrl}/cliente/${idCliente}`);
    }

    vincular(request: ProdutoXClienteCreate): Observable<ResponseModel<ProdutoXCliente>> {
        return this.http.post<ResponseModel<ProdutoXCliente>>(`${this.apiUrl}/vincular`, request);
    }

    remover(idCliente: number, idProduto: number): Observable<ResponseModel<object>> {
        return this.http.delete<ResponseModel<object>>(`${this.apiUrl}/cliente/${idCliente}/produto/${idProduto}`);
    }
}