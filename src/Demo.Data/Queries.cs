namespace Demo.Data
{
    public static class Queries
    {
        public const string BedroomPrediction =
            "SELECT [Bedrooms] From [# of Bedrooms Prediction] NATURAL PREDICTION JOIN (SELECT '{0}' AS [Property Type]) AS t";
        public const string PricePrediction =
            "SELECT [Price], PredictStDev([Price]) AS [SD] From [PricePredictionModel] NATURAL PREDICTION JOIN ({0}) AS t";
        public const string SizePrediction =
            "SELECT [Square Mts], PredictStDev([Square Mts]) AS [SD] From [SizePredictionModel] NATURAL PREDICTION JOIN ({0}) AS t";
    }
}
