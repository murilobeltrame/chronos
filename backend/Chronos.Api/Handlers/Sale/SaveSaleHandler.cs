﻿using Chronos.Api.Data;
using Chronos.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Chronos.Api.Handlers.Sale;

public interface ISaveSaleHandler
{
    Task Handle(Request request);

    public record Request(Guid CompanyId, DateTime Date, decimal Total, List<SaleItem> Items)
    {
        public record SaleItem(Guid ProductId, int Quantity, decimal Price, decimal Total);
    };
}

public class SaveSaleHandler(Context context) : ISaveSaleHandler
{
    public async Task Handle(ISaveSaleHandler.Request request)
    {
        Validate(request);

        var sale = new Entities.Sale
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            Date = request.Date,
            Total = request.Total,
            Items = request.Items.Select(i => new SaleItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                Total = i.Total
            }).ToList()
        };

        await context.Set<Entities.Sale>().AddAsync(sale);
        await context.SaveChangesAsync();
    }

    private static void Validate(ISaveSaleHandler.Request request)
    {
        if (request.CompanyId == Guid.Empty) throw new ValidationException("CompanyId cannot be empty.");
        if (request.Items == null || !request.Items.Any()) throw new ValidationException("Sale must contain at least one item.");
        if (request.Total <= 0) throw new ValidationException("Total must be greater than zero.");
    }
}
