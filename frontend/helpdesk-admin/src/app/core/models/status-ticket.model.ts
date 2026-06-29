export interface StatusTicket {
  idEmpresa: number;
  idStatusTicket: number;
  dscStatusTicket: string;
  tipoTicket: string;
  cor?: string;
}

export interface StatusTicketCreate {
  dscStatusTicket: string;
  tipoTicket: string;
  cor?: string;
}

export interface StatusTicketUpdate {
  dscStatusTicket: string;
  tipoTicket: string;
  cor?: string;
}