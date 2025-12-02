[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1050:Declare types in namespaces",
    Justification = "Intentional: top-level type required to define a global pipe operator.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Bug",
    "S3903:Types should be defined in named namespaces",
    Justification = "Intentional: pipe operator must be globally accessible.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1034:Nested types should not be visible",
    Justification = "Intentional: nested type used only to enable generic operator implementation.")]
public static class PipeOperator
{
    extension<T, TResult>(T)
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage",
            "CA2225:Operator overloads have named alternates",
            Justification = "Intentional: pipe operator has no meaningful named equivalent.")]
        public static TResult operator | (T source, Func<T, TResult> f) => f(source);
    }
}
