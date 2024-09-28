using Ecoeden.User.Domain.Models.Core;
using MediatR;

namespace Ecoeden.User.Application.Features.User.Queries.CheckUserNameOrEmailExists;
public class CheckUserNameOrEmailExistsQuery : IRequest<Result<bool>>
{
    public CheckUserNameOrEmailExistsQuery(string checkOption, string checkValue, string correlationId)
    {
        CheckOption = checkOption;
        CheckValue = checkValue;
        CorrelationId = correlationId;
    }

    public string CheckOption { get; set; }
    public string CheckValue { get; set; }
    public string CorrelationId { get; set; }

}
