export interface Tecnico {
    idEmpresa: number;
    idTecnico: number;
    nome: string;
    notificarNovosChamados: string;
    idUsuario?: number | null;
    nomeUsuario?: string | null;
}

export interface TecnicoCreate {
    nome: string;
    notificarNovosChamados: string;
    idUsuario?: number | null;
}

export interface TecnicoUpdate {
    nome: string;
    notificarNovosChamados: string;
    idUsuario?: number | null;
}