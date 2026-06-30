import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import { Combo } from '../models/combo.model';

@Injectable({
    providedIn: 'root'
})
export class CombosService {
    private readonly apiUrl = `${environment.apiUrl}/Combos`;

    constructor(private http: HttpClient) { }

    clientes(): Observable<ResponseModel<Combo[]>> {
        return this.http.get<ResponseModel<Combo[]>>(`${this.apiUrl}/clientes`);
    }

    produtos(): Observable<ResponseModel<Combo[]>> {
        return this.http.get<ResponseModel<Combo[]>>(`${this.apiUrl}/produtos`);
    }

    produtosPorCliente(idCliente: number): Observable<ResponseModel<Combo[]>> {
        return this.http.get<ResponseModel<Combo[]>>(`${this.apiUrl}/produtos-por-cliente/${idCliente}`);
    }

    usuarios(): Observable<ResponseModel<Combo[]>> {
        return this.http.get<ResponseModel<Combo[]>>(`${this.apiUrl}/usuarios`);
    }
}