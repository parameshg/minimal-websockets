using EnsureThat;
using FluentValidation;
using MediatR;
using NJsonSchema;
using WebSockets.Repositories;

namespace WebSockets.Handlers
{
    public class ValidateSchemaRequestValidator : AbstractValidator<ValidateRequest>
    {
        public ValidateSchemaRequestValidator()
        {
            RuleFor(request => request.Payload).NotEmpty().Length(1, 4096);
        }
    }

    public class ValidateRequest : IRequest<ValidateResponse>
    {
        public string? Schema { get; set; }

        public string? Payload { get; set; }
    }

    public class ValidateResponse
    {
        public bool Valid { get; set; }
    }

    public class ValidationHandler : IRequestHandler<ValidateRequest, ValidateResponse>
    {
        private ISchemaRepository Repository { get; }

        public ValidationHandler(ISchemaRepository repository)
        {
            Repository = EnsureArg.IsNotNull(repository);
        }

        public async Task<ValidateResponse> Handle(ValidateRequest request, CancellationToken cancel)
        {
            var result = new ValidateResponse();

            try
            {
                var schema = await JsonSchema.FromJsonAsync(await Repository.GetSchema(request.Schema));

                result.Valid = schema.Validate(request.Payload).Count.Equals(0);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }

            return result;
        }
    }
}