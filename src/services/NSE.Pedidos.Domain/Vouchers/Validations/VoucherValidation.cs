using FluentValidation;

namespace NSE.Pedidos.Domain.Vouchers.Validations;

public class VoucherValidation : AbstractValidator<Voucher>
{
    public VoucherValidation()
    {
        RuleFor(v => v.DataValidade)
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("Este voucher está expirado");

        RuleFor(v => v.Quantidade)
            .GreaterThan(0)
            .WithMessage("Este voucher já foi utilizado");

        RuleFor(v => v)
            .Must(v => v.Ativo && !v.Utilizado)
            .WithMessage("Este voucher não está mais ativo ou já foi utilizado");
    }
}