export interface Cliente {
  idEmpresa: number;
  idCliente: number;
  dscCliente: string;
  email?: string;
  ativo: string;
  codClienteERP?: string;
}

export interface ClienteCreate {
  dscCliente: string;
  email?: string;
  codClienteERP?: string;
}

export interface ClienteUpdate {
  dscCliente: string;
  email?: string;
  codClienteERP?: string;
  ativo: string;
}