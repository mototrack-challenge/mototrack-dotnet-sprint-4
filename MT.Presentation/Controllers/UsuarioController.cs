using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MT.Application.Dtos;
using MT.Application.Interfaces;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace MT.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    #region :: INJEÇÃO DE DEPENDÊNCIA

    private readonly IUsuarioService _usuarioService;
    private readonly IConfiguration _configuration;

    public UsuarioController(IUsuarioService usuarioService, IConfiguration configuration)
    {
        _usuarioService = usuarioService;
        _configuration = configuration;
    }

    #endregion

    #region :: LOGIN

    [HttpPost("auth")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(AutenticacaoDTO entity)
    {
        var result = await _usuarioService.AutenticarUserAsync(entity);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);


        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_configuration["Secretkey"]!.ToString());

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(new Claim[] {

            new Claim(ClaimTypes.Email, result.Value!.Email.ToString()),
            }),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return StatusCode(result.StatusCode, new
        {
            user = result.Value.Email,
            token = tokenHandler.WriteToken(token),
        });
    }

    #endregion

    #region :: CREATE

    [HttpPost]
    [AllowAnonymous]
    //[SwaggerOperation(
    //    Summary = "Cadastra um novo usuário",
    //    Description = "Cadastra um novo usuário no sistema e retorna os dados cadastrados."
    //)]
    //[SwaggerRequestExample(typeof(UsuarioDTO), typeof(ColaboradorRequestSample))]
    //[SwaggerResponse(statusCode: 200, description: "Usuário salvo com sucesso", type: typeof(ColaboradorEntity))]
    //[SwaggerResponseExample(statusCode: 200, typeof(ColaboradorResponseSample))]
    public async Task<IActionResult> Post(UsuarioDTO dto)
    {
        var result = await _usuarioService.AdicionarUsuarioAsync(dto);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }

    #endregion

}
