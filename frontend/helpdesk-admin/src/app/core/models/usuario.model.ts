export interface Usuario {
    idEmpresa: number;
    idUsuario: number;
    nomeUsuario: string;
    nomeCompleto?: string | null;
    email?: string | null;
    ehAdm: string;
    ativo: string;
}

export interface UsuarioCreate {
    nomeUsuario: string;
    senha: string;
    nomeCompleto?: string | null;
    email?: string | null;
    ehAdm: string;
}

export interface UsuarioUpdate {
    nomeUsuario: string;
    nomeCompleto?: string | null;
    email?: string | null;
    ehAdm: string;
    ativo: string;
}

export interface UsuarioAlterarSenha {
    novaSenha: string;
}