using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDesk.API.Entities;
using Microsoft.IdentityModel.Tokens;

namespace HelpDesk.API.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(Empresa empresa, Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new Exception("Chave JWT não configurada.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var claims = new List<Claim>
        {
            new Claim("IdEmpresa", usuario.IdEmpresa.ToString()),
            new Claim("IdUsuario", usuario.IdUsuario.ToString()),
            new Claim("CodigoAcesso", empresa.CodigoAcesso),
            new Claim("NomeUsuario", usuario.NomeUsuario),
            new Claim("EhAdm", usuario.EhAdm),
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.NomeUsuario)
        };

        if (!string.IsNullOrWhiteSpace(usuario.Email))
            claims.Add(new Claim(ClaimTypes.Email, usuario.Email));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expireMinutes = int.TryParse(_configuration["Jwt:ExpireMinutes"], out var minutes)
            ? minutes
            : 480;

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}