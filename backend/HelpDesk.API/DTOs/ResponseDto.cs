namespace HelpDesk.API.DTOs;

public class ResponseDto<T>
{
    public int StatusCode { get; set; }
    public List<string> Messages { get; set; } = new();
    public T? Dados { get; set; }

    public static ResponseDto<T> Ok(T dados, string mensagem = "Operação realizada com sucesso")
    {
        return new ResponseDto<T>
        {
            StatusCode = 200,
            Messages = new List<string> { mensagem },
            Dados = dados
        };
    }

    public static ResponseDto<T> Error(int statusCode, string mensagem)
    {
        return new ResponseDto<T>
        {
            StatusCode = statusCode,
            Messages = new List<string> { mensagem },
            Dados = default
        };
    }
}