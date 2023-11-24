using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using static Umbraco.Cms.Core.Constants.Conventions;

namespace Umbraco12HeadLessDemo.Custom
{


    public class ProductSelector : ISelectorHandler, IContentIndexHandler
    {
        private const string FeaturedProductsSpecifier = "featuredProducts";
        private const string FieldName = "isFeatured";

        // Querying
        public bool CanHandle(string query)
            => query.StartsWith(FeaturedProductsSpecifier, StringComparison.OrdinalIgnoreCase);

        public SelectorOption BuildSelectorOption(string selector) =>
            new SelectorOption
            {
                FieldName = FieldName,
                Values = new[] { "y" }
            };

        // Indexing
        public IEnumerable<IndexFieldValue> GetFieldValues(IContent content, string? culture)
        {
            if (content.ContentType.Alias.InvariantEquals("product") == false)
            {
                return Enumerable.Empty<IndexFieldValue>();
            }

            var isFeatured = content.GetValue<bool>("isFeatured");

            return new[]
            {
            new IndexFieldValue
            {
                FieldName = FieldName,
                Values = new object[] { isFeatured ? "y" : "n" }
            }
        };
        }

        public IEnumerable<IndexField> GetFields()
            => new[]
            {
            new IndexField
            {
                FieldName = FieldName,
                FieldType = FieldType.StringRaw,
                VariesByCulture = false
            }
            };
    }



    public class ProductFilter : IFilterHandler, IContentIndexHandler
    {
        private const string ProductFilterSpecifier = "product:";
        private const string FieldName = "category";

        // Querying
        public bool CanHandle(string query)
            => query.StartsWith(ProductFilterSpecifier, StringComparison.OrdinalIgnoreCase);

        public FilterOption BuildFilterOption(string filter)
        {
            var fieldValue = filter.Substring(ProductFilterSpecifier.Length);

            // There might be several values for the filter
            var values = fieldValue.Split(',');

            return new FilterOption
            {
                FieldName = FieldName,
                Values = values,
                Operator = FilterOperation.Contains
            };
        }

        // Indexing
        public IEnumerable<IndexFieldValue> GetFieldValues(IContent content, string? culture)
        {
            if (content.ContentType.Alias.InvariantEquals("product") == false)
            {
                return Enumerable.Empty<IndexFieldValue>();
            }

            var tags = content.GetValue<string>("category")?.Replace(","," ");

            if (tags is null)
            {
                return Array.Empty<IndexFieldValue>();
            }


            return new[]
            {
            new IndexFieldValue
            {
                FieldName = FieldName,
                Values = new object[] { tags }
            }
        };
        }

        public IEnumerable<IndexField> GetFields() => new[]
        {
        new IndexField
        {
            FieldName = FieldName,
            FieldType = FieldType.StringRaw,
            VariesByCulture = false
        }
    };
    }
}
