namespace Mikarsoft.BlackHoleCore.Connector
{
    public interface IBHMethodParser
    {
        // Numeric Methods
        BHStatement Equals(MethodExpressionData methodDate);

        BHStatement EqualTo(MethodExpressionData methodDate);

        BHStatement GreaterThan(MethodExpressionData methodDate);

        BHStatement GreaterThanOrEqual(MethodExpressionData methodDate);

        BHStatement LessThan(MethodExpressionData methodDate);

        BHStatement LessThanOrEqual(MethodExpressionData methodDate);

        BHStatement Min(MethodExpressionData methodDate);

        BHStatement Max(MethodExpressionData methodDate);

        BHStatement Average(MethodExpressionData methodDate);

        BHStatement Absolut(MethodExpressionData methodDate);

        BHStatement Round(MethodExpressionData methodDate);

        BHStatement Plus(MethodExpressionData methodDate);

        BHStatement Minus(MethodExpressionData methodDate);

        BHStatement Between(MethodExpressionData methodDate);


        // Text Methods

        BHStatement Contains(MethodExpressionData methodDate);

        BHStatement Like(MethodExpressionData methodDate);

        BHStatement Length(MethodExpressionData methodDate);
        BHStatement ToUpper(MethodExpressionData methodDate);
        BHStatement ToLower(MethodExpressionData methodDate);


        // Date Methods

        BHStatement After(MethodExpressionData methodDate);
        BHStatement Before(MethodExpressionData methodDate);
    }
}
