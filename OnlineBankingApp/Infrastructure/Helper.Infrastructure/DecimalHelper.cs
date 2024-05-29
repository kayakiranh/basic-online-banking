namespace Helper.Infrastructure
{
    [Serializable]
    public static class DecimalHelper
    {
        public static bool GreaterThanZero(this decimal value)
        {
            return value > 0;
        }
    }
}