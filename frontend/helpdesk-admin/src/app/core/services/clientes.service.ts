import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import { Cliente, ClienteCreate, ClienteUpdate } from '../models/cliente.model';

@Injectable({
    providedIn: 'root'
})
export class ClientesService {
    private readonly apiUrl = `${environment.apiUrl}/Clientes`;

    constructor(private http: HttpClient) { }

    listar(): Observable<ResponseModel<Cliente[]>> {
        return this.http.get<ResponseModel<Cliente[]>>(this.apiUrl);
    }

    buscarPorId(idCliente: number): Observable<ResponseModel<Cliente>> {
        return this.http.get<ResponseModel<Cliente>>(`${this.apiUrl}/${idCliente}`);
    }

    criar(request: ClienteCreate): Observable<ResponseModel<Cliente>> {
        return this.http.post<ResponseModel<Cliente>>(this.apiUrl, request);
    }

    atualizar(idCliente: number, request: ClienteUpdate): Observable<ResponseModel<object>> {
        return this.http.put<ResponseModel<object>>(`${this.apiUrl}/${idCliente}`, request);
    }

    ativar(idCliente: number): Observable<ResponseModel<object>> {
        return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idCliente}/ativar`, {});
    }

    desativar(idCliente: number): Observable<ResponseModel<object>> {
        return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idCliente}/desativar`, {});
    }
}