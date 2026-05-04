 namespace Claims.Application.Abstractions.Formulas;

public interface IFormula<TFormulaArgs>
{
    decimal Calculate(TFormulaArgs args);
}
