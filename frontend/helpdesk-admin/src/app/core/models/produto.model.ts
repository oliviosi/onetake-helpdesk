export interface Produto {
  idEmpresa: number;
  idProduto: number;
  dscProduto: string;
  ativo: string;
}

export interface ProdutoCreate {
  dscProduto: string;
}

export interface ProdutoUpdate {
  dscProduto: string;
  ativo: string;
}