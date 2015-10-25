namespace Demo.Data
{
    public static class Queries
    {
        public const string BedroomPrediction =
            "SELECT [Bedrooms] From [# of Bedrooms Prediction] NATURAL PREDICTION JOIN (SELECT '{0}' AS [Property Type]) AS t";
        public const string PricePrediction =
            "SELECT [Price] From [PricingPredictionModel] NATURAL PREDICTION JOIN ({0}) AS t";
    }
}
