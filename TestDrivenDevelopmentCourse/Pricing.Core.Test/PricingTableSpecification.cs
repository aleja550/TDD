using FluentAssertions;
using Pricing.Core.Domain;

namespace Pricing.Core.Test;

public class PricingTableSpecification
{
    [Fact]
    public void Should_throw_argument_null_exception_if_price_tiers_is_null()
    {
        var create = () => new PricingTable(null);
        
        create.Should().ThrowExactly<ArgumentNullException>();
    }
    
    [Fact]
    public void Should_throw_argument_exception_if_has_no_price_tiers()
    {
        var create = () => new PricingTable([]);

        create.Should().ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(PricingTable.Tiers))
            .WithMessage("Missing Pricing Tiers*");
    }
    
    [Fact]
    public void Should_have_one_tier_when_created_with_one()
    {
        var pricingTable = new PricingTable([CreatePriceTier()]);
        
        pricingTable.Tiers.Should().HaveCount(1);
    }
    
    [Fact]
    public void Price_tiers_should_be_ordered_by_hour_limit()
    {
        var pricingTable = new PricingTable([
            CreatePriceTier(24),
            CreatePriceTier(4)
        ]);
        
        pricingTable.Tiers.Should().BeInAscendingOrder(
            t => t.HourLimit);
    }

    [Theory]
    [InlineData(2,1,25)]
    [InlineData(3,2, 49)]
    public void Maximum_daily_price_should_be_calculated_using_tiers_if_not_defined(decimal price1, decimal price2, decimal maxPrice)
    {
        var pricingTable = new PricingTable([
            CreatePriceTier(1, price1),
            CreatePriceTier(24, price2)
        ], null);
        
        pricingTable.GetMaxDailyPrice().Should().Be(maxPrice);
    }

    private static PriceTier CreatePriceTier(int hourLimit = 24, decimal price = 1) => new (hourLimit, price);
    
}