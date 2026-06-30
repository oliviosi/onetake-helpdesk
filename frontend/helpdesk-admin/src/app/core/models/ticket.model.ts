export interface TicketResumo {
  idEmpresa: number;
  idTicket: number;
  dtHrAbertura: string;
  idCliente: number;
  dscCliente: string;
  idUsuario: number;
  nomeUsuario: string;
  assunto: string;
  idTipoOcorrencia: number;
  dscTipoOcorrencia: string;
  prioridade: string;
  idStatusTicket: number;
  dscStatusTicket: string;
  dtHrFinalizacao?: string | null;
  ticketCancelado: string;
}

export interface TicketInteracao {
  idEmpresa: number;
  idTicketInteracao: number;
  idTicket: number;
  dtHrInteracao: string;
  dscInteracao: string;
  idTecnico?: number | null;
  nomeTecnico?: string | null;
  idUsuario?: number | null;
  nomeUsuario?: string | null;
  privativo: string;
  aguardarInteracaoUsuario: string;
}

export interface TicketDetalhe extends TicketResumo {
  dscTicket?: string | null;
  idTecnicoFinalizacao?: number | null;
  nomeTecnicoFinalizacao?: string | null;
  interacoes: TicketInteracao[];
}

export interface TicketCreate {
  idCliente: number;
  idUsuario: number;
  assunto: string;
  idTipoOcorrencia: number;
  dscTicket: string;
  prioridade: string;
}

export interface TicketResponder {
  mensagem: string;
  idTecnico?: number | null;
  idUsuario?: number | null;
  privativo: string;
  aguardarInteracaoUsuario: string;
}