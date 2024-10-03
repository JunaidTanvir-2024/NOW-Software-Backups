using CQRSAndMediatrWithFluentValidationSampleApplication.Product.Data;
using CQRSAndMediatrWithFluentValidationSampleApplication.Product.Dto;
using MediatR;

namespace CQRSAndMediatrWithFluentValidationSampleApplication.Product.Query
{
    public class GetProductQuery : IRequest<ProductDto>
    {
        public string Sku { get; set; }
    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
    {
        private readonly ProductsInMemory _productsInMemory;

        public GetProductQueryHandler()
        {
            _productsInMemory = new ProductsInMemory();
        }

        public Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var list = new List<List<string>>()
            {
                new List<string>
                {
                    "Hello",
                    " ",
                    "World"
                },
                new List<string>
                {
                    "My",
                    " name is",
                    "Junaid"
                }
            };

            var d = list.Select(e => e).ToList();
            var s = list.SelectMany(e => e).ToList();







            var product = _productsInMemory.ProductDtos.FirstOrDefault(p => p.Sku.Equals(request.Sku));
            if (product == null)
            {
                throw new InvalidOperationException("Invalid product");
            }

            return Task.FromResult(product);
        }
    }
}
