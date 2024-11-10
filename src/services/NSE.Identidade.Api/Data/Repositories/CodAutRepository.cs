using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using NSE.Identidade.Api.Data.Interfaces;
using NSE.Identidade.Api.Models;
using NSE.Identidade.Api.Services.Interfaces;

namespace NSE.Identidade.Api.Data.Repositories;

public class CodAutRepository(IdentidadeDbContext context, IEmailService emailService) : ICodAutRepository
{
    public async Task GerarCodigoEnviarEmail(string email)
    {
        var random = new Random();
        var codigoAutenticacao = random.Next(100000, 1000000).ToString();
        var codAut = new CodAut(email, codigoAutenticacao);

        await Task.WhenAll(
            emailService.EnviarEmail(email, codigoAutenticacao),
            CreateOrUpdate(codAut)
        );
    }
    
    public async Task<bool> Validar(string email, string codigoAutenticacao)
    {
        return await context.CodAut.AnyAsync(c =>
            c.Email == email &&
            c.CodigoAutenticacao == codigoAutenticacao);
    }
    
    private async Task CreateOrUpdate(CodAut codAut)
    {
        var exists = await Exists(codAut.Email);
        if (exists is null)
        {
            await Create(codAut);
            return;
        }

        await Update(exists, codAut);
    }

    private async Task Create(CodAut codAut)
    {
        await context.CodAut.AddAsync(codAut);
        await context.SaveChangesAsync();
    }
    
    private async Task Update(CodAut oldCodAut, CodAut newCodAut)
    {
        context.CodAut.Entry(oldCodAut).CurrentValues.SetValues(newCodAut);
        await context.SaveChangesAsync();
    }
    
    private async Task<CodAut> Exists(string email)
    {
        return await context.CodAut.SingleOrDefaultAsync(c => c.Email == email);
    }
}