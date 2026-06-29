export interface TipoOcorrencia {
  idEmpresa: number;
  idTipoOcorrencia: number;
  dscTipoOcorrencia: string;
  ativo: string;
}

export interface TipoOcorrenciaCreate {
  dscTipoOcorrencia: string;
}

export interface TipoOcorrenciaUpdate {
  dscTipoOcorrencia: string;
  ativo: string;
}