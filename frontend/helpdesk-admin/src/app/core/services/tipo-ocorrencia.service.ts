import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ResponseModel } from '../models/response.model';
import {
  TipoOcorrencia,
  TipoOcorrenciaCreate,
  TipoOcorrenciaUpdate
} from '../models/tipo-ocorrencia.model';

@Injectable({
  providedIn: 'root'
})
export class TipoOcorrenciaService {
  private readonly apiUrl = `${environment.apiUrl}/TipoOcorrencia`;

  constructor(private http: HttpClient) {}

  listar(): Observable<ResponseModel<TipoOcorrencia[]>> {
    return this.http.get<ResponseModel<TipoOcorrencia[]>>(this.apiUrl);
  }

  buscarPorId(idTipoOcorrencia: number): Observable<ResponseModel<TipoOcorrencia>> {
    return this.http.get<ResponseModel<TipoOcorrencia>>(`${this.apiUrl}/${idTipoOcorrencia}`);
  }

  criar(request: TipoOcorrenciaCreate): Observable<ResponseModel<TipoOcorrencia>> {
    return this.http.post<ResponseModel<TipoOcorrencia>>(this.apiUrl, request);
  }

  atualizar(idTipoOcorrencia: number, request: TipoOcorrenciaUpdate): Observable<ResponseModel<object>> {
    return this.http.put<ResponseModel<object>>(`${this.apiUrl}/${idTipoOcorrencia}`, request);
  }

  ativar(idTipoOcorrencia: number): Observable<ResponseModel<object>> {
    return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idTipoOcorrencia}/ativar`, {});
  }

  desativar(idTipoOcorrencia: number): Observable<ResponseModel<object>> {
    return this.http.patch<ResponseModel<object>>(`${this.apiUrl}/${idTipoOcorrencia}/desativar`, {});
  }
}