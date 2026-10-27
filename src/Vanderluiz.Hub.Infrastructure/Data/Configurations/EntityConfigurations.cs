using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanderluiz.Hub.Domain.Entities;

namespace Vanderluiz.Hub.Infrastructure.Data.Configurations;

public class ProjetoConfiguration : IEntityTypeConfiguration<Projeto>
{
    public void Configure(EntityTypeBuilder<Projeto> b)
    {
        b.ToTable("Projetos");
        b.Property(p => p.Titulo).HasMaxLength(150).IsRequired();
        b.Property(p => p.Slug).HasMaxLength(160).IsRequired();
        b.HasIndex(p => p.Slug).IsUnique();
        b.Property(p => p.Resumo).HasMaxLength(500).IsRequired();
        b.Property(p => p.Descricao).IsRequired();
        b.Property(p => p.UrlRepositorio).HasMaxLength(300);
        b.Property(p => p.UrlDemo).HasMaxLength(300);
        b.Property(p => p.ImagemUrl); // nvarchar(max) para suportar upload de GIF/Imagem em Base64 ou URL
        b.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

        b.HasMany(p => p.Tecnologias)
         .WithMany(t => t.Projetos)
         .UsingEntity(j => j.ToTable("ProjetoTecnologias"));
    }
}

public class TecnologiaConfiguration : IEntityTypeConfiguration<Tecnologia>
{
    public void Configure(EntityTypeBuilder<Tecnologia> b)
    {
        b.ToTable("Tecnologias");
        b.Property(t => t.Nome).HasMaxLength(80).IsRequired();
        b.HasIndex(t => t.Nome).IsUnique();
        b.Property(t => t.IconeClasse).HasMaxLength(100);
        b.Property(t => t.Categoria).HasConversion<string>().HasMaxLength(20);
        b.Property(t => t.Nivel).HasConversion<string>().HasMaxLength(20);
    }
}

public class ExperienciaConfiguration : IEntityTypeConfiguration<Experiencia>
{
    public void Configure(EntityTypeBuilder<Experiencia> b)
    {
        b.ToTable("Experiencias");
        b.Property(e => e.Cargo).HasMaxLength(120).IsRequired();
        b.Property(e => e.Empresa).HasMaxLength(120).IsRequired();
        b.Property(e => e.Local).HasMaxLength(120);
        b.Property(e => e.Descricao).IsRequired();
        b.Ignore(e => e.Atual);
    }
}

public class FormacaoConfiguration : IEntityTypeConfiguration<Formacao>
{
    public void Configure(EntityTypeBuilder<Formacao> b)
    {
        b.ToTable("Formacoes");
        b.Property(f => f.Curso).HasMaxLength(150).IsRequired();
        b.Property(f => f.Instituicao).HasMaxLength(150).IsRequired();
        b.Property(f => f.Tipo).HasConversion<string>().HasMaxLength(20);
        b.Ignore(f => f.EmAndamento);
    }
}

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> b)
    {
        b.ToTable("Posts");
        b.Property(p => p.Titulo).HasMaxLength(200).IsRequired();
        b.Property(p => p.Slug).HasMaxLength(220).IsRequired();
        b.HasIndex(p => p.Slug).IsUnique();
        b.Property(p => p.Resumo).HasMaxLength(500).IsRequired();
        b.Property(p => p.Conteudo).IsRequired();
    }
}

public class PerfilConfigConfiguration : IEntityTypeConfiguration<PerfilConfig>
{
    public void Configure(EntityTypeBuilder<PerfilConfig> b)
    {
        b.ToTable("Perfis");
        b.Property(p => p.Nome).HasMaxLength(100).IsRequired();
        b.Property(p => p.Titulo).HasMaxLength(150).IsRequired();
        b.Property(p => p.Bio).HasMaxLength(1000).IsRequired();
        b.Property(p => p.Localizacao).HasMaxLength(100);
        b.Property(p => p.Email).HasMaxLength(150);
        b.Property(p => p.GitHub).HasMaxLength(250);
        b.Property(p => p.LinkedIn).HasMaxLength(250);
        b.Property(p => p.FotoUrl); // nvarchar(max) para suportar upload de Foto em Base64 ou URL
        b.Property(p => p.Telefone).HasMaxLength(50);
    }
}
