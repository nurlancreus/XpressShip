using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.ApiClients.Queries.GetAll;

namespace XpressShip.API.Tests.Unit.ApiClient
{
    public static class ClientsHandlers
    {
        public static async Task<IResult> HandleGetAll(ISender sender, HttpContext context, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetAllApiClientsQuery(), cancellationToken);

            return result.Match(
                onSuccess: value => Results.Ok(value),
                onFailure: error => error.HandleError(context));
        }

        // Repeat for other endpoints...
    }
}
