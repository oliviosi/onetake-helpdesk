import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import { AlterarMinhaSenha } from '../models/minha-conta.model';

@Injectable({
    providedIn: 'root'
})
export class MinhaContaService {
    private readonly apiUrl = `${environment.apiUrl}/MinhaConta`;

    constructor(private http: HttpClient) { }

    alterarSenha(request: AlterarMinhaSenha): Observable<ResponseModel<object>> {
        return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/alterar-senha`, request);
    }
}