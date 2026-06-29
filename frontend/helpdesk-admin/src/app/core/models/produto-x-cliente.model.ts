export interface ProdutoXCliente {
    idEmpresa: number;
    idCliente: number;
    idProduto: number;
    dscProduto: string;
    ativo: string;
}

export interface ProdutoXClienteCreate {
    idCliente: number;
    idProduto: number;
}