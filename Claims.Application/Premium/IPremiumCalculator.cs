using Claims.Domain;

namespace Claims.Application.Premium;

public interface IPremiumCalculator
{
    decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType);
}
