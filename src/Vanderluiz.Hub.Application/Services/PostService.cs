using Vanderluiz.Hub.Application.Common;
using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Mappings;
using Vanderluiz.Hub.Domain.Entities;

namespace Vanderluiz.Hub.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _posts;

    public PostService(IPostRepository posts) => _posts = posts;

    public async Task<IReadOnlyList<PostResumoDto>> ListarPublicadosAsync(int? quantidade = null, CancellationToken ct = default)
    {
        var posts = await _posts.ListarPublicadosAsync(quantidade, ct);
        return posts.Select(p => p.ToResumoDto()).ToList();
    }

    public async Task<IReadOnlyList<PostResumoDto>> ListarTodosAsync(CancellationToken ct = default)
    {
        var posts = await _posts.ListarAsync(ct);
        return posts.OrderByDescending(p => p.CriadoEm).Select(p => p.ToResumoDto()).ToList();
    }

    public async Task<PostDto?> ObterPublicadoPorSlugAsync(string slug, CancellationToken ct = default)
    {
        var post = await _posts.ObterPublicadoPorSlugAsync(slug, ct);
        return post?.ToDto();
    }

    public async Task<PostDto?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var post = await _posts.ObterPorIdAsync(id, ct);
        return post?.ToDto();
    }

    public async Task<PostDto> CriarAsync(PostInputDto input, CancellationToken ct = default)
    {
        var slug = SlugHelper.Gerar(string.IsNullOrWhiteSpace(input.Slug) ? input.Titulo : input.Slug);

        var post = new Post
        {
            Titulo = input.Titulo.Trim(),
            Slug = slug,
            Resumo = input.Resumo.Trim(),
            Conteudo = input.Conteudo,
            Publicado = input.Publicado,
            PublicadoEm = input.Publicado ? DateTime.UtcNow : null
        };

        await _posts.AdicionarAsync(post, ct);
        await _posts.SalvarAsync(ct);
        return post.ToDto();
    }

    public async Task<bool> AtualizarAsync(int id, PostInputDto input, CancellationToken ct = default)
    {
        var post = await _posts.ObterPorIdAsync(id, ct);
        if (post is null) return false;

        var slug = SlugHelper.Gerar(string.IsNullOrWhiteSpace(input.Slug) ? input.Titulo : input.Slug);

        post.Titulo = input.Titulo.Trim();
        post.Slug = slug;
        post.Resumo = input.Resumo.Trim();
        post.Conteudo = input.Conteudo;

        if (input.Publicado && !post.Publicado)
        {
            post.PublicadoEm = DateTime.UtcNow;
        }

        post.Publicado = input.Publicado;
        post.AtualizadoEm = DateTime.UtcNow;

        _posts.Atualizar(post);
        await _posts.SalvarAsync(ct);
        return true;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var post = await _posts.ObterPorIdAsync(id, ct);
        if (post is null) return false;

        _posts.Remover(post);
        await _posts.SalvarAsync(ct);
        return true;
    }
}
