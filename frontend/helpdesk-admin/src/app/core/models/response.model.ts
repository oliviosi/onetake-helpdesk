export interface ResponseModel<T> {
  statusCode: number;
  messages: string[];
  dados: T;
}