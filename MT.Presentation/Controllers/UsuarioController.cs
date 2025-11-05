using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MT.Application.Dtos;
using MT.Application.Interfaces;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.RateLimiting;
using MT.Application.Services;
using MT.Domain.Entities;
using MT.Presentation.Doc.Samples;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

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

    #region :: READ

    [HttpGet]
    [Authorize]
    [SwaggerOperation(
            Summary = "Lista de usuários",
            Description = "Retorna a lista completa de usuários cadastrados."
        )]
    [SwaggerResponse(statusCode: 200, description: "Lista retornada com sucesso", type: typeof(IEnumerable<UsuarioEntity>))]
    [SwaggerResponse(statusCode: 204, description: "Lista não tem dados")]
    [SwaggerResponseExample(statusCode: 200, typeof(ColaboradorResponseListSample))]
    [EnableRateLimiting("MotoTrack")]
    public async Task<IActionResult> Get(int deslocamento = 0, int registrosRetornados = 10)
    {
        var result = await _usuarioService.ObterTodosUsuariosAsync(deslocamento, registrosRetornados);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        var hateaos = new
        {
            data = result.Value.Data.Select(u => new {
                u.Id,
                u.Nome,
                u.Email,
                u.Senha,
                links = new
                {
                    self = Url.Action(nameof(GetId), "Usuario", new { id = u.Id }, Request.Scheme),
                    put = Url.Action(nameof(Put), "Usuario", new { id = u.Id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "Usuario", new { id = u.Id }, Request.Scheme)
                }
            }),
            links = new
            {
                self = Url.Action(nameof(GetId), "Usuario", null),
                post = Url.Action(nameof(Post), "Usuario", null, Request.Scheme),
            },
            pagina = new
            {
                result.Value.Deslocamento,
                result.Value.RegistrosRetornados,
                result.Value.TotalRegistros
            }
        };

        return StatusCode(result.StatusCode, hateaos);
    }

    [HttpGet("{id}")]
    [Authorize]
    [SwaggerOperation(
            Summary = "Obtém um usuário pelo ID",
            Description = "Retorna o usuário correspondente ao ID informado."
        )]
    [SwaggerResponse(statusCode: 200, description: "Usuário encontrado", type: typeof(UsuarioEntity))]
    [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
    [SwaggerResponseExample(statusCode: 200, typeof(ColaboradorResponseSample))]
    public async Task<IActionResult> GetId(long id)
    {
        var result = await _usuarioService.ObterUsuarioPorIdAsync(id);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return Ok(result.Value);
    }

    #endregion

    #region :: CREATE

    [HttpPost]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Cadastra um novo usuário",
        Description = "Cadastra um novo usuário no sistema e retorna os dados cadastrados."
    )]
    [SwaggerRequestExample(typeof(UsuarioDTO), typeof(ColaboradorRequestSample))]
    [SwaggerResponse(statusCode: 200, description: "Usuário salvo com sucesso", type: typeof(UsuarioEntity))]
    [SwaggerResponseExample(statusCode: 200, typeof(ColaboradorResponseSample))]
    public async Task<IActionResult> Post(UsuarioDTO dto)
    {
        var result = await _usuarioService.AdicionarUsuarioAsync(dto);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }

    #endregion

    #region :: UPDATE

    [HttpPut("{id}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Atualiza um usuário",
        Description = "Edita os dados de um usuário já cadastrado com base no ID informado."
    )]
    [SwaggerResponse(statusCode: 200, description: "Usuário atualizado com sucesso", type: typeof(UsuarioEntity))]
    [SwaggerResponse(statusCode: 400, description: "Erro na requisição (validação ou dados inválidos)")]
    [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
    [SwaggerRequestExample(typeof(ColaboradorDTO), typeof(ColaboradorRequestSample))]
    [SwaggerResponseExample(statusCode: 200, typeof(ColaboradorResponseSample))]
    public async Task<IActionResult> Put(long id, UsuarioDTO dto)
    {
        var result = await _usuarioService.EditarUsuarioAsync(id, dto);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }

    #endregion

    #region :: DELETE

    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Remove um usuário",
        Description = "Exclui permanentemente um usuário com base no ID informado."
    )]
    [SwaggerResponse(statusCode: 200, description: "Usuário removido com sucesso")]
    [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _usuarioService.DeletarUsuarioAsync(id);

        if (!result.IsSuccess) return StatusCode(result.StatusCode, result.Error);

        return StatusCode(result.StatusCode, result);
    }

    #endregion
}
