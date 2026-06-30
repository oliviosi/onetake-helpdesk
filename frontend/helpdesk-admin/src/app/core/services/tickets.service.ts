import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import {
  TicketCreate,
  TicketDetalhe,
  TicketResponder,
  TicketResumo
} from '../models/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class TicketsService {
  private readonly apiUrl = `${environment.apiUrl}/Tickets`;

  constructor(private http: HttpClient) { }

  listar(): Observable<ResponseModel<TicketResumo[]>> {
    return this.http.get<ResponseModel<TicketResumo[]>>(this.apiUrl);
  }

  buscarPorId(idTicket: number): Observable<ResponseModel<TicketDetalhe>> {
    return this.http.get<ResponseModel<TicketDetalhe>>(`${this.apiUrl}/${idTicket}`);
  }

  criar(request: TicketCreate): Observable<ResponseModel<object>> {
    return this.http.post<ResponseModel<object>>(this.apiUrl, request);
  }

  responder(idTicket: number, request: TicketResponder): Observable<ResponseModel<object>> {
    return this.http.post<ResponseModel<object>>(`${this.apiUrl}/${idTicket}/responder`, request);
  }

  fechar(idTicket: number, idTecnico: number): Observable<ResponseModel<object>> {
    return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idTicket}/fechar?idTecnico=${idTecnico}`, {});
  }

  cancelar(idTicket: number): Observable<ResponseModel<object>> {
    return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idTicket}/cancelar`, {});
  }
}