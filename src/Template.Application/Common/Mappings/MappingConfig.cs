using Mapster;
using System;
using Template.Application.Common.Models.DTOs;
using Template.Domain.Entities;

namespace Template.Application.Common.Mappings;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        config.NewConfig<Customer, CustomerDto>()
             .Map(dest => dest.Id, src => src.Id)
             .Map(dest => dest.Name, src => src.Name)
             .Map(dest => dest.Email, src => src.Email);

        config.NewConfig<CustomerDto, Customer>()
             .ConstructUsing(src => new Customer(src.Name, src.Email))
             .Map(dest => dest.Id, src => src.Id)
             .Map(dest => dest.Name, src => src.Name)
             .Map(dest => dest.Email, src => src.Email);
    }
}