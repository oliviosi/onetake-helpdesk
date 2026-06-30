import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import {
    Usuario,
    UsuarioAlterarSenha,
    UsuarioCreate,
    UsuarioUpdate
} from '../models/usuario.model';

@Injectable({
    providedIn: 'root'
})
export class UsuariosService {
    private readonly apiUrl = `${environment.apiUrl}/Usuarios`;

    constructor(private http: HttpClient) { }

    listar(): Observable<ResponseModel<Usuario[]>> {
        return this.http.get<ResponseModel<Usuario[]>>(this.apiUrl);
    }

    buscarPorId(idUsuario: number): Observable<ResponseModel<Usuario>> {
        return this.http.get<ResponseModel<Usuario>>(`${this.apiUrl}/${idUsuario}`);
    }

    criar(request: UsuarioCreate): Observable<ResponseModel<Usuario>> {
        return this.http.post<ResponseModel<Usuario>>(this.apiUrl, request);
    }

    atualizar(idUsuario: number, request: UsuarioUpdate): Observable<ResponseModel<object>> {
        return this.http.put<ResponseModel<object>>(`${this.apiUrl}/${idUsuario}`, request);
    }

    alterarSenha(idUsuario: number, request: UsuarioAlterarSenha): Observable<ResponseModel<object>> {
        return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idUsuario}/alterar-senha`, request);
    }

    ativar(idUsuario: number): Observable<ResponseModel<object>> {
        return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idUsuario}/ativar`, {});
    }

    desativar(idUsuario: number): Observable<ResponseModel<object>> {
        return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idUsuario}/desativar`, {});
    }
}