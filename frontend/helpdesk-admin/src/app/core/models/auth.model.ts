export interface LoginRequest {
  codigoAcesso: string;
  usuario: string;
  senha: string;
}

export interface UsuarioLogado {
  idEmpresa: number;
  idUsuario: number;
  nomeUsuario: string;
  nomeCompleto?: string;
  email?: string;
  ehAdm: string;
}

export interface LoginResponse {
  token: string;
  usuario: UsuarioLogado;
}