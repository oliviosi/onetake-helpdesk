import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import { Tecnico, TecnicoCreate, TecnicoUpdate } from '../models/tecnico.model';

@Injectable({
  providedIn: 'root'
})
export class TecnicosService {
  private readonly apiUrl = `${environment.apiUrl}/Tecnicos`;

  constructor(private http: HttpClient) {}

  listar(): Observable<ResponseModel<Tecnico[]>> {
    return this.http.get<ResponseModel<Tecnico[]>>(this.apiUrl);
  }

  buscarPorId(idTecnico: number): Observable<ResponseModel<Tecnico>> {
    return this.http.get<ResponseModel<Tecnico>>(`${this.apiUrl}/${idTecnico}`);
  }

  criar(request: TecnicoCreate): Observable<ResponseModel<Tecnico>> {
    return this.http.post<ResponseModel<Tecnico>>(this.apiUrl, request);
  }

  atualizar(idTecnico: number, request: TecnicoUpdate): Observable<ResponseModel<object>> {
    return this.http.put<ResponseModel<object>>(`${this.apiUrl}/${idTecnico}`, request);
  }

  excluir(idTecnico: number): Observable<ResponseModel<object>> {
    return this.http.delete<ResponseModel<object>>(`${this.apiUrl}/${idTecnico}`);
  }
}