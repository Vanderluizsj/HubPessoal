using Vanderluiz.Hub.Application.DTOs;
using Vanderluiz.Hub.Application.Interfaces;
using Vanderluiz.Hub.Application.Interfaces.Repositories;
using Vanderluiz.Hub.Application.Mappings;

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

    public async Task<PostDto?> ObterPublicadoPorSlugAsync(string slug, CancellationToken ct = default)
    {
        var post = await _posts.ObterPublicadoPorSlugAsync(slug, ct);
        return post?.ToDto();
    }
}

