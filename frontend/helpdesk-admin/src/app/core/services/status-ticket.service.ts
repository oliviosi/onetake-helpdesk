import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import {
    StatusTicket,
    StatusTicketCreate,
    StatusTicketUpdate
} from '../models/status-ticket.model';

@Injectable({
    providedIn: 'root'
})
export class StatusTicketService {
    private readonly apiUrl = `${environment.apiUrl}/StatusTicket`;

    constructor(private http: HttpClient) { }

    listar(): Observable<ResponseModel<StatusTicket[]>> {
        return this.http.get<ResponseModel<StatusTicket[]>>(this.apiUrl);
    }

    buscarPorId(idStatusTicket: number): Observable<ResponseModel<StatusTicket>> {
        return this.http.get<ResponseModel<StatusTicket>>(`${this.apiUrl}/${idStatusTicket}`);
    }

    criar(request: StatusTicketCreate): Observable<ResponseModel<StatusTicket>> {
        return this.http.post<ResponseModel<StatusTicket>>(this.apiUrl, request);
    }

    atualizar(idStatusTicket: number, request: StatusTicketUpdate): Observable<ResponseModel<object>> {
        return this.http.put<ResponseModel<object>>(`${this.apiUrl}/${idStatusTicket}`, request);
    }

    excluir(idStatusTicket: number): Observable<ResponseModel<object>> {
        return this.http.delete<ResponseModel<object>>(`${this.apiUrl}/${idStatusTicket}`);
    }
}